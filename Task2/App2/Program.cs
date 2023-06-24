
using OfficeOpenXml;


namespace FinancialDataAnalysisTool
{
    public class AssetData
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }

    public class FinancialDataAnalyzer
    {
        private List<AssetData> data;

        public FinancialDataAnalyzer(List<AssetData> data)
        {
            this.data = data;
        }

        public double CalculateReturn(DateTime startDate, DateTime endDate)
        {
            var startData = data.OrderBy(d => Math.Abs((d.Date - startDate).TotalDays)).FirstOrDefault();
            var endData = data.OrderBy(d => Math.Abs((d.Date - endDate).TotalDays)).FirstOrDefault();

            if (startData == null || endData == null)
            {
                throw new ArgumentException("No available data within the specified range.");
            }

            double startValue = startData.Value;
            double endValue = endData.Value;
            return (endValue - startValue) / startValue;
        }



        public double CalculateVolatility()
        {
            double meanReturn = data.Average(d => d.Value);
            double sumSquaredDiff = data.Sum(d => Math.Pow(d.Value - meanReturn, 2));
            double variance = sumSquaredDiff / (data.Count - 1);
            return Math.Sqrt(variance);
        }

        public double CalculateCorrelation(List<AssetData> otherData)
        {
            var mergedData = data.Join(
                otherData,
                d => d.Date,
                od => od.Date,
                (d, od) => new { Value1 = d.Value, Value2 = od.Value }
            );

            double meanValue1 = mergedData.Average(d => d.Value1);
            double meanValue2 = mergedData.Average(d => d.Value2);
            double sumProduct = mergedData.Sum(d => (d.Value1 - meanValue1) * (d.Value2 - meanValue2));
            double sumSquaredDiff1 = mergedData.Sum(d => Math.Pow(d.Value1 - meanValue1, 2));
            double sumSquaredDiff2 = mergedData.Sum(d => Math.Pow(d.Value2 - meanValue2, 2));

            double correlation = sumProduct / Math.Sqrt(sumSquaredDiff1 * sumSquaredDiff2);
            return correlation;
        }
    }

    public class FinancialDataAnalyzerApp
    {
        
        private static List<AssetData> ReadDataFromExcel(string filePath)
        {
            var data = new List<AssetData>();

            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var dateCell = worksheet.Cells[row, 2];
                    var valueCell = worksheet.Cells[row, 3];

                    if (DateTime.TryParse(dateCell.GetValue<string>(), out DateTime date) && double.TryParse(valueCell.GetValue<string>(), out double assetValue))
                    {
                        data.Add(new AssetData { Date = date, Value = assetValue });
                    }
                    else
                    {
                        // Handle invalid data format
                        Console.WriteLine($"Invalid data format at row {row}");
                    }
                }
            }

            return data;
        }



        public static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                string filePath = "D:\\TechnoBrain data\\stock_prices_latest.xlsx";
                var data = ReadDataFromExcel(filePath);
                var analyzer = new FinancialDataAnalyzer(data);

                // Calculate returns
                var startDate = new DateTime(2023, 1, 1);
                var endDate = new DateTime(2023, 6, 1);
                double returns = analyzer.CalculateReturn(startDate, endDate);
                Console.WriteLine($"Returns from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}: {returns:P}");

                // Calculate volatility
                double volatility = analyzer.CalculateVolatility();
                Console.WriteLine($"Volatility: {volatility:P}");

                // Calculate correlation with another asset
                var otherData = ReadDataFromExcel("D:\\TechnoBrain data\\stock_prices_latest.xlsx");
                double correlation = analyzer.CalculateCorrelation(otherData);
                Console.WriteLine($"Correlation with other asset: {correlation:F}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
