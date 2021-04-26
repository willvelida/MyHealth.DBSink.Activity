using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.DBSink.Activity.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Functions
{
    public class CreateActivityDocument
    {
        private readonly IConfiguration _configuration;
        private readonly IActivityDbService _activityDbService;

        public CreateActivityDocument(
            IConfiguration configuration,
            IActivityDbService activityDbService)
        {
            _configuration = configuration;
            _activityDbService = activityDbService;
        }

        [FunctionName(nameof(CreateActivityDocument))]
        public async Task Run([ServiceBusTrigger("myhealthactivitytopic", "myhealthactivitysubscription", Connection = "ServiceBusConnectionString")] string mySbMsg, ILogger logger)
        {
            try
            {
                // Convert incoming message into Activity Document
                var activityDocument = JsonConvert.DeserializeObject<mdl.Activity>(mySbMsg);

                // Persist Activity Document to Cosmos DB
                await _activityDbService.AddActivityDocument(activityDocument);
                logger.LogInformation($"Activity Document with {activityDocument.ActivityDate} has been persisted");
            }
            catch (Exception ex)
            {
                // Log Error
                logger.LogError($"Exception thrown in {nameof(CreateActivityDocument)}: {ex}", ex);
                throw;
            }
        }
    }
}
