namespace NASA.Automation.Core;

public static class ConfigManager
{
    public static string BaseUrl => "https://api.nasa.gov";
    public static string ApiKey => "DEMO_KEY";
    
    // UI
    public static string NasaApiHome => "https://api.nasa.gov";

    // Helpers
    public static bool Headless =>
        (Environment.GetEnvironmentVariable("HEADLESS") ?? "1") != "0";
}

