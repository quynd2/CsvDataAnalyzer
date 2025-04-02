using Xunit;
using Infrastructure.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using Application.Interfaces;
using Moq;

public class DataAnalyzerTests
{
    private readonly DataAnalyzer _dataAnalyzer;

    public DataAnalyzerTests()
    {
        _dataAnalyzer = new DataAnalyzer();
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

    [Fact]
    public void GetMostExpensiveHourWindow_ShouldReturnCorrectWindow()
    {
        var data = new List<DataRecord>
        {
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 10:00", "dd/MM/yyyy HH:mm", null), Value = 10 },
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 10:30", "dd/MM/yyyy HH:mm", null), Value = 30 },
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 11:00", "dd/MM/yyyy HH:mm", null), Value = 20 },
            new DataRecord { Time = DateTime.ParseExact("01/03/2025 11:15", "dd/MM/yyyy HH:mm", null), Value = 25 }
        };

        var result = _dataAnalyzer.GetMostExpensiveHourWindow(data);

        Assert.Equal(DateTime.ParseExact("01/03/2025 10:30", "dd/MM/yyyy HH:mm", null), result.Item1);
        Assert.Equal(50, result.Item2);
    }
}