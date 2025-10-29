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
    [BeforeScenario("@UI")]
    public async Task BeforeUiScenarioAsync()
    {
        var pw = await Playwright.CreateAsync();
        var browser = await pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = ConfigManager.Headless,
            Args = new[]
            {
                "--disable-gpu",
                "--no-sandbox",
                "--start-maximized",
                "--window-position=0,0",
                "--window-size=1920,1080"
            }
        });

        // Create a new context without viewport limits (maximizes window)
        var browserContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = null // Setting to null means use full available window size
        });

        var page = await browserContext.NewPageAsync();

        // Optional: explicitly maximize the page window in headed mode
        if (!ConfigManager.Headless)
        {
            await page.EvaluateAsync("window.moveTo(0, 0); window.resizeTo(screen.width, screen.height);");
        }

        var signUpPage = new NasaSignUpPage(page);

        _context["browser"] = browser;
        _context["page"] = page;
        _context["signUpPage"] = signUpPage;

        Console.WriteLine("Browser launched in maximized mode for @ui scenario.");
    }

    // Teardown after UI Scenarios
    [AfterScenario("@UI")]
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

    // Global hooks
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
