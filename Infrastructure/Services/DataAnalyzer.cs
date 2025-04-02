using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services
{
    public class DataAnalyzer : IDataAnalyzer
    {
        private readonly ILogger<DataAnalyzer> _logger;

        public DataAnalyzer(ILogger<DataAnalyzer> logger)
        {
            _logger = logger;
        }

        public double GetMinimum(List<DataRecord> data) => data.Min(x => x.Value);
        public double GetMaximum(List<DataRecord> data) => data.Max(x => x.Value);
        public double GetAverage(List<DataRecord> data) => data.Average(x => x.Value);

        public (DateTime, double) GetMostExpensiveHourWindow(List<DataRecord> data)
        {
            if (data == null || data.Count == 0)
                return (DateTime.MinValue, 0);

            data = data.OrderBy(x => x.Time).ToList();
            double maxSum = double.MinValue;
            DateTime maxTime = DateTime.MinValue;

            for (int i = 0; i < data.Count; i++)
            {
                var currentRecord = data[i];
                var timeEnd = currentRecord.Time.AddMinutes(30);
                var recordsInPeriod = data.Where(r => r.Time >= currentRecord.Time && r.Time < timeEnd).ToList();

                if (recordsInPeriod.Count > 1)
                {
                    var maxRecord = recordsInPeriod.OrderByDescending(r => r.Value).First();
                    foreach (var record in recordsInPeriod)
                    {
                        if (record != maxRecord)
                        {
                            _logger.LogError($"Error record: Time={record.Time}, Value={record.Value}");
                        }
                    }

                    if (maxRecord.Value > maxSum)
                    {
                        maxSum = maxRecord.Value;
                        maxTime = maxRecord.Time;
                    }

                    i += recordsInPeriod.Count - 1; // Skip the records in the current hour
                }
                else
                {
                    if (currentRecord.Value > maxSum)
                    {
                        maxSum = currentRecord.Value;
                        maxTime = currentRecord.Time;
                    }
                }
            }

            return (maxTime, maxSum);
        }
    }
}
