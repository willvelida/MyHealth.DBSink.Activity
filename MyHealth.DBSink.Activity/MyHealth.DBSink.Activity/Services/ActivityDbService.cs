using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyHealth.DBSink.Activity.Models;
using System.Threading.Tasks;

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

        public async Task AddActivityDocument(ActivityDocument activityDocument)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            await _myHealthContainer.CreateItemAsync(
                activityDocument,
                new PartitionKey(activityDocument.DocumentType),
                itemRequestOptions);
        }
    }
}
