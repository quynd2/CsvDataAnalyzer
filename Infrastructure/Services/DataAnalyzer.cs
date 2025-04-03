using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class DataAnalyzer : IDataAnalyzer
    {
        private readonly ILogger<DataAnalyzer> _logger;

        public DataAnalyzer(ILogger<DataAnalyzer> logger)
        {
            _logger = logger;
        }

        public double GetMinimum(List<DataRecord> data) => data?.Min(x => x.Value) ?? 0;
        public double GetMaximum(List<DataRecord> data) => data?.Max(x => x.Value) ?? 0;
        public double GetAverage(List<DataRecord> data) => data?.Average(x => x.Value) ?? 0;

        public (DateTime startTime, DateTime endTime, double value) GetMostExpensiveWindow(List<DataRecord> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                    return (DateTime.MinValue, DateTime.MinValue, 0);

                data = data.OrderBy(x => x.Time).ToList();
                double maxValue = data.Max(x => x.Value);
                var maxRecord = data.Last(x => x.Value == maxValue);
                var nextRecord = data.FirstOrDefault(x => x.Time > maxRecord.Time && x.Value != maxValue);

                return (maxRecord.Time, nextRecord?.Time ?? maxRecord.Time, maxValue);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while analyzing data.");
                return (DateTime.MinValue, DateTime.MinValue, 0);
            }
        }
    }
}
