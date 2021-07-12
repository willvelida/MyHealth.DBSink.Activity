using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using MyHealth.Common.Models;
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
            var fixture = new Fixture();
            var testActivityDocument = fixture.Create<Common.Models.Activity>();
            
            _mockContainer.SetupCreateItemAsync<Common.Models.Activity>();

            // Act
            Func<Task> serviceAction = async () => await _sut.AddActivityDocument(testActivityDocument);

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
            var testActivityDocument = fixture.Create<Common.Models.Activity>();

            _mockContainer.SetupCreateItemAsync<Common.Models.Activity>();
            _mockContainer.Setup(x => x.CreateItemAsync(
                It.IsAny<ActivityEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> serviceAction = async () => await _sut.AddActivityDocument(testActivityDocument);

            // Assert
            await serviceAction.Should().ThrowAsync<Exception>();
        }
    }
}
