using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.DBSink.Activity.Mappers;
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
        private readonly IActivityEnvelopeMapper _activityEnvelopeMapper;
        private readonly IServiceBusHelpers _serviceBusHelpers;

        public CreateActivityDocument(
            IConfiguration configuration,
            IActivityDbService activityDbService,
            IActivityEnvelopeMapper activityEnvelopeMapper,
            IServiceBusHelpers serviceBusHelpers)
        {
            _configuration = configuration;
            _activityDbService = activityDbService;
            _activityEnvelopeMapper = activityEnvelopeMapper;
            _serviceBusHelpers = serviceBusHelpers;
        }

        [FunctionName(nameof(CreateActivityDocument))]
        public async Task Run([ServiceBusTrigger("myhealthactivitytopic", "myhealthactivitysubscription", Connection = "ServiceBusConnectionString")] string mySbMsg, ILogger logger)
        {
            try
            {
                // Convert incoming message into Activity Document
                var activity = JsonConvert.DeserializeObject<mdl.Activity>(mySbMsg);
                var activityEnvelope = _activityEnvelopeMapper.MapActivityToActivityEnvelope(activity);
                // Persist Activity Document to Cosmos DB
                await _activityDbService.AddActivityDocument(activityEnvelope);
                logger.LogInformation($"Activity Document with {activityEnvelope.Date} has been persisted");
            }
            catch (Exception ex)
            {
                // Log Error
                logger.LogError($"Exception thrown in {nameof(CreateActivityDocument)}: {ex}", ex);
                await _serviceBusHelpers.SendMessageToQueue(_configuration["ExceptionQueue"], ex);
                throw;
            }
        }
    }
}
