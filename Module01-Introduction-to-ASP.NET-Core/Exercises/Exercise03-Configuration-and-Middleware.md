# Exercise 3: Working with Configuration and Middleware

## 🎯 Objective
Learn how to work with ASP.NET Core configuration system and create custom middleware components.

## ⏱️ Estimated Time
25-30 minutes

## 📋 Prerequisites
- Completed Exercises 1 and 2
- Understanding of the request pipeline concept

## 📝 Instructions

### Part 1: Configuration Management (10 minutes)

#### Step 1: Create a Configuration Class

1. Create a new folder called `Configuration` in your project
2. Add a new file `Configuration/AppSettings.cs`:

```csharp
namespace MyFirstWebApp.Configuration
{
    public class AppSettings
    {
        public string AppName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public FeatureFlags Features { get; set; } = new();
    }

    public class FeatureFlags
    {
        public bool EnableLogging { get; set; }
        public bool ShowDebugInfo { get; set; }
        public bool EnableApiEndpoints { get; set; }
    }
}
```

#### Step 2: Update appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AppSettings": {
    "AppName": "My First ASP.NET Core App",
    "Version": "1.0.0",
    "Author": "Your Name",
    "Features": {
      "EnableLogging": true,
      "ShowDebugInfo": true,
      "EnableApiEndpoints": false
    }
  }
}
```

#### Step 3: Register Configuration in Program.cs

Add after `var builder = WebApplication.CreateBuilder(args);`:

```csharp
// Configure strongly-typed settings
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));
```

Don't forget to add the using statement:
```csharp
using MyFirstWebApp.Configuration;
```

#### Step 4: Use Configuration in a Page

Update `Pages/Index.cshtml.cs`:

```csharp
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
```

Update `Pages/Index.cshtml` to display configuration:

```html
@page
@model IndexModel
@{
    ViewData["Title"] = "My First ASP.NET Core App";
}

<div class="text-center">
    <h1 class="display-4">@Model.AppSettings.AppName</h1>
    <p>Version: @Model.AppSettings.Version | Author: @Model.AppSettings.Author</p>
    
    @if (ViewData["DebugMessage"] != null)
    {
        <div class="alert alert-warning">
            @ViewData["DebugMessage"]
        </div>
    }
    
    <div class="alert alert-info mt-4">
        <h4>Feature Flags:</h4>
        <ul class="text-start">
            <li>Logging: @(Model.AppSettings.Features.EnableLogging ? "Enabled" : "Disabled")</li>
            <li>Debug Info: @(Model.AppSettings.Features.ShowDebugInfo ? "Enabled" : "Disabled")</li>
            <li>API Endpoints: @(Model.AppSettings.Features.EnableApiEndpoints ? "Enabled" : "Disabled")</li>
        </ul>
    </div>
</div>
```

### Part 2: Custom Middleware (15 minutes)

#### Step 1: Create Request Logging Middleware

1. Create a new folder called `Middleware`
2. Add `Middleware/RequestLoggingMiddleware.cs`:

```csharp
namespace MyFirstWebApp.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request information
            var requestInfo = $"Request: {context.Request.Method} {context.Request.Path} " +
                            $"from {context.Connection.RemoteIpAddress}";
            _logger.LogInformation(requestInfo);

            // Record start time
            var startTime = DateTime.UtcNow;

            // Call the next middleware
            await _next(context);

            // Log response information
            var elapsed = DateTime.UtcNow - startTime;
            var responseInfo = $"Response: {context.Response.StatusCode} in {elapsed.TotalMilliseconds}ms";
            _logger.LogInformation(responseInfo);
        }
    }

    // Extension method to make it easy to use
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
```

#### Step 2: Create Response Headers Middleware

Add `Middleware/SecurityHeadersMiddleware.cs`:

```csharp
namespace MyFirstWebApp.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("X-Powered-By", "ASP.NET Core");

            await _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
```

#### Step 3: Register Middleware in Program.cs

Update your Program.cs:

```csharp
using MyFirstWebApp.Configuration;
using MyFirstWebApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

// Get app settings to check feature flags
var appSettings = app.Configuration.GetSection("AppSettings").Get<AppSettings>();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Add custom middleware
app.UseSecurityHeaders();

if (appSettings?.Features.EnableLogging == true)
{
    app.UseRequestLogging();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
```

### Part 3: Test Your Implementation (5 minutes)

1. Run your application with `dotnet run`

2. Open browser developer tools (F12) and check:
   - Network tab to see response headers
   - Console to see if logging is working

3. Make several requests and check the console output for logging

4. Try changing feature flags in appsettings.json:
   - Set `EnableLogging` to false
   - Restart the app and verify logging stops

## ✅ Success Criteria

- [ ] Configuration class created and working
- [ ] Settings displayed on the home page
- [ ] Request logging middleware working (when enabled)
- [ ] Security headers middleware adding headers
- [ ] Feature flags controlling middleware behavior

## 🚀 Bonus Challenges

1. **Environment-Specific Features**:
   - Add different feature flags for Development vs Production
   - Use `app.Environment.IsDevelopment()` to conditionally apply middleware

2. **Request/Response Modification**:
   - Create middleware that adds a custom header with request processing time
   - Create middleware that modifies response content (e.g., adds a footer)

3. **Advanced Configuration**:
   - Add configuration validation
   - Create a configuration page that shows all settings (only in Development)
   - Implement configuration hot-reload

## 🤔 Reflection Questions

1. What's the difference between middleware and services?
2. Why is middleware order important?
3. How would you share data between middleware components?
4. What are the benefits of strongly-typed configuration?

## 📊 Middleware Pipeline Diagram

```
Request →
    ↓
[SecurityHeaders] → Adds security headers
    ↓
[RequestLogging] → Logs request start
    ↓
[HttpsRedirection] → Redirects to HTTPS
    ↓
[StaticFiles] → Serves static files if match
    ↓
[Routing] → Determines endpoint
    ↓
[Authorization] → Checks permissions
    ↓
[Endpoint] → Executes page/controller
    ↓
Response ←
```

## 🆘 Troubleshooting

**Issue**: Middleware not executing
**Solution**: Check registration order and feature flags

**Issue**: Configuration not binding
**Solution**: Ensure JSON structure matches class structure

**Issue**: Logger not working
**Solution**: Check log level in appsettings.json

## 📚 Resources
- [ASP.NET Core Middleware](https://docs.microsoft.com/aspnet/core/fundamentals/middleware/)
- [Configuration in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/)
- [Options pattern](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/options)

---

**Congratulations!** You've completed all Module 1 exercises. You now understand the basics of ASP.NET Core application structure, configuration, and middleware.

**Ready for Module 2?** [Implementing ASP.NET Core with React.js →](../../Module02-ASP.NET-Core-with-React/README.md)