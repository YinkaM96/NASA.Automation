using TechTalk.SpecFlow;
using NASA.Automation.UI;
using NASA.Automation.UI.Pages;
using FluentAssertions;

namespace NASA.Automation.Tests.Steps;

[Binding]
public class NasaUiStepDefs
{
    private static Microsoft.Playwright.IPage? _page;
    private static NasaSignUpPage? _sign;

    [Given(@"I open the NASA APIs homepage")]
    public async Task GivenIOpenTheNasaApisHomepage()
    {
        _page = await PlaywrightSetup.LaunchAsync();
        _sign = new NasaSignUpPage(_page);
        await _sign.NavigateAsync();
    }

    [When(@"I start registration")]
    public async Task WhenIStartRegistration() => await _sign!.StartRegistrationAsync();

    [When(@"I fill the registration form with:")]
    public async Task WhenIFillTheRegistrationFormWith(Table table)
    {
        var dict = table.Rows.ToDictionary(r => r.Keys.First(), r => r.Values.First());
        var fullName = dict.TryGetValue("FullName", out var n) ? n : "Test User";
        var email = dict.TryGetValue("Email", out var e) ? e : $"testuser+{Guid.NewGuid():N}@example.com";
        var reason = dict.TryGetValue("Reason", out var s) ? s : "Automated test";
        await _sign!.FillFormAsync(fullName, email, reason);
    }

    [When(@"I submit the registration form")]
    public async Task WhenISubmitTheRegistrationForm() => await _sign!.SubmitAsync();

    [Then(@"I should see a confirmation message")]
    public async Task ThenIShouldSeeAConfirmationMessage()
    {
        var ok = await _sign!.IsConfirmationVisibleAsync();
        ok.Should().BeTrue("the UI should show confirmation after successful submission");
    }
}
