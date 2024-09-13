using Microsoft.AspNetCore.Mvc;
using ABP_Task_Zakharov.Services;
using System.IO;
using System.Threading.Tasks;

namespace ABP_Task_Zakharov.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AnalyticsService _analyticsService;

        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // Метод для генерації звіту
        [HttpPost("generate-report")]
        public async Task<IActionResult> GenerateReport()
        {
            // Шлях до файлу для збереження звіту (можна змінити за потреби)
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "analytics_report.txt");

            // Викликаємо метод для генерації звіту
            await _analyticsService.GenerateAnalyticsReportAsync(filePath);

            return Ok($"Звіт було успішно згенеровано та збережено у {filePath}");
        }
    }
}