using Microsoft.Playwright;
using NASA.Automation.Core;
using static System.Net.Mime.MediaTypeNames;

namespace NASA.Automation.UI;

public static class PlaywrightSetup
{
    public static async Task<IPage> LaunchAsync()
    {
        var pw = await Playwright.CreateAsync();
        var browser = await pw.Chromium.LaunchAsync(new() { Headless = ConfigManager.Headless, Args = new[] { "--disable-gpu" } });
        var context = await browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1280, Height = 900 }
        });
        return await context.NewPageAsync();
    }
}
