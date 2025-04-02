using Xunit;
using Moq;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Services;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public class CsvDataServiceTests
{
    private readonly CsvDataService _csvDataService;
    private readonly Mock<ICsvDataService> _csvDataServiceMock;

    public CsvDataServiceTests()
    {
        _csvDataServiceMock = new Mock<ICsvDataService>();
        _csvDataService = new CsvDataService();
    }

    [Fact]
    public void GetCachedData_ShouldReturnCorrectData()
    {
        var filePath = Path.GetTempFileName();
        var data = new List<DataRecord>
        {
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 10:00", "dd/MM/yyyy HH:mm", null), Value = 10 },
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 10:30", "dd/MM/yyyy HH:mm", null), Value = 20 },
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 11:00", "dd/MM/yyyy HH:mm", null), Value = 30 }
        };

        // Write data to temporary CSV file
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(data);
        }

        // Clear the cache using reflection
        var cachedDataField = typeof(CsvDataService).GetField("_cachedData", BindingFlags.NonPublic | BindingFlags.Instance);
        cachedDataField.SetValue(_csvDataService, null);

        // Set the file path using reflection
        var filePathField = typeof(CsvDataService).GetField("_filePath", BindingFlags.NonPublic | BindingFlags.Instance);
        filePathField.SetValue(_csvDataService, filePath);

        var result = _csvDataService.GetCachedData();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(10, result[0].Value);
        Assert.Equal(20, result[1].Value);
        Assert.Equal(30, result[2].Value);

        // Clean up temporary file
        File.Delete(filePath);
    }

    [Fact]
    public void GetCachedData_ShouldReadFromFile_WhenCacheIsEmpty()
    {
        var filePath = Path.GetTempFileName();
        var data = new List<DataRecord>
        {
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 10:00", "dd/MM/yyyy HH:mm", null), Value = 10 },
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 10:30", "dd/MM/yyyy HH:mm", null), Value = 20 },
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 11:00", "dd/MM/yyyy HH:mm", null), Value = 30 }
        };

        // Write data to temporary CSV file
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(data);
        }

        // Clear the cache using reflection
        var cachedDataField = typeof(CsvDataService).GetField("_cachedData", BindingFlags.NonPublic | BindingFlags.Instance);
        cachedDataField.SetValue(_csvDataService, null);

        // Set the file path using reflection
        var filePathField = typeof(CsvDataService).GetField("_filePath", BindingFlags.NonPublic | BindingFlags.Instance);
        filePathField.SetValue(_csvDataService, filePath);

        var result = _csvDataService.GetCachedData();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(10, result[0].Value);
        Assert.Equal(20, result[1].Value);
        Assert.Equal(30, result[2].Value);

        // Clean up temporary file
        File.Delete(filePath);
    }

    [Fact]
    public void GetCachedData_ShouldReturnCachedData_WhenCacheIsNotEmpty()
    {
        var cachedData = new List<DataRecord>
        {
            new DataRecord { Value = 10 },
            new DataRecord { Value = 20 },
            new DataRecord { Value = 30 }
        };

        // Set the cache using reflection
        var cachedDataField = typeof(CsvDataService).GetField("_cachedData", BindingFlags.NonPublic | BindingFlags.Instance);
        cachedDataField.SetValue(_csvDataService, cachedData);

        var result = _csvDataService.GetCachedData();

        Assert.Equal(3, result.Count);
        Assert.Equal(10, result[0].Value);
        Assert.Equal(20, result[1].Value);
        Assert.Equal(30, result[2].Value);
    }
}