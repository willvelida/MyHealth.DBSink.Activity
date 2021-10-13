using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyHealth.Common;
using MyHealth.Common.Models;
using MyHealth.DBSink.Activity.Functions;
using MyHealth.DBSink.Activity.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.UnitTests.FunctionTests
{
    public class CreateActivityDocumentShould
    {
        private Mock<ILogger> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IActivityService> _mockActivityDbService;
        private Mock<IServiceBusHelpers> _mockServiceBusHelpers;

        private CreateActivityDocument _func;

        public CreateActivityDocumentShould()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger>();
            _mockConfiguration.Setup(x => x["ServiceBusConnectionString"]).Returns("ServiceBusConnectionString");
            _mockServiceBusHelpers = new Mock<IServiceBusHelpers>();

            _func = new CreateActivityDocument(
                _mockConfiguration.Object,
                _mockActivityDbService.Object,
                _mockServiceBusHelpers.Object);
        }

        [Fact]
        public async Task AddActivityDocumentSuccessfully()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivity = fixture.Create<mdl.Activity>();
            var testActivityEnvelope = fixture.Create<ActivityEnvelope>();
            var testActivityDocumentString = JsonConvert.SerializeObject(testActivity);

            _mockActivityDbService.Setup(x => x.MapActivityToActivityEnvelope(It.IsAny<mdl.Activity>())).Returns(testActivityEnvelope);
            _mockActivityDbService.Setup(x => x.AddActivityDocument(It.IsAny<mdl.ActivityEnvelope>())).Returns(Task.CompletedTask);

            // Act
            await _func.Run(testActivityDocumentString, _mockLogger.Object);

            // Assert
            _mockActivityDbService.Verify(x => x.MapActivityToActivityEnvelope(It.IsAny<mdl.Activity>()), Times.Once);
            _mockActivityDbService.Verify(x => x.AddActivityDocument(It.IsAny<mdl.ActivityEnvelope>()), Times.Once);
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task CatchAndLogExceptionWhenActivityEnvelopeMapperThrowsException()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivity = fixture.Create<mdl.Activity>();
            var testActivityDocumentString = JsonConvert.SerializeObject(testActivity);

            _mockActivityDbService.Setup(x => x.MapActivityToActivityEnvelope(testActivity)).Throws<ArgumentNullException>();

            // Act
            Func<Task> responseAction = async () => await _func.Run(testActivityDocumentString, _mockLogger.Object);

            // Assert
            _mockActivityDbService.Verify(x => x.MapActivityToActivityEnvelope(It.IsAny<mdl.Activity>()), Times.Never);
            await responseAction.Should().ThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task CatchAndLogErrorWhenAddActivityDocumentThrowsException()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivityEnvelope = fixture.Create<ActivityEnvelope>();

            var testActivityDocumentString = JsonConvert.SerializeObject(testActivityEnvelope);

            _mockActivityDbService.Setup(x => x.AddActivityDocument(It.IsAny<mdl.ActivityEnvelope>())).ThrowsAsync(It.IsAny<Exception>());

            // Act
            Func<Task> responseAction = async () => await _func.Run(testActivityDocumentString, _mockLogger.Object);

            // Assert
            _mockActivityDbService.Verify(x => x.AddActivityDocument(It.IsAny<mdl.ActivityEnvelope>()), Times.Never);
            await responseAction.Should().ThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
