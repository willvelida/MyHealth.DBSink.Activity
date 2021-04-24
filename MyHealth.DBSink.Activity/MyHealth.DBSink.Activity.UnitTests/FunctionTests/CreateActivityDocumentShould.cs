using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyHealth.DBSink.Activity.Functions;
using MyHealth.DBSink.Activity.Models;
using MyHealth.DBSink.Activity.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MyHealth.DBSink.Activity.UnitTests.FunctionTests
{
    public class CreateActivityDocumentShould
    {
        private Mock<ILogger<CreateActivityDocument>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IActivityDbService> _mockActivityDbService;

        private CreateActivityDocument _func;

        public CreateActivityDocumentShould()
        {
            _mockLogger = new Mock<ILogger<CreateActivityDocument>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["ServiceBusConnectionString"]).Returns("ServiceBusConnectionString");
            _mockActivityDbService = new Mock<IActivityDbService>();

            _func = new CreateActivityDocument(_mockLogger.Object, _mockConfiguration.Object, _mockActivityDbService.Object);
        }

        [Fact]
        public async Task AddActivityDocumentSuccessfully()
        {
            // Arrange
            var testActivityDocument = new ActivityDocument
            {
                Id = Guid.NewGuid().ToString(),
                Activity = new Common.Models.Activity
                {
                    CaloriesBurned = 10000
                },
                DocumentType = "Test"
            };

            var testActivityDocumentString = JsonConvert.SerializeObject(testActivityDocument);

            _mockActivityDbService.Setup(x => x.AddActivityDocument(It.IsAny<ActivityDocument>())).Returns(Task.CompletedTask);

            // Act
            await _func.Run(testActivityDocumentString);

            // Assert
            _mockActivityDbService.Verify(x => x.AddActivityDocument(It.IsAny<ActivityDocument>()), Times.Once);
        }

        [Fact]
        public async Task CatchAndLogErrorWhenAddActivityDocumentThrowsException()
        {
            // Arrange
            var testActivityDocument = new ActivityDocument
            {
                Id = Guid.NewGuid().ToString(),
                Activity = new Common.Models.Activity
                {
                    CaloriesBurned = 10000
                },
                DocumentType = "Test"
            };

            var testActivityDocumentString = JsonConvert.SerializeObject(testActivityDocument);

            _mockActivityDbService.Setup(x => x.AddActivityDocument(It.IsAny<ActivityDocument>())).ThrowsAsync(It.IsAny<Exception>());

            // Act
            Func<Task> responseAction = async () => await _func.Run(testActivityDocumentString);

            // Assert
            _mockActivityDbService.Verify(x => x.AddActivityDocument(It.IsAny<ActivityDocument>()), Times.Never);
            await responseAction.Should().ThrowAsync<Exception>();
        }
    }
}
