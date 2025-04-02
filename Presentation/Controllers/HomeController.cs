using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Application.Interfaces;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICsvDataService _csvDataService;
        private readonly IDataAnalyzer _dataAnalyzer;

        public HomeController(ICsvDataService csvDataService, IDataAnalyzer dataAnalyzer)
        {
            _csvDataService = csvDataService;
            _dataAnalyzer = dataAnalyzer;
        }

        public IActionResult Index(DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var originalData = _csvDataService.GetCachedData();
            var from = fromDate ?? DateTime.MinValue;
            var to = toDate ?? DateTime.MaxValue;
            var data = originalData.Where(x => x.Time >= from && x.Time <= to).ToList();
            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-ddThh:mm") ?? data.Min(x => x.Time).ToString("yyyy-MM-ddThh:mm");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-ddThh:mm") ?? data.Max(x => x.Time).ToString("yyyy-MM-ddThh:mm");
            ViewBag.Minimum = _dataAnalyzer.GetMinimum(originalData).ToString("F2");
            ViewBag.Maximum = _dataAnalyzer.GetMaximum(originalData).ToString("F2");
            ViewBag.Average = _dataAnalyzer.GetAverage(originalData).ToString("F2");
            ViewBag.MostExpensiveWindow = _dataAnalyzer.GetMostExpensiveHourWindow(originalData);
            ViewBag.TotalPages = (int)Math.Ceiling((double)data.Count / pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedData);
        }

        [HttpGet]
        public IActionResult GetChartData(DateTime? fromDate, DateTime? toDate)
        {
            var originalData = _csvDataService.GetCachedData();
            var from = fromDate ?? DateTime.MinValue;
            var to = toDate ?? DateTime.MaxValue;
            var data = originalData.Where(x => x.Time >= from && x.Time <= to).ToList();
            var labels = data.Select(d => d.Time.ToString("yyyy-MM-dd HH:mm")).ToList();
            var values = data.Select(d => d.Value).ToList();
            var minimum = originalData.Min(x => x.Value);
            var maximum = originalData.Max(x => x.Value);
            var average = originalData.Average(x => x.Value);

            return Json(new { labels, values, minimum, maximum, average });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile csvFile)
        {
            var (isValid, errorMessage) = await _csvDataService.UploadCsvFile(csvFile);
            if (!isValid)
            {
                return Json(new { success = false, message = errorMessage });
            }

            return Json(new { success = true });
        }
    }
}
