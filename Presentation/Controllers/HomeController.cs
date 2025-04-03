using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Application.Interfaces;
using Domain.Constants;

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
            if (originalData == null || originalData.Count == 0)
                return View(new List<DataRecord>());

            var from = fromDate ?? DateTime.MinValue;
            var to = toDate ?? DateTime.MaxValue;
            var data = originalData.Where(x => x.Time >= from && x.Time <= to).ToList();
            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            if (pagedData == null || pagedData.Count == 0)
                return View(new List<DataRecord>());

            ViewBag.FromDate = fromDate?.ToString(Constant.DATE_TIME_FORMAT_DISPLAY) ?? data.Min(x => x.Time).ToString(Constant.DATE_TIME_FORMAT_DISPLAY);
            ViewBag.ToDate = toDate?.ToString(Constant.DATE_TIME_FORMAT_DISPLAY) ?? data.Max(x => x.Time).ToString(Constant.DATE_TIME_FORMAT_DISPLAY);
            ViewBag.Minimum = _dataAnalyzer.GetMinimum(data).ToString("F2");
            ViewBag.Maximum = _dataAnalyzer.GetMaximum(data).ToString("F2");
            ViewBag.Average = _dataAnalyzer.GetAverage(data).ToString("F2");
            var expensiveWindow = _dataAnalyzer.GetMostExpensiveWindow(data);
            ViewBag.MostExpensiveWindow = string.Format("{0} - {1} : {2}", expensiveWindow.startTime, expensiveWindow.endTime, expensiveWindow.value.ToString("F2"));
            ViewBag.TotalPages = (int)Math.Ceiling((double)data.Count / pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedData);
        }

        [HttpGet]
        public IActionResult GetChartData(DateTime? fromDate, DateTime? toDate)
        {
            var originalData = _csvDataService.GetCachedData();
            if (originalData == null || originalData.Count == 0)
                return Json(null);
            var from = fromDate ?? DateTime.MinValue;
            var to = toDate ?? DateTime.MaxValue;
            var data = originalData.Where(x => x.Time >= from && x.Time <= to).ToList();
            if (originalData == null || originalData.Count == 0)
                return Json(null);
            var labels = data.Select(d => d.Time.ToString(Constant.DATE_TIME_FORMAT_DISPLAY)).ToList();
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
