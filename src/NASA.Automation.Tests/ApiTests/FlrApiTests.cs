using FluentAssertions;
using Newtonsoft.Json.Linq;
using NASA.Automation.API;
using NUnit.Framework;

namespace NASA.Automation.Tests.ApiTests;

[TestFixture]
public class FlrApiTests
{
    private NasaDonkiClient _client = null!;

    [SetUp]
    public void Setup() => _client = new NasaDonkiClient();

    [Test, Category("API"), Category("FLR")]
    public void GetFlr_ValidRange_Returns200_AndContainsFlrId()
    {
        var response = _client.GetFlr("2023-01-01", "2023-01-07");

        ((int)response.StatusCode).Should().Be(200);
        response.Content.Should().NotBeNullOrWhiteSpace();

        var json = JToken.Parse(response.Content!);
        json.Type.Should().Be(JTokenType.Array, "NASA FLR endpoint returns an array");

        var array = (JArray)json;
        array.Should().NotBeEmpty("at least one FLR event is expected");

        var firstItem = (JObject)array.First!;
        firstItem.ContainsKey("flrID")
                 .Should().BeTrue("each FLR object should contain an 'flrID' field");
    }

    [Test, Category("API"), Category("FLR")] 
    public void GetFlr_MissingStartDate_Returns400()
    {
        var response = _client.GetFlr("", "2023-01-07");
        ((int)response.StatusCode).Should().Be(400);
    }
}
