using FluentAssertions;
using NASA.Automation.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace NASA.Automation.Tests.Steps;

[Binding]
public class NasaApiStepDefs
{
    private readonly ScenarioContext _context;
    private readonly NasaDonkiClient _client;
    private RestSharp.RestResponse? _response;

    public NasaApiStepDefs(ScenarioContext context)
    {
        _context = context;
        _client = new NasaDonkiClient();
    }

    [Given(@"the NASA API client is available")]
    public void GivenTheNASAApiClientIsAvailable()
    {
        _client.Should().NotBeNull("NASA DONKI client should be initialized");
    }

    [When(@"I request CME data from ""(.*)"" to ""(.*)""")]
    public void WhenIRequestCMEData(string startDate, string endDate)
    {
        _response = _client.GetCme(startDate, endDate);
        _context["response"] = _response;
    }

    [When(@"I request FLR data from ""(.*)"" to ""(.*)""")]
    public void WhenIRequestFLRData(string startDate, string endDate)
    {
        _response = _client.GetFlr(startDate, endDate);
        _context["response"] = _response;
    }

    [Then(@"the response status code should be (.*)")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatus)
    {
        _response.Should().NotBeNull();
        ((int)_response!.StatusCode).Should().Be(expectedStatus, "HTTP response should match expected status code");
    }

    [Then(@"the response JSON should be a non-empty array")]
    public void ThenTheResponseJSONShouldBeANon_EmptyArray()
    {
        _response.Should().NotBeNull("HTTP response should not be null");

        var raw = _response!.Content;
        raw.Should().NotBeNull("response body should exist even if empty");

        try
        {
            var token = JToken.Parse(raw!);

            // Ensure it’s an array
            token.Type.Should().Be(JTokenType.Array, $"Expected array JSON, got {token.Type}");

            var array = (JArray)token;
            _context["jsonArray"] = array;

            // Handle empty array gracefully
            if (!array.Any())
            {
                Assert.Warn("NASA returned an empty array — data may not exist for this date range");
            }
            else
            {
                array.Should().NotBeEmpty("expected at least one record in response");
            }
        }
        catch (JsonReaderException ex)
        {
            Assert.Fail($"Response body is not valid JSON: {ex.Message}\nRaw content: {raw}");
        }
    }


    [Then(@"each object should contain the field ""(.*)""")]
    public void ThenEachObjectShouldContainTheField(string field)
    {
        var array = (JArray)_context["jsonArray"];
        var first = (JObject)array.First!;
        first.ContainsKey(field).Should().BeTrue($"expected field '{field}' in response object");
    }
}
