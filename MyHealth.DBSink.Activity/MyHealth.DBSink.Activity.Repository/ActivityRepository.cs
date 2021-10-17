using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyHealth.Common.Models;
using MyHealth.DBSink.Activity.Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Activity.Repository
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _myHealthContainer;
        private readonly IConfiguration _configuration;

        public ActivityRepository(
            CosmosClient cosmosClient,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosClient = cosmosClient;
            _myHealthContainer = _cosmosClient.GetContainer(_configuration["MyHealth:DatabaseName"], _configuration["MyHealth:RecordsContainer"]);
        }

        public async Task CreateActivity(ActivityEnvelope activityEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _myHealthContainer.CreateItemAsync(
                    activityEnvelope,
                    new PartitionKey(activityEnvelope.DocumentType),
                    itemRequestOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
