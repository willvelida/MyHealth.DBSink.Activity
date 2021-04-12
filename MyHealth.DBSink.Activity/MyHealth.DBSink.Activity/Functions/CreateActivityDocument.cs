using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.Common;

namespace MyHealth.DBSink.Activity.Functions
{
    public class CreateActivityDocument
    {
        private readonly ILogger<CreateActivityDocument> _logger;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public CreateActivityDocument(
            ILogger<CreateActivityDocument> logger,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName(nameof(CreateActivityDocument))]
        public async Task Run([ServiceBusTrigger("mytopic", "mysubscription", Connection = "")]string mySbMsg, ILogger log)
        {
            try
            {
                // Convert incoming message into Activity Document

                // Persist Activity Document to Cosmos DB
            }
            catch (Exception ex)
            {
                // Log Error
                _logger.LogError($"Exception thrown in {nameof(CreateActivityDocument)}: {ex.Message}");
                // Send Exception to Exception topic
                await _serviceBusHelpers.SendMessageToTopic(_configuration["ExceptionTopic"], ex);
            }
        }
    }
}
