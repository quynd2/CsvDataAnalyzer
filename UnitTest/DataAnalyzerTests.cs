using Xunit;
using Infrastructure.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using Application.Interfaces;
using Moq;
using Microsoft.Extensions.Logging;

public class DataAnalyzerTests
{
    private readonly DataAnalyzer _dataAnalyzer;
    private readonly Mock<ILogger<DataAnalyzer>> _logger;

    public DataAnalyzerTests()
    {
        _logger = new Mock<ILogger<DataAnalyzer>>();
        _dataAnalyzer = new DataAnalyzer(_logger.Object);
    }

    [Fact]
    public void GetMinimum_ShouldReturnCorrectValue()
    {
        var data = new List<DataRecord>
        {
            new DataRecord { Value = 10 },
            new DataRecord { Value = 5 },
            new DataRecord { Value = 15 }
        };

        var result = _dataAnalyzer.GetMinimum(data);

        Assert.Equal(5, result);
    }

    [Fact]
    public void GetMaximum_ShouldReturnCorrectValue()
    {
        var data = new List<DataRecord>
        {
            new DataRecord { Value = 10 },
            new DataRecord { Value = 5 },
            new DataRecord { Value = 15 }
        };

        var result = _dataAnalyzer.GetMaximum(data);

        Assert.Equal(15, result);
    }

    [Fact]
    public void GetAverage_ShouldReturnCorrectValue()
    {
        var data = new List<DataRecord>
        {
            new DataRecord { Value = 10 },
            new DataRecord { Value = 5 },
            new DataRecord { Value = 15 }
        };

        var result = _dataAnalyzer.GetAverage(data);

        Assert.Equal(10, result);
    }
}