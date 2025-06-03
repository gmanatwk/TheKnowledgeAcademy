using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _configuration;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public string ApplicationName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string ServerTime { get; set; } = string.Empty;

    public void OnGet()
    {
        _logger.LogInformation("Index page visited at {Time}", DateTime.UtcNow);

        // Get application information from configuration
        ApplicationName = _configuration["ApplicationSettings:ApplicationName"] ?? "Unknown";
        Version = _configuration["ApplicationSettings:Version"] ?? "Unknown";
        Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        ServerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
