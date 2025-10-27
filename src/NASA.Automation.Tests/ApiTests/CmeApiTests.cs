using FluentAssertions;
using Newtonsoft.Json.Linq;
using NASA.Automation.API;
using NUnit.Framework;

namespace NASA.Automation.Tests.ApiTests;

[TestFixture]
public class CmeApiTests
{
    private NasaDonkiClient _client = null!;

    [SetUp]
    public void Setup() => _client = new NasaDonkiClient();

    [Test, Category("API"), Category("CME")]
    public void GetCME_ValidRange_Returns200_AndContainsActivityId()
    {
        // Act
        var response = _client.GetCme("2023-01-01", "2023-01-07");

        // Assert
        ((int)response.StatusCode).Should().Be(200, "valid date range should return 200 OK");
        response.Content.Should().NotBeNullOrWhiteSpace("API should return a non-empty body");

        var json = JToken.Parse(response.Content!);
        json.Type.Should().Be(JTokenType.Array, "NASA CME endpoint returns an array");

        var array = (JArray)json;
        array.Should().NotBeEmpty("at least one CME event is expected");

        var firstItem = (JObject)array.First!;
        firstItem.ContainsKey("activityID")
                 .Should().BeTrue("each CME object should contain an 'activityID' field");
    }

    [Test, Category("API"), Category("CME")]
    public void GetCME_InvalidDateFormat_Returns400()
    {
        var response = _client.GetCme("2023/01/01", "2023-01-07");
        ((int)response.StatusCode).Should().Be(400, "invalid date formats should be rejected with 400");
    }
}
