using Azure;
using IoTServiceAppLibrary.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ServiceAppAzureFunctions.Methods
{
    public class DeviceRegistration
    {
        private readonly ILogger _logger;
        private readonly IoTHubManager _hubManager;
        public DeviceRegistration(ILoggerFactory loggerFactory, IoTHubManager ioTHubManager)
        {
            _logger = loggerFactory.CreateLogger<DeviceRegistration>();
            _hubManager = ioTHubManager;
        }

        [Function("DeviceRegistration")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {

            var deviceId = req.Query["deviceId"];
            
            if(!string.IsNullOrEmpty(deviceId))
            {
                var device = await _hubManager.GetDeviceFromCloudAsync(deviceId);
                
                if(device is null)
                {
                    device = await _hubManager.RegisterDeviceToCloudAsync(deviceId);
                    return CreateResponse(req, HttpStatusCode.OK, "");
                }
                else 
                {
                    return CreateResponse(req, HttpStatusCode.Conflict, "");
                }

                //if(device is not null)
                //{
                //}

            }
            return CreateResponse(req, HttpStatusCode.BadRequest, "");
        }

        private HttpResponseData CreateResponse(HttpRequestData req, HttpStatusCode statusCode, string data)
        {
            var response = req.CreateResponse(statusCode);
            
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    response.WriteString("");
                    break;
                
                case HttpStatusCode.Conflict:
                response.WriteString("Device already exists with the same Device Id");
                    break;
            }

            return response;
        }
    }
}
