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

    [StepDefinition(@"I navigate to the NASA API home page")]
    public async Task WhenINavigateToTheNASAAPIHomePage()
    {
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        await signUpPage.NavigateAsync();

        var pageLoaded = await signUpPage.ValidatePageLoadedAsync();
        pageLoaded.Should().BeTrue("the NASA API homepage should load successfully before continuing.");
    }

    [When(@"I fill the registration form with:")]
    public async Task WhenIFillTheRegistrationFormWith(Table table)
    {
        var data = table.Rows
            .SelectMany(r => r)
            .ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);

        // Extract values, allowing empty strings for validation scenarios
        var firstName = data.TryGetValue("FirstName", out var fn) ? fn?.Trim() ?? string.Empty : string.Empty;
        var lastName = data.TryGetValue("LastName", out var ln) ? ln?.Trim() ?? string.Empty : string.Empty;
        var email = data.TryGetValue("Email", out var e) ? e?.Trim() ?? string.Empty : string.Empty;
        var reason = data.TryGetValue("Reason", out var rs) ? rs?.Trim() ?? string.Empty : string.Empty;

        // If the email contains {guid}, generate one only if not already stored
        if (email.Contains("{guid}", StringComparison.OrdinalIgnoreCase))
        {
            if (!_context.ContainsKey("email") || string.IsNullOrWhiteSpace(_context["email"]?.ToString()))
            {
                email = email.Replace("{guid}", Guid.NewGuid().ToString("N"));
                _context["email"] = email;
            }
            else
            {
                // Reuse the previously stored email
                email = _context["email"].ToString();
            }
        }
        else if (string.IsNullOrWhiteSpace(email) && _context.ContainsKey("email"))
        {
            // Reuse existing email if new one is empty
            email = _context["email"].ToString();
        }
        else
        {
            // Otherwise, set email for the first time if not already stored
            if (!_context.ContainsKey("email") || string.IsNullOrWhiteSpace(_context["email"]?.ToString()))
            {
                _context["email"] = email;
            }
        }

        
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        await signUpPage.FillFormAsync(firstName, lastName, email!, reason);

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

    [Then(@"I should see a warning dialog for restricted email")]
    public async Task ThenIShouldSeeAWarningDialog()
    {
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");
        var (visible, message) = await signUpPage.IsWarningDialogVisibleAsync();

        visible.Should().BeTrue("a warning dialog should appear for restricted email like test@example.com");
        message.Should().Contain("API key signup unexpectedly failed");

        Console.WriteLine($"Warning dialog detected: {message}");
    }

    [Then(@"I should see the correct warning message for ""(.*)""")]
    public async Task ThenIShouldSeeAWarningMessage(string field)
    {
        var signUpPage = _context.Get<NasaSignUpPage>("signUpPage");

        var actualMessage = await signUpPage.GetValidationMessageAsync(field);
        actualMessage.Should().NotBeNullOrEmpty($"a warning message should be visible for the '{field}' field");

        // Expected messages per field
        var expectedMessage = field.ToLower() switch
        {
            "empty first name" => "Fill out this field.",
            "empty last name" => "Fill out this field.",
            "empty email" or "invalid email character" => "Enter an email address.",
            _ => throw new ArgumentException($"Unknown field: {field}")
        };

        actualMessage.Should().Be(expectedMessage,
            $"the '{field}' field should display '{expectedMessage}' but got '{actualMessage}'");

    }
}
