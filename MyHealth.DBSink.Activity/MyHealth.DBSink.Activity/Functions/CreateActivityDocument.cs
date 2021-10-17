using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.DBSink.Activity.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Functions
{
    public class CreateActivityDocument
    {
        private readonly IActivityService _activityService;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public CreateActivityDocument(
            IActivityService activityService,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration)
        {
            _activityService = activityService;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName(nameof(CreateActivityDocument))]
        public async Task Run([ServiceBusTrigger("myhealthactivitytopic", "myhealthactivitysubscription", Connection = "ServiceBusConnectionString")] string mySbMsg, ILogger logger)
        {
            try
            {
                // Convert incoming message into Activity Document
                var activity = JsonConvert.DeserializeObject<mdl.Activity>(mySbMsg);
                var activityEnvelope = _activityService.MapActivityToActivityEnvelope(activity);
                // Persist Activity Document to Cosmos DB
                await _activityService.AddActivityDocument(activityEnvelope);
                logger.LogInformation($"Activity Document with {activityEnvelope.Date} has been persisted");
            }
            catch (Exception ex)
            {
                // Log Error
                logger.LogError($"Exception thrown in {nameof(CreateActivityDocument)}: {ex}", ex);
                await _serviceBusHelpers.SendMessageToQueue(_configuration["MyHealth:ExceptionQueue"], ex);
                throw;
            }
        }
    }
}
