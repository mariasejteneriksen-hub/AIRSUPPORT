# ============================================================
# AIRSUPPORT — Churn-model (logistisk regression)
# Bygger videre på ModelFeatures.csv fra /korrelation i appen
# ============================================================

# ---- 0) Pakker ----
# install.packages("caret")  # kør kun første gang, hvis den ikke allerede er installeret
library(caret)

# ---- 1) Indlæs data ----
# "NULL" og tomme celler fra SQL-eksporten skal behandles som ægte manglende værdier (NA),
# ikke som bogstavelig tekst.
data <- read.csv("ModelFeatures.csv", stringsAsFactors = FALSE, na.strings = c("NA", "NULL", ""))

str(data)
summary(data)

# ---- 1.5) Kunder uden identificerbart CustomerNr ----
# Nogle rækker mangler CustomerNr helt (formentlig ugyldige/"spøgelses"-rækker i master_customers,
# jf. de tomme fakturalinjer vi fandt tidligere). De kan ikke bruges i modellen (ingen ID til at
# koble tilbage til en rigtig kunde), men gemmes til senere undersøgelse i stedet for at blive smidt væk.
uden_id <- data[is.na(data$CustomerNr), ]
write.csv(uden_id, "kunder_uden_id.csv", row.names = FALSE)
nrow(uden_id)

data <- data[!is.na(data$CustomerNr), ]
nrow(data)

# ---- 2) Undersøg manglende værdier (svarer til trin 6 i planen) ----
na_counts <- sapply(data, function(x) sum(is.na(x)))
print(na_counts[na_counts > 0])

# LifetimeYears/AverageTurnover er NA for kunder uden CustomerSince — dvs. reelt ikke
# "rigtige" kunder med en tenure at måle churn på (fx prospects). De filtreres fra.
data <- data[!is.na(data$LifetimeYears), ]

# Enkelte kunder har LifetimeYears <= 0 (CustomerSince på/efter ChurnDate — sandsynligvis en
# datafejl eller ekstremt kortvarig kunderelation), hvilket gør AverageTurnover NA (division med ~0).
data <- data[!is.na(data$AverageTurnover), ]

# RFM-kolonnerne (DaysSinceLastInvoice, AvgDaysBetweenInvoices, InvoiceTrend, PPSInvoiceCount,
# OCInvoiceCount, ActiveProgramCount, AvgLineAmount) er NA for kunder, der er "rigtige" kunder,
# men aldrig har fået en faktura. Det er informativt i sig selv (meget lav aktivitet), så vi
# laver et flag i stedet for bare at slette rækkerne.
data$HasInvoiceHistory <- as.integer(!is.na(data$PPSInvoiceCount))

rfm_cols <- c("DaysSinceLastInvoice", "AvgDaysBetweenInvoices", "InvoiceTrend",
              "PPSInvoiceCount", "OCInvoiceCount", "ActiveProgramCount", "AvgLineAmount")

for (col in rfm_cols) {
  data[[col]][is.na(data[[col]])] <- 0
}

# Bekræft at der ikke er flere NA'er tilbage i prædiktorerne
na_counts_after <- sapply(data, function(x) sum(is.na(x)))
print(na_counts_after[na_counts_after > 0])

# ---- 3) Gør target til factor ----
data$IsChurned <- factor(data$IsChurned, levels = c(0, 1), labels = c("Aktiv", "Churnet"))
table(data$IsChurned)  # tjek fordelingen — hvor stor en andel er churnet?

# ---- 4) Stratificeret train/test-split (svarer til trin 7) ----
set.seed(42)  # for reproducerbarhed
train_index <- createDataPartition(data$IsChurned, p = 0.80, list = FALSE)
train_data <- data[train_index, ]
test_data  <- data[-train_index, ]

# Tjek at andelen af churnede er nogenlunde ens i begge sæt
prop.table(table(train_data$IsChurned))
prop.table(table(test_data$IsChurned))

