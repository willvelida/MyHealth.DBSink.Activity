using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using MyHealth.Common.Models;
using MyHealth.DBSink.Activity.Repository;
using MyHealth.DBSink.Activity.UnitTests.TestHelpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyHealth.DBSink.Activity.UnitTests.RepositoryTests
{
    public class ActivityRepositoryShould
    {
        private Mock<CosmosClient> _mockCosmosClient;
        private Mock<Container> _mockContainer;
        private Mock<IConfiguration> _mockConfiguration;

        private ActivityRepository _sut;

        public ActivityRepositoryShould()
        {
            _mockCosmosClient = new Mock<CosmosClient>();
            _mockContainer = new Mock<Container>();
            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockContainer.Object);
            _mockConfiguration = new Mock<IConfiguration>();
            Environment.SetEnvironmentVariable("MyHealth:DatabaseName", "testdb");
            Environment.SetEnvironmentVariable("MyHealth:RecordsContainer", "testcontainer");

            _sut = new ActivityRepository(_mockCosmosClient.Object);
        }

        [Fact]
        public async Task AddActivityDocumentWhenCreateItemAsyncIsCalled()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivityDocument = fixture.Create<Common.Models.ActivityEnvelope>();

            _mockContainer.SetupCreateItemAsync<Common.Models.ActivityEnvelope>();

            // Act
            Func<Task> serviceAction = async () => await _sut.CreateActivity(testActivityDocument);

            // Assert
            await serviceAction.Should().NotThrowAsync<Exception>();
            _mockContainer.Verify(x => x.CreateItemAsync(
                It.IsAny<ActivityEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateItemAsyncCallFails()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivityDocument = fixture.Create<Common.Models.ActivityEnvelope>();

            _mockContainer.SetupCreateItemAsync<Common.Models.ActivityEnvelope>();
            _mockContainer.Setup(x => x.CreateItemAsync(
                It.IsAny<ActivityEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> serviceAction = async () => await _sut.CreateActivity(testActivityDocument);

            // Assert
            await serviceAction.Should().ThrowAsync<Exception>();
        }
    }
}
