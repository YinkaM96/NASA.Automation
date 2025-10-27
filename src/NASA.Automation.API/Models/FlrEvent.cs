using Newtonsoft.Json;

namespace NASA.Automation.API.Models
{
    public class FlrEvent
    {
        [JsonProperty("flrID")] public string? FlrId { get; set; }
        [JsonProperty("classType")] public string? Class { get; set; }
        [JsonProperty("beginTime")] public string? Begin { get; set; }
        [JsonProperty("sourceLocation")] public string? SourceLocation { get; set; }
    }

}
