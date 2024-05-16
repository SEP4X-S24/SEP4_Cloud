using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using weatherstation.Logic;

namespace weatherstation.Functions
{
    public class InsertData
    {
        private readonly ILogger<InsertData> _logger;

        public InsertData(ILogger<InsertData> logger)
        {
            _logger = logger;
        }

        [Function("InsertData")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            var res = req.CreateResponse(); 
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);
            try
            {
                await WeatherLogic.InsertWeatherData(json);
                res.StatusCode = System.Net.HttpStatusCode.OK;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { msg = "Data inserted succesfully" })));
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while inserting.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occured while inserting data" })));
                return res;
            }
        }
    }
}
