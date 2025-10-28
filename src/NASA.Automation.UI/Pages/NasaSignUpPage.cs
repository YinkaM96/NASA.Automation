using Microsoft.Playwright;
using NASA.Automation.Core;
using NASA.Automation.UI.Utils;

namespace NASA.Automation.UI.Pages;

public class NasaSignUpPage
{
    private readonly IPage _page;

    public NasaSignUpPage(IPage page) => _page = page;

    public async Task NavigateAsync()
    {
        await _page.GotoAsync(ConfigManager.NasaApiHome, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task<bool> ValidatePageLoadedAsync()
    {
        Console.WriteLine("Waiting for NASA Sign-Up page to load...");

        try
        {
            // Wait for the page and network to stabilize
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Primary element indicating the page is ready
            var generateApiKeyButton = _page.Locator("a[href='#signUp']:has(span:has-text('Generate API Key'))");

            // Wait up to 15 seconds for it to become visible
            await generateApiKeyButton.First.WaitForAsync(new LocatorWaitForOptions
            {
                Timeout = 15000,
                State = WaitForSelectorState.Visible
            });

            Console.WriteLine("NASA Sign-Up page loaded successfully.");
            return true;
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Timed out waiting for NASA Sign-Up page to load.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to validate page load: {ex.Message}");
            return false;
        }
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
            "input[id='user_first_name'], input[name='user[first_name]']"
        );

        var lastNameInput = _page.Locator(
            "input[id='user_last_name'], input[name='user[last_name]']"
        );

        var emailInput = _page.Locator(
            "input[id='user_email'], input[name='user[email]']"
        );

        var reasonTextArea = _page.Locator(
            "textarea[id='user_use_description'], textarea[name='user[use_description]']"
        );

        await ElementUtils.SafeFillAsync(firstNameInput, firstName, "First Name");
        await ElementUtils.SafeFillAsync(lastNameInput, lastName, "Last Name");
        await ElementUtils.SafeFillAsync(emailInput, email, "Email");
        await ElementUtils.SafeFillAsync(reasonTextArea, reason, "Reason");
    }

    public async Task SubmitAsync()
    {
        var submit = _page.Locator("button[type='submit']:has-text('Sign Up')");
        if (await submit.CountAsync() > 0)
            await ElementUtils.SafeClickAsync(submit, "Sign Up");
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