# ---- 4.5) Standardisér de kontinuerte prædiktorer (z-score) ----
# Ikke strengt nødvendigt for selve modelfitningen (logistisk regression er skala-invariant
# i forhold til p-værdier/signifikans), men gør koefficienterne sammenlignelige på tværs af
# variable med meget forskellige skalaer (fx AverageTurnover i hundredtusinder vs. TailsOnPPS 0-20),
# og hjælper den numeriske optimering.
#
# Skaleringsparametrene beregnes KUN på træningssættet og genbruges på testsættet —
# ellers lækker information fra test ind i træning.
# CustomerNr er kun til reference, IsChurned er target.
# HasInvoiceHistory er konstant (kun værdien 1) efter NA-filtreringen ovenfor — alle "rigtige"
# kunder (med CustomerSince) har fået mindst én faktura, så flagget tilfører ingen information
# og udelukkes helt fra prædiktorerne (ellers giver glm en "singularities"-advarsel).
predictor_cols <- setdiff(names(data), c("CustomerNr", "IsChurned", "HasInvoiceHistory"))

train_center <- sapply(train_data[predictor_cols], mean)
train_sd     <- sapply(train_data[predictor_cols], sd)

train_data[predictor_cols] <- scale(train_data[predictor_cols], center = train_center, scale = train_sd)
test_data[predictor_cols]  <- scale(test_data[predictor_cols],  center = train_center, scale = train_sd)

# ---- 5) Fit fuld logistisk regression (svarer til trin 8) ----
formula_full <- as.formula(paste("IsChurned ~", paste(predictor_cols, collapse = " + ")))

model_full <- glm(formula_full, data = train_data, family = binomial)
summary(model_full)

# ---- 6) Forenkl modellen ud fra p-værdier ----
# Kig på summary(model_full) — behold kun prædiktorer med p < 0.05 (eller 0.10, hvis I vil være mindre strenge)
# Eksempel (ret listen til, når I har set resultatet ovenfor):
# significant_predictors <- c("DaysSinceLastInvoice", "AverageTurnover", "TailsOnPPS")
# formula_reduced <- as.formula(paste("IsChurned ~", paste(significant_predictors, collapse = " + ")))
# model_reduced <- glm(formula_reduced, data = train_data, family = binomial)
# summary(model_reduced)

# Alternativ: automatisk stepwise-udvælgelse (bruger AIC, ikke kun p-værdier)
model_step <- step(model_full, direction = "backward")
summary(model_step)

# ---- 7) Evaluér på testsættet ----
test_probs <- predict(model_step, newdata = test_data, type = "response")
test_pred  <- factor(ifelse(test_probs > 0.5, "Churnet", "Aktiv"), levels = c("Aktiv", "Churnet"))

confusionMatrix(test_pred, test_data$IsChurned, positive = "Churnet")

# ---- 8) Odds ratios (lettere at fortolke end rå koefficienter) ----
# NB: fordi prædiktorerne er standardiserede, skal odds ratios nu læses som "pr. 1 standardafvigelses
# stigning" i variablen, ikke "pr. 1 enheds stigning" (fx "pr. 1 SD højere AverageTurnover", ikke "pr. 1 kr.")
odds_ratios <- exp(cbind(OR = coef(model_step), confint(model_step)))
round(odds_ratios, 3)

# ============================================================
# 9) REVIDERET MODEL — udelukker variable med mistænkt data leakage
# ============================================================
# Undersøgelse af model_full afslørede to problemer:
#
# 1) AverageTurnover havde høj VIF (4,08) OG modsat fortegn af det rå, ukontrollerede billede
#    (churnede har lavere gennemsnitlig omsætning i virkeligheden, men modellen viste "højere
#    omsætning = højere churn-odds") — tegn på, at koefficienten var forvrænget af
#    multikollinearitet (mest med FleetSize/TailsOnPPS), ikke et ægte fund.
#
# 2) TailsOnPPS, TailsFlightWatchTerrestrial, TailsFlightWatchSatellite, TailsNotamMonitoring,
#    FleetSize og Balance kommer alle fra LIME's "nu-status"-kundekort, ikke en historisk log.
#    Churnede kunder havde systematisk markant lavere værdier på ALLE seks (fx TailsOnPPS
#    gennemsnit 0,59 mod 4,68 for aktive — 8x lavere), hvilket tyder på, at felterne bliver
#    nulstillet/opdateret, EFTER en kunde er markeret som churnet — dvs. mulig data leakage,
#    ikke et ægte tidligt varselstegn. Bør bekræftes med Air Support (bliver disse felter
#    automatisk opdateret ved opsigelse?).
#
# PPSInvoiceCount/OCInvoiceCount m.fl. (fakturalinje-baserede) er IKKE ramt af samme problem,
# fordi de bygger på tidsstemplede, historiske fakturaer, og vi har allerede bevist, at der
# ikke findes fakturalinjer efter ChurnDate.

