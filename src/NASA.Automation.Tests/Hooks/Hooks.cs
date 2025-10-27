using TechTalk.SpecFlow;
using Microsoft.Playwright;
using NASA.Automation.UI.Pages;
using NASA.Automation.Core;

namespace NASA.Automation.Tests.Hooks;

[Binding]
public class Hooks
{
    private readonly ScenarioContext _context;

    public Hooks(ScenarioContext context)
    {
        _context = context;
    }

    // Setup before UI Scenarios
    [BeforeScenario("@ui")]
    public async Task BeforeUiScenarioAsync()
    {
        var pw = await Playwright.CreateAsync();
        var browser = await pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = ConfigManager.Headless,
            Args = new[] { "--disable-gpu", "--no-sandbox" }
        });

        var browserContext = await browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1280, Height = 900 }
        });

        var page = await browserContext.NewPageAsync();
        var signUpPage = new NasaSignUpPage(page);

        _context["browser"] = browser;
        _context["page"] = page;
        _context["signUpPage"] = signUpPage;

        Console.WriteLine("🌐 Browser launched and Playwright context initialized for @ui scenario.");
    }

    // Teardown after UI Scenarios
    [AfterScenario("@ui")]
    public async Task AfterUiScenarioAsync()
    {
        try
        {
            if (_context.ContainsKey("browser"))
            {
                var browser = _context.Get<IBrowser>("browser");
                await browser.CloseAsync();
                Console.WriteLine("Browser closed successfully after UI scenario.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Browser teardown failed: {ex.Message}");
        }
    }

    // Optional: global hooks
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Console.WriteLine("Starting NASA Automation test suite...");
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Console.WriteLine("All tests completed. Cleaning up environment...");
    }
}
