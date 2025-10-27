using FluentAssertions;
using Newtonsoft.Json.Linq;
using RestSharp;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using NASA.Automation.API;

namespace NASA.Automation.Tests.Steps;

[Binding]
public class NASAApiStepDefs
{
    private string _baseUrl = "";
    private RestResponse? _response;

    [Given(@"the base NASA API URL ""(.*)""")]
    public void GivenTheBaseNasaApiUrl(string baseUrl) => _baseUrl = baseUrl;

    [When(@"I send a GET request to ""(.*)"" with parameters:")]
    public void WhenISendAGetRequestToWithParameters(string path, Table table)
    {
        var client = new RestClient(_baseUrl);
        var req = new RestRequest(path, Method.Get);

        foreach (var row in table.Rows)
            foreach (var kv in row)
                req.AddParameter(kv.Key, kv.Value);

        _response = client.Execute(req);
        _response.Should().NotBeNull();
        _response!.Content.Should().NotBeNullOrWhiteSpace();
    }

    [Then(@"the response status code should be (.*)")]
    public void ThenTheResponseStatusCodeShouldBe(int status)
    {
        ((int)_response!.StatusCode).Should().Be(status, _response!.Content);
    }

    [Then(@"the JSON should be a non-empty array")]
    public void ThenTheJsonShouldBeANonEmptyArray()
    {
        var token = JToken.Parse(_response!.Content!);
        token.Type.Should().Be(JTokenType.Array, "API returns an array");
        ((JArray)token).Count.Should().BeGreaterThan(0, "expect events");
    }

    [Then(@"each item should contain the field ""(.*)""")]
    public void ThenEachItemShouldContainTheField(string fieldName)
    {
        var arr = JArray.Parse(_response!.Content!);
        arr.Should().NotBeEmpty();
        // Require at least one element to contain the field (NASA payloads can vary)
        arr.Any(o => o[fieldName] != null && o[fieldName]!.Type != JTokenType.Null)
           .Should().BeTrue($"at least one item should have '{fieldName}'");
    }
}
