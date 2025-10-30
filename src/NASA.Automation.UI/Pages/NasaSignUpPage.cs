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
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var generateApiKeyButton = _page.Locator("a[href='#signUp']:has(span:has-text('Generate API Key'))");

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

    public async Task FillFormAsync(string firstName, string lastName, string? email, string reason)
    {
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
        var confirmation = _page.Locator("p:has-text('Your API key for'):has-text('has been e-mailed to you.') ");
        try
        {
            await confirmation.First.WaitForAsync(new() { Timeout = 15000, State = WaitForSelectorState.Visible });
            return true;
        }
        catch { return false; }
    }

    public async Task<string?> GetValidationMessageAsync(string fieldName)
    {
        var selector = fieldName.ToLower() switch
        {
            "empty first name" => "div[id='user_first_name_feedback'].invalid-feedback",
            "empty last name" => "div[id='user_last_name_feedback'].invalid-feedback",
            "empty email" or "invalid email character" => "div[id='user_email_feedback'].invalid-feedback",
            _ => throw new ArgumentException($"Unknown field: {fieldName}")
        };

        var element = _page.Locator(selector);

        if (await element.CountAsync() == 0 || !await element.First.IsVisibleAsync())
            return null;

        var text = (await element.First.InnerTextAsync()).Trim();
        return text;
    }

    public async Task<(bool Visible, string Message)> IsWarningDialogVisibleAsync()
    {
        var dialog = _page.Locator("#alert_modal_message");

        try
        {
            await dialog.WaitForAsync(new() { Timeout = 5000, State = WaitForSelectorState.Visible });

            var text = (await dialog.InnerTextAsync())?.Trim() ?? string.Empty;

            var isMatch = text.Contains("API key signup unexpectedly failed", StringComparison.OrdinalIgnoreCase);

            return (isMatch, text);
        }
        catch
        {
            return (false, string.Empty);
        }
    }
}
