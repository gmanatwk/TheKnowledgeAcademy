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

app.UseProcessingTimeMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();