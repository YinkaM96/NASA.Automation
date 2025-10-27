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

    public async Task FillFormAsync(string firstName, string lastName, string email, string reason)
    {
        // Flexible selectors to handle possible DOM or attribute changes
        var firstNameInput = _page.Locator(
            "input[id='first_name'], input[name='first_name'], input[aria-label*='First Name' i], input[placeholder*='First Name' i]"
        );

        var lastNameInput = _page.Locator(
            "input[id='last_name'], input[name='last_name'], input[aria-label*='Last Name' i], input[placeholder*='Last Name' i]"
        );

        var emailInput = _page.Locator(
            "input[id='email'], input[name='email'], input[type='email'], input[aria-label*='Email' i], input[placeholder*='Email' i]"
        );

        var reasonTextArea = _page.Locator(
            "textarea[id='reason'], textarea[name='reason'], textarea[aria-label*='Reason' i], textarea[placeholder*='Reason' i]"
        );
        if (await firstNameInput.CountAsync() > 0) await firstNameInput.First.FillAsync(firstName);
        if (await lastNameInput.CountAsync() > 0) await lastNameInput.First.FillAsync(lastName);
        if (await emailInput.CountAsync() > 0) await emailInput.First.FillAsync(email);
        if (await reasonTextArea.CountAsync() > 0) await reasonTextArea.First.FillAsync(reason);
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
