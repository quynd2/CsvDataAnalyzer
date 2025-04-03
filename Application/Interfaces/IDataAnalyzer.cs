using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDataAnalyzer
    {
        double GetMinimum(List<DataRecord> data);
        double GetMaximum(List<DataRecord> data);
        double GetAverage(List<DataRecord> data);
        (DateTime startTime, DateTime endTime, double value) GetMostExpensiveWindow(List<DataRecord> data);
    }
}
