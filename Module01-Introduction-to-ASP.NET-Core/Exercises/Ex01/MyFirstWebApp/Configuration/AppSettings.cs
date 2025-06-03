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