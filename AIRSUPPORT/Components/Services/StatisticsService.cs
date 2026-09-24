namespace AIRSUPPORT.Components.Services
{
    public class StatisticsService
    {
        // Middelværdi (mean) 
        public double Mean(List<double> values)
        {
            if (values.Count == 0)
                return 0;

            return values.Sum() / values.Count;
        }

        //  Median 
        public double Median(List<double> values)
        {
            return Percentile(values, 50);
        }

        //  Typetal (mode) 
        // Returnerer den/de hyppigst forekommende værdi(er). Kan være flere, hvis der er lige mange.
        public List<double> Mode(List<double> values)
        {
            if (values.Count == 0)
                return new List<double>();

            var maxFrequency = values
                .GroupBy(v => v)
                .Max(g => g.Count());

            return values
                .GroupBy(v => v)
                .Where(g => g.Count() == maxFrequency)
                .Select(g => g.Key)
                .OrderBy(v => v)
                .ToList();
        }

        //  Max/min (range)
        public double Range(List<double> values)
        {
            if (values.Count == 0)
                return 0;

            return values.Max() - values.Min();
        }

        // Percentil (bruges af Median, Q1 og Q3) 
        // Lineær interpolation mellem de to nærmeste rækker (samme metode som Excel's PERCENTILE.INC)
        public double Percentile(List<double> values, double percentile)
        {
            if (values.Count == 0)
                return 0;

            var sorted = values.OrderBy(v => v).ToList();

            if (sorted.Count == 1)
                return sorted[0];

            var rank = (percentile / 100.0) * (sorted.Count - 1);
            var lowerIndex = (int)Math.Floor(rank);
            var upperIndex = (int)Math.Ceiling(rank);

            if (lowerIndex == upperIndex)
                return sorted[lowerIndex];

            var fraction = rank - lowerIndex;
            return sorted[lowerIndex] + fraction * (sorted[upperIndex] - sorted[lowerIndex]);
        }

        //  Kvartiler og IQR 
        public double Q1(List<double> values) => Percentile(values, 25);

        public double Q3(List<double> values) => Percentile(values, 75);

        public double InterquartileRange(List<double> values) => Q3(values) - Q1(values);

        // Varians 
        // Populationsvarians, da vi regner på hele datasættet (alle kunder), ikke en stikprøve
        public double Variance(List<double> values)
        {
            if (values.Count == 0)
                return 0;

            var mean = Mean(values);
            return values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        }

        //  Standardafvigelse 
        public double StandardDeviation(List<double> values)
        {
            return Math.Sqrt(Variance(values));
        }

        //  Femtalsopsummering (five-number summary) 
        public (double Min, double Q1, double Median, double Q3, double Max) FiveNumberSummary(List<double> values)
        {
            if (values.Count == 0)
                return (0, 0, 0, 0, 0);

            return (values.Min(), Q1(values), Median(values), Q3(values), values.Max());
        }

        //  Histogram
        // Inddeler værdierne i et antal lige store intervaller (buckets) og tæller hvor mange der falder i hvert.
        // trimOutliers = true (default): intervalbredden beregnes ud fra P1-P99 i stedet for min/max, så et par
        // ekstreme outliers (fx en dummy-værdi på 999.999.999) ikke gør alle de andre søjler usynlige.
        // Værdier uden for P1/P99 tælles stadig med — de lander bare i yderste bucket.
        public List<(string RangeLabel, int Count)> Histogram(List<double> values, int bucketCount = 10, bool trimOutliers = true)
        {
            if (values.Count == 0 || bucketCount <= 0)
                return new List<(string, int)>();

            var min = trimOutliers ? Percentile(values, 1) : values.Min();
            var max = trimOutliers ? Percentile(values, 99) : values.Max();

            if (min == max)
                return new List<(string, int)> { (min.ToString("N0"), values.Count) };

            var bucketWidth = (max - min) / bucketCount;
            var buckets = new int[bucketCount];

            foreach (var value in values)
            {
                var index = (int)((value - min) / bucketWidth);
                if (index >= bucketCount)
                    index = bucketCount - 1; // værdier over P99 lander i sidste bucket
                if (index < 0)
                    index = 0; // værdier under P1 lander i første bucket

                buckets[index]++;
            }

            var result = new List<(string RangeLabel, int Count)>();
            for (var i = 0; i < bucketCount; i++)
            {
                var rangeStart = min + i * bucketWidth;
                var rangeEnd = rangeStart + bucketWidth;
                result.Add(($"{rangeStart:N0}-{rangeEnd:N0}", buckets[i]));
            }

            return result;
        }
    }
}