predictor_cols_v3 <- c("PriorityCustomer", "LifetimeYears", "DaysSinceLastInvoice",
                        "AvgDaysBetweenInvoices", "InvoiceTrend", "PPSInvoiceCount",
                        "OCInvoiceCount", "ActiveProgramCount", "AvgLineAmount")

formula_v3 <- as.formula(paste("IsChurned ~", paste(predictor_cols_v3, collapse = " + ")))
model_v3 <- glm(formula_v3, data = train_data, family = binomial)

summary_table_v3 <- summary(model_v3)$coefficients
odds_ratios_v3 <- exp(cbind(OR = coef(model_v3), confint(model_v3)))
round(cbind(summary_table_v3, odds_ratios_v3), 4)

library(car)
vif(model_v3)  # skal nu være markant lavere over hele linjen

# ---- Metode 1: Automatisk baglæns udvælgelse (AIC) ----
model_step_v3 <- step(model_v3, direction = "backward")
summary(model_step_v3)

# ---- Metode 2: Manuel forenkling til kun de klart signifikante (p < 0.05) ----
model_manual <- glm(IsChurned ~ PriorityCustomer + DaysSinceLastInvoice + AvgDaysBetweenInvoices,
                     data = train_data, family = binomial)
summary(model_manual)

# ---- Sammenlign modellerne (lavere AIC = bedre balance mellem tilpasning og enkelthed) ----
AIC(model_v3, model_step_v3, model_manual)

# ---- Evaluér begge forenklede modeller på testsættet ----
evaluate_model <- function(model, test_data) {
  probs <- predict(model, newdata = test_data, type = "response")
  pred  <- factor(ifelse(probs > 0.5, "Churnet", "Aktiv"), levels = c("Aktiv", "Churnet"))
  confusionMatrix(pred, test_data$IsChurned, positive = "Churnet")
}

evaluate_model(model_step_v3, test_data)
evaluate_model(model_manual, test_data)

# ============================================================
# 10) VISUALISERING — den endelige model (model_manual)
# ============================================================
# install.packages(c("pROC", "ggplot2"))  # kør kun første gang
library(pROC)
library(ggplot2)

test_probs_manual <- predict(model_manual, newdata = test_data, type = "response")
test_pred_manual   <- factor(ifelse(test_probs_manual > 0.5, "Churnet", "Aktiv"),
                              levels = c("Aktiv", "Churnet"))

# ---- 10.1) ROC-kurve + AUC ----
# AUC måler modellens evne til at rangere churnede højere end aktive, uafhængigt af tærsklen
# på 0.5 — 0.5 = ikke bedre end at gætte, 1.0 = perfekt adskillelse.
roc_obj <- roc(test_data$IsChurned, test_probs_manual, levels = c("Aktiv", "Churnet"), direction = "<")
plot(roc_obj, main = "ROC-kurve — churn-model", col = "#2563eb", lwd = 2)
auc(roc_obj)

# ---- 10.2) Odds ratios som forest plot ----
odds_ratios_manual <- exp(cbind(OR = coef(model_manual), confint(model_manual)))
or_table <- data.frame(
  Variable = rownames(odds_ratios_manual)[-1],  # ekskluder intercept
  OR       = odds_ratios_manual[-1, "OR"],
  Lower    = odds_ratios_manual[-1, "2.5 %"],
  Upper    = odds_ratios_manual[-1, "97.5 %"]
)

ggplot(or_table, aes(x = reorder(Variable, OR), y = OR)) +
  geom_point(size = 3, color = "#2563eb") +
  geom_errorbar(aes(ymin = Lower, ymax = Upper), width = 0.2, color = "#2563eb") +
  geom_hline(yintercept = 1, linetype = "dashed", color = "red") +  # OR=1 = ingen effekt
  coord_flip() +
  labs(title = "Odds ratios med 95% konfidensinterval",
       subtitle = "Under 1 (stiplet linje) = lavere churn-odds, over 1 = højere churn-odds",
       x = "", y = "Odds Ratio (pr. 1 SD stigning)") +
  theme_minimal()

