using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MyFirstWebApp.Configuration;

namespace MyFirstWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppSettings _appSettings;

        public IndexModel(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string UserName { get; set; } = "ASP.NET Core Developer";
        public AppSettings AppSettings => _appSettings;

        public void OnGet()
        {
            if (_appSettings.Features.ShowDebugInfo)
            {
                ViewData["DebugMessage"] = "Debug mode is enabled!";
            }
        }
    }
}