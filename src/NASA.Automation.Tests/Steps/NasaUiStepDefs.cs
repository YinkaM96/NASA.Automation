using TechTalk.SpecFlow;
using FluentAssertions;
using Microsoft.Playwright;
using NASA.Automation.UI.Pages;

namespace NASA.Automation.Tests.Steps;

[Binding]
public class NasaUiStepDefs
{
    private readonly ScenarioContext _context;

    public NasaUiStepDefs(ScenarioContext context)
    {
        _context = context;
    }

    [When(@"I navigate to the NASA API home page")]
    public async Task WhenINavigateToTheNASAAPIHomePage()
    {
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        await signUpPage.NavigateAsync();

        var page = _context.Get<IPage>("page");
        var title = await page.TitleAsync();
        title.Should().Contain("NASA", "the page title should indicate NASA site");
    }

    [When(@"I start the registration process")]
    public async Task WhenIStartTheRegistrationProcess()
    {
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        await signUpPage.StartRegistrationAsync();
    }

    [When(@"I fill the registration form with:")]
    public async Task WhenIFillTheRegistrationFormWith(Table table)
    {
        var data = table.Rows.ToDictionary(r => r.Keys.First(), r => r.Values.First());
        var fullName = data.TryGetValue("FullName", out var n) ? n : "Test User";
        var email = data.TryGetValue("Email", out var e) ? e : $"testuser+{Guid.NewGuid():N}@example.com";
        var reason = data.TryGetValue("Reason", out var r) ? r : "Automated test";

        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        await signUpPage.FillFormAsync(fullName, email, reason);

        _context["email"] = email;
    }

    [When(@"I submit the registration form")]
    public async Task WhenISubmitTheRegistrationForm()
    {
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        await signUpPage.SubmitAsync();
    }

    [Then(@"I should see a confirmation or success message")]
    public async Task ThenIShouldSeeAConfirmationOrSuccessMessage()
    {
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        var successVisible = await signUpPage.IsConfirmationVisibleAsync();
        successVisible.Should().BeTrue("a confirmation or success message should be visible after submission");
    }
}