# ---- 10.3) Konfusionsmatrix som heatmap ----
cm <- confusionMatrix(test_pred_manual, test_data$IsChurned, positive = "Churnet")
cm_table <- as.data.frame(cm$table)

ggplot(cm_table, aes(x = Reference, y = Prediction, fill = Freq)) +
  geom_tile(color = "white") +
  geom_text(aes(label = Freq), size = 6, color = "black") +
  scale_fill_gradient(low = "#eff6ff", high = "#2563eb") +
  labs(title = "Konfusionsmatrix — churn-model (testsæt)") +
  theme_minimal()

# ---- 10.4) Fordeling af forudsagte sandsynligheder, opdelt på faktisk status ----
# Viser hvor godt modellen adskiller de to grupper — jo mindre overlap, jo bedre
prob_df <- data.frame(Prob = test_probs_manual, Actual = test_data$IsChurned)

ggplot(prob_df, aes(x = Prob, fill = Actual)) +
  geom_density(alpha = 0.5) +
  geom_vline(xintercept = 0.5, linetype = "dashed", color = "black") +
  labs(title = "Fordeling af forudsagt churn-sandsynlighed",
       x = "Forudsagt sandsynlighed for churn", y = "Tæthed", fill = "Faktisk status") +
  theme_minimal()

# ============================================================
# 11) SAMMENLIGNING MED ANDRE MODELTYPER
# ============================================================
# Logistisk regression er en klassisk, veletableret baseline — simpel og letfortolkelig.
# Her tjekker vi, om mere fleksible modeller (der kan fange ikke-lineære sammenhænge og
# interaktioner) reelt præsterer bedre på PRÆCIS samme trænings-/testsæt.

# install.packages(c("rpart", "rpart.plot", "randomForest"))
library(rpart)
library(rpart.plot)
library(randomForest)

formula_final <- IsChurned ~ PriorityCustomer + DaysSinceLastInvoice + AvgDaysBetweenInvoices

# ---- 11.1) Decision Tree ----
model_tree <- rpart(formula_final, data = train_data, method = "class")
rpart.plot(model_tree, main = "Beslutningstræ — churn")

tree_pred <- predict(model_tree, newdata = test_data, type = "class")
confusionMatrix(tree_pred, test_data$IsChurned, positive = "Churnet")

# ---- 11.2) Random Forest ----
set.seed(42)
model_rf <- randomForest(formula_final, data = train_data, ntree = 500, importance = TRUE)

rf_pred <- predict(model_rf, newdata = test_data)
confusionMatrix(rf_pred, test_data$IsChurned, positive = "Churnet")

varImpPlot(model_rf, main = "Variable importance — Random Forest")

# ---- 11.3) Sammenlign AUC på tværs af de tre modeltyper ----
tree_probs <- predict(model_tree, newdata = test_data, type = "prob")[, "Churnet"]
roc_tree   <- roc(test_data$IsChurned, tree_probs, levels = c("Aktiv", "Churnet"), direction = "<")

rf_probs <- predict(model_rf, newdata = test_data, type = "prob")[, "Churnet"]
roc_rf   <- roc(test_data$IsChurned, rf_probs, levels = c("Aktiv", "Churnet"), direction = "<")

plot(roc_obj, col = "#2563eb", lwd = 2, main = "ROC-sammenligning — tre modeltyper")
plot(roc_tree, col = "#dc2626", lwd = 2, add = TRUE)
plot(roc_rf, col = "#16a34a", lwd = 2, add = TRUE)
legend("bottomright", legend = c("Logistisk regression", "Decision Tree", "Random Forest"),
       col = c("#2563eb", "#dc2626", "#16a34a"), lwd = 2)

cat("AUC - Logistisk regression:", round(auc(roc_obj), 4), "\n")
cat("AUC - Decision Tree:       ", round(auc(roc_tree), 4), "\n")
cat("AUC - Random Forest:       ", round(auc(roc_rf), 4), "\n")
