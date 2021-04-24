using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using MyHealth.DBSink.Activity.Models;
using MyHealth.DBSink.Activity.Services;
using MyHealth.DBSink.Activity.UnitTests.TestHelpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyHealth.DBSink.Activity.UnitTests.ServicesTests
{
    public class ActivityDbServiceShould
    {
        private Mock<CosmosClient> _mockCosmosClient;
        private Mock<Container> _mockContainer;
        private Mock<IConfiguration> _mockConfiguration;

        private ActivityDbService _sut;

        public ActivityDbServiceShould()
        {
            _mockCosmosClient = new Mock<CosmosClient>();
            _mockContainer = new Mock<Container>();
            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockContainer.Object);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["DatabaseName"]).Returns("db");
            _mockConfiguration.Setup(x => x["ContainerName"]).Returns("col");

            _sut = new ActivityDbService(_mockCosmosClient.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task AddActivityDocumentWhenCreateItemAsyncIsCalled()
        {
            // Arrange
            Common.Models.Activity testActivityDocument = new Common.Models.Activity
            {
                CaloriesBurned = 10000
            };


            _mockContainer.SetupCreateItemAsync<Common.Models.Activity>();

            // Act
            await _sut.AddActivityDocument(testActivityDocument);

            // Assert
            _mockContainer.Verify(x => x.CreateItemAsync(
                It.IsAny<ActivityDocument>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
