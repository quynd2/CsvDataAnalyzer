using Application.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Entities;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Infrastructure.Services
{
    public class CsvDataService : ICsvDataService
    {
        private List<DataRecord> _cachedData;
        private readonly object _lock = new();
        private string _filePath = "sampleSheet2.csv";

        public List<DataRecord> GetCachedData()
        {
            if (_cachedData == null)
            {
                lock (_lock)
                {
                    if (_cachedData == null)
                    {
                        _cachedData = ReadCsvData(_filePath);
                    }
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

            using var reader = new StreamReader(csvFile.OpenReadStream());
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
                var filePath = Path.Combine(Path.GetTempPath(), csvFile.FileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await csvFile.CopyToAsync(fileStream);

                // Update the file path and cached data
                UpdateFilePath(filePath);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Error reading CSV file: {ex.Message}");
            }
        }

        public void UpdateFilePath(string filePath)
        {
            lock (_lock)
            {
                _filePath = filePath;
                //_cachedData = ReadCsvData(_filePath);
            }
        }

        private List<DataRecord> ReadCsvData(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null, // Ignore bad data
                MissingFieldFound = null, // Ignore missing fields
            });

            csv.Context.RegisterClassMap<DataRecordMap>();
            return csv.GetRecords<DataRecord>().OrderBy(x => x.Time).ToList();
        }
    }
}
