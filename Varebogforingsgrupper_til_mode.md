# Afklaring af Varebogføringsgrupper (Posting Group) — til møde med vejleder

## Baggrund
I `Sales Invoice Line`-tabellen i NAV (kolonnen `Posting Group`, vises i NAV's brugerflade som "Inventory Posting Group"/"Varebogføringsgruppe") bruges der langt flere koder, end der er registreret i opsætningstabellen `Inventory Posting Group` (som kun indeholder 19 koder).

## Kendte koder (fra opsætningstabellen, bekræftet af Airsupport)

| Kode | Beskrivelse |
|---|---|
| ADMIN COST | Admin fees |
| AERODATA | Reseller AERODATA |
| AIREON | Reseller |
| APG | Reseller APG |
| DEV | Development, pris på kundebeta... |
| FLIGHTAWAR | Sat-based tracking |
| HOSTING | AS hosting of PPS etc |
| INTEGRATIO | Third part integrations, no cost, ... |
| LOGIPAD | Reseller LPD |
| OC | OpsControl Flight Watch/tracking |
| OC, FSP | OpsControl for FSP, Pay per Use |
| OC, NOTAM | OC NOTAM |
| OC, TERR | OC terrestial |
| OC, VAR | Various OC that can not match o... |
| PPS | PPS |
| RAIM | Reseller RAIM |
| REPORTS | Lobster etc reports |
| SMS | SMS Service |
| UNKNOWN | Other/unknown - old - no longer ... |

## Ukendte koder (findes i rigtige fakturalinjer, men IKKE i opsætningstabellen)

Sorteret efter hvor mange varelinjer (Type = Item) de dækker over — de øverste er vigtigst at få afklaret:

| Kode | Antal linjer | Vores foreløbige gæt (baseret på Description-tekst) |
|---|---|---|
| **SWL** | **137.332** | Software-relateret (beskrivelserne pegede på noget software) |
| UPDATE | 26.496 | ? |
| PPU | 11.846 | ? |
| BASIS | 11.254 | ? |
| INST | 6.601 | ? |
| MISC | 6.294 | ? |
| DWI | 4.546 | ? |
| TRAIN | 1.366 | Muligvis "Training" (matcher OC/SMS-stil af forkortelser) |
| FEES | 1.138 | Muligvis gebyrer |
| SWLBA | 339 | ? |
| WSI | 279 | ? |
| SWLB | 217 | ? |
| WX | 199 | ? |
| RFS | 148 | ? |
| STYKLISTE | 64 | ? |
| AS HOSTING | 57 | Muligvis en variant af "HOSTING" |
| SPLIT | 23 | ? |
| SPIMST | 5 | ? |
| TAILLOG | 1 | ? |

*(Tom/blank Posting Group: 282.818 linjer — svarer til kommentarlinjer uden vare, ikke noget der kræver afklaring)*

## Spørgsmål til mødet
1. Er opsætningstabellen `Inventory Posting Group` bevidst ufuldstændig, eller mangler der bare vedligeholdelse?
2. Kan I få officielle beskrivelser for i hvert fald de største ukendte koder (SWL, UPDATE, PPU, BASIS, INST, MISC, DWI)?
3. Er nogle af de mindre koder (under 100 linjer) reelt "døde"/historiske, ligesom `UNKNOWN`, og kan trygt ignoreres?
