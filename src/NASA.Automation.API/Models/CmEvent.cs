using Newtonsoft.Json;

namespace NASA.Automation.API.Models
{
    public class CmEvent
    {
        [JsonProperty("activityID")] public string? ActivityId { get; set; }
        [JsonProperty("startTime")] public string? StartTime { get; set; }
        [JsonProperty("sourceLocation")] public string? SourceLocation { get; set; }
    }

}