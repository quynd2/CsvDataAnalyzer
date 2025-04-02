using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICsvDataService
    {
        List<DataRecord> GetCachedData();
        Task<(bool isValid, string errorMessage)> UploadCsvFile(IFormFile csvFile);
    }
}
