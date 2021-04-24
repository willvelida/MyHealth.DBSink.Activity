using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using mdl = MyHealth.Common.Models;
using System.Threading.Tasks;
using MyHealth.DBSink.Activity.Models;
using System;

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

            ActivityDocument document = new ActivityDocument
            {
                Id = Guid.NewGuid().ToString(),
                Activity = activityDocument,
                DocumentType = "Activity"
            };

            await _myHealthContainer.CreateItemAsync(
                document,
                new PartitionKey(document.DocumentType),
                itemRequestOptions);
        }
    }
}
