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

        public ActivityRepository(
            CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _myHealthContainer = _cosmosClient.GetContainer(Environment.GetEnvironmentVariable("MyHealth:DatabaseName"), Environment.GetEnvironmentVariable("MyHealth:RecordsContainer"));
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
