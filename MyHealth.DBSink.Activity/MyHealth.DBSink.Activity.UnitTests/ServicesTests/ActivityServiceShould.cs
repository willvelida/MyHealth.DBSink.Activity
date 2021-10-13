using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using MyHealth.DBSink.Activity.Repository.Interfaces;
using MyHealth.DBSink.Activity.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.UnitTests.ServicesTests
{
    public class ActivityServiceShould
    {
        private Mock<IActivityRepository> _mockRepository;

        private ActivityService _sut;

        public ActivityServiceShould()
        {
            _mockRepository = new Mock<IActivityRepository>();

            _sut = new ActivityService(_mockRepository.Object);
        }

        [Fact]
        public void ThrowExceptionWhenIncomingActivityObjectIsNull()
        {
            Action activityEnveloperMapperAction = () => _sut.MapActivityToActivityEnvelope(null);

            activityEnveloperMapperAction.Should().Throw<Exception>().WithMessage("No Activity Document to Map!");
        }

        [Fact]
        public void MapActivityToActivityEnvelopeCorrectly()
        {
            var fixture = new Fixture();
            var testActivity = fixture.Create<mdl.Activity>();
            testActivity.ActivityDate = "2021-08-28";

            var expectedActivityEnvelope = _sut.MapActivityToActivityEnvelope(testActivity);

            using (new AssertionScope())
            {
                expectedActivityEnvelope.Should().BeOfType<mdl.ActivityEnvelope>();
                expectedActivityEnvelope.Activity.Should().Be(testActivity);
                expectedActivityEnvelope.DocumentType.Should().Be("Activity");
                expectedActivityEnvelope.Date.Should().Be(testActivity.ActivityDate);
            }
        }

        [Fact]
        public async Task AddActivityDocumentWhenAddActivityDocumentIsCalled()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivityDocument = fixture.Create<mdl.ActivityEnvelope>();

            // Act
            Func<Task> serviceAction = async () => await _sut.AddActivityDocument(testActivityDocument);

            // Assert
            await serviceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateItemAsyncCallFails()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivityDocument = fixture.Create<mdl.ActivityEnvelope>();

            _mockRepository.Setup(x => x.CreateActivity(It.IsAny<mdl.ActivityEnvelope>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> serviceAction = async () => await _sut.AddActivityDocument(testActivityDocument);

            // Assert
            await serviceAction.Should().ThrowAsync<Exception>();
        }
    }
}
