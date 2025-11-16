using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WifiPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class LogsController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LogsController> _logger;

    public LogsController(IWebHostEnvironment environment, ILogger<LogsController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] string? date = null)
    {
        _logger.LogInformation("Запрос логов за дату {Date}", date ?? "текущую");

        try
        {
            var logsDirectory = Path.Combine(_environment.ContentRootPath, "Logs");

            if (!Directory.Exists(logsDirectory))
            {
                return NotFound("Директория с логами не найдена");
            }

            string logFilePath;
            if (string.IsNullOrEmpty(date))
            {
                var logFiles = Directory.GetFiles(logsDirectory, "wifi-portal-*.txt")
                    .OrderByDescending(f => f)
                    .FirstOrDefault();

                if (logFiles == null) return NotFound("Лог-файлы не найдены");
                logFilePath = logFiles;
            }
            else
            {
                logFilePath = Path.Combine(logsDirectory, $"wifi-portal-{date}.txt");
                if (!System.IO.File.Exists(logFilePath))
                    return NotFound($"Лог-файл за дату {date} не найден");
            }

            var fileName = Path.GetFileName(logFilePath);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(logFilePath);

            _logger.LogInformation("Логи успешно получены из файла {FilePath}", logFilePath);

            return File(fileBytes, "text/plain", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении логов");
            return StatusCode(500, "Ошибка при чтении логов");
        }
    }

    [HttpGet("dates")]
    public IActionResult GetAvailableLogDates()
    {
        _logger.LogInformation("Запрос доступных дат логов");

        try
        {
            var logsDirectory = Path.Combine(_environment.ContentRootPath, "Logs");

            if (!Directory.Exists(logsDirectory))
            {
                return Ok(Array.Empty<string>());
            }

            var logFiles = Directory.GetFiles(logsDirectory, "wifi-portal-*.txt")
                .Select(Path.GetFileName)
                .Where(name => name != null)
                .Select(name => name!.Replace("wifi-portal-", "").Replace(".txt", ""))
                .OrderByDescending(date => date)
                .ToList();

            _logger.LogInformation("Найдено {Count} лог-файлов", logFiles.Count);
            return Ok(logFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка дат логов");
            return StatusCode(500, "Ошибка при получении списка дат логов");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> ClearOldLogs()
    {
        _logger.LogInformation("Очистка старых логов");

        try
        {
            var logsDirectory = Path.Combine(_environment.ContentRootPath, "Logs");

            if (!Directory.Exists(logsDirectory))
            {
                return Ok("Директория с логами не существует");
            }

            var logFiles = Directory.GetFiles(logsDirectory, "wifi-portal-*.txt")
                .OrderByDescending(f => f)
                .Skip(5)
                .ToList();

            await Task.Run(() =>
            {
                foreach (var file in logFiles)
                {
                    System.IO.File.Delete(file);
                    _logger.LogInformation("Удален лог-файл {FilePath}", file);
                }
            });

            _logger.LogInformation("Удалено {Count} старых лог-файлов", logFiles.Count);
            return Ok($"Удалено {logFiles.Count} старых лог-файлов");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при очистке логов");
            return StatusCode(500, "Ошибка при очистке логов");
        }
    }
}