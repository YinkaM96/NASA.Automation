using Microsoft.Playwright;
using NASA.Automation.Core;

namespace NASA.Automation.UI.Pages;

public class NasaSignUpPage
{
    private readonly IPage _page;

    public NasaSignUpPage(IPage page) => _page = page;

    public async Task NavigateAsync()
    {
        await _page.GotoAsync(ConfigManager.NasaApiHome, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task StartRegistrationAsync()
    {
        var cta = _page.Locator("a:has-text('Sign Up'), a:has-text('Get Started'), a:has-text('Register')");
        if (await cta.CountAsync() > 0)
            await cta.First.ClickAsync(new LocatorClickOptions { Timeout = 10000 });
    }

    public async Task FillFormAsync(string fullName, string email, string reason)
    {
        // Flexible selectors to handle content changes
        var fullNameInput = _page.Locator("input[name='fullName'], input#fullName, input[aria-label*='Full Name' i]");
        var emailInput = _page.Locator("input[type='email'], input[name='email'], input#email, input[aria-label*='Email' i]");
        var reasonArea = _page.Locator("textarea[name='reason'], textarea#reason, textarea[aria-label*='Reason' i]");

        if (await fullNameInput.CountAsync() > 0) await fullNameInput.First.FillAsync(fullName);
        if (await emailInput.CountAsync() > 0) await emailInput.First.FillAsync(email);
        if (await reasonArea.CountAsync() > 0) await reasonArea.First.FillAsync(reason);
    }

    public async Task SubmitAsync()
    {
        var submit = _page.Locator("button[type='submit'], button:has-text('Sign Up'), button:has-text('Register'), input[type='submit']");
        if (await submit.CountAsync() > 0)
            await submit.First.ClickAsync();
    }

    public async Task<bool> IsConfirmationVisibleAsync()
    {
        var confirmation = _page.Locator(":text-matches('Thank you|API key|Check your email|Success', 'i')");
        try
        {
            await confirmation.First.WaitForAsync(new() { Timeout = 15000, State = WaitForSelectorState.Visible });
            return true;
        }
        catch { return false; }
    }
}
