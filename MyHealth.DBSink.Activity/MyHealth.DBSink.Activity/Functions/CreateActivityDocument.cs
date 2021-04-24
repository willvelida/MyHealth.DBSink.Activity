using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.DBSink.Activity.Models;
using MyHealth.DBSink.Activity.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Activity.Functions
{
    public class CreateActivityDocument
    {
        private readonly ILogger<CreateActivityDocument> _logger;
        private readonly IConfiguration _configuration;
        private readonly IActivityDbService _activityDbService;

        public CreateActivityDocument(
            ILogger<CreateActivityDocument> logger,
            IConfiguration configuration,
            IActivityDbService activityDbService)
        {
            _logger = logger;
            _configuration = configuration;
            _activityDbService = activityDbService;
        }

        [FunctionName(nameof(CreateActivityDocument))]
        public async Task Run([ServiceBusTrigger("myhealthactivitytopic", "myhealthactivitysubscription", Connection = "ServiceBusConnectionString")] string mySbMsg)
        {
            try
            {
                // Convert incoming message into Activity Document
                var activityDocument = JsonConvert.DeserializeObject<ActivityDocument>(mySbMsg);

                // Persist Activity Document to Cosmos DB
                await _activityDbService.AddActivityDocument(activityDocument);
            }
            catch (Exception ex)
            {
                // Log Error
                _logger.LogError($"Exception thrown in {nameof(CreateActivityDocument)}", ex);
                throw;
            }
        }
    }
}
