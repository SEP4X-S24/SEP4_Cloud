using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using weatherstation.Application.LogicInterfaces;
using weatherstation.Utils;

namespace weatherstation.Functions
{
    public class GetWeatherStatistics
    {
        private readonly ILogger<GetWeatherStatistics> _logger;
        private IWeatherLogic weatherLogic;

        public GetWeatherStatistics(ILogger<GetWeatherStatistics> logger, IWeatherLogic weatherLogic)
        {
            _logger = logger;
            this.weatherLogic = weatherLogic;
        }

        [Function("GetWeatherStatistics")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetWeatherStatistics/{period}")] HttpRequestData reqData, string period)
        {
            var res = reqData.CreateResponse();
            try
            {

                Token decoder = new Token();
                string token = decoder.Extract(reqData);

                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }

                string result = "";
                if (!decoder.IsTokenValid(token))
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "Unauthorized" }, Formatting.Indented);
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    dynamic dto = period.ToLower() switch
                    {
                        "week" => await weatherLogic.GetStatistics("WEEK"),
                        "month" => await weatherLogic.GetStatistics("MONTH"),
                        "year" => await weatherLogic.GetStatistics("YEAR"),
                        _ => throw new ArgumentException("Invalid period specified")
                    };

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dto, Formatting.Indented)));
                    return res;
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = ex.Message }, Formatting.Indented)));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "There was an error while retrieving data." }, Formatting.Indented)));
                return res;
            }
            return res;
        }
    }
}