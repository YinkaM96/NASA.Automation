using RestSharp;
using NASA.Automation.Core;

namespace NASA.Automation.API;

    public class NasaDonkiClient
    {
        private readonly RestClient _client = new(ConfigManager.BaseUrl);

        public RestResponse GetCme(string startDate, string endDate)
        {
            var req = new RestRequest("/DONKI/CME")
              .AddParameter("startDate", startDate)
              .AddParameter("endDate", endDate)
              .AddParameter("api_key", ConfigManager.ApiKey);

            return _client.Execute(req);
        }

        public RestResponse GetFlr(string startDate, string endDate, bool includeKey = true)
        {
            var req = new RestRequest("/DONKI/FLR")
              .AddParameter("endDate", endDate);

            if (!string.IsNullOrWhiteSpace(startDate))
                req.AddParameter("startDate", startDate);

            if (includeKey)
                req.AddParameter("api_key", ConfigManager.ApiKey);

            return _client.Execute(req);
        }
    }

