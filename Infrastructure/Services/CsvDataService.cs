using Application.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Entities;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Infrastructure.Services
{
    public class CsvDataService : ICsvDataService
    {
        private readonly ILogger<CsvDataService> _logger;
        private List<DataRecord> _cachedData;
        private readonly object _lock = new();
        private readonly string _filePathStorage = "wwwroot/filePath.txt";

        public CsvDataService(ILogger<CsvDataService> logger)
        {
            _logger = logger;
            var filePath = ReadFilePath();
            _cachedData = ReadCsvData(filePath);
        }

        public List<DataRecord> GetCachedData()
        {
            if (_cachedData == null || _cachedData.Count == 0)
            {
                lock (_lock)
                {
                    var filePath = ReadFilePath();
                    _cachedData = ReadCsvData(filePath);
                }
            }
            return _cachedData;
        }

        public async Task<(bool isValid, string errorMessage)> UploadCsvFile(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                return (false, "Please upload a CSV file.");
            }

            if (!csvFile.FileName.EndsWith(".csv"))
            {
                return (false, "Invalid file format. Please upload a CSV file.");
            }

            var filePath = Path.Combine(Path.GetTempPath(), csvFile.FileName);
            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = null, // Ignore bad data
                    MissingFieldFound = null, // Ignore missing fields
                });

                csv.Context.RegisterClassMap<DataRecordMap>();

                try
                {
                    var records = csv.GetRecords<DataRecord>().ToList();
                    if (records.Any(r => r.Time == default || r.Value == default))
                    {
                        return (false, "Invalid data format. Ensure the CSV has 'Date' and 'Value' columns.");
                    }

                    // Save the file to a temporary location
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await csvFile.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Error reading CSV file: {ex.Message}");
                }
            }

            // Update the file path and cached data
            UpdateFilePath(filePath);
            return (true, string.Empty);
        }

        public void UpdateFilePath(string filePath)
        {
            lock (_lock)
            {
                WriteFilePath(filePath);
                _cachedData = ReadCsvData(filePath);
            }
        }

        private string ReadFilePath()
        {
            if (!File.Exists(_filePathStorage))
            {
                return string.Empty;
            }

            return File.ReadAllText(_filePathStorage);
        }

        private void WriteFilePath(string filePath)
        {
            if (File.Exists(_filePathStorage))
            {
                File.Delete(_filePathStorage);
            }
            File.WriteAllText(_filePathStorage, filePath);
        }

        private List<DataRecord> ReadCsvData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File not found: {filePath}");
                return new List<DataRecord>();
            }

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null, // Ignore bad data
                MissingFieldFound = null, // Ignore missing fields
            });

            csv.Context.RegisterClassMap<DataRecordMap>();
            var records = csv.GetRecords<DataRecord>().ToList();

            var uniqueRecords = new Dictionary<DateTime, DataRecord>();
            foreach (var record in records)
            {
                if (uniqueRecords.ContainsKey(record.Time))
                {
                    _logger.LogError($"Duplicate record found: Time={record.Time}, Value={record.Value}");
                }
                uniqueRecords[record.Time] = record;
            }

            return uniqueRecords.Values.OrderBy(x => x.Time).ToList();
        }
    }
}
