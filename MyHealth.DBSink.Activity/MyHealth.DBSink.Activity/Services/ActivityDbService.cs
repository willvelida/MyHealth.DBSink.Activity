using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyHealth.Common.Models;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Services
{
    public class ActivityDbService : IActivityDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _myHealthContainer;
        private readonly IConfiguration _configuration;

        public ActivityDbService(
            CosmosClient cosmosClient,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosClient = cosmosClient;
            _myHealthContainer = _cosmosClient.GetContainer(_configuration["DatabaseName"], _configuration["ContainerName"]);
        }

        public async Task AddActivityDocument(mdl.Activity activityDocument)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            ActivityEnvelope activityEnvelope = new ActivityEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                Activity = activityDocument,
                DocumentType = "Activity"
            };

            await _myHealthContainer.CreateItemAsync(
                activityEnvelope,
                new PartitionKey(activityEnvelope.DocumentType),
                itemRequestOptions);
        }
    }
}
