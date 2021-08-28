using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MyHealth.DBSink.Activity.Mappers;
using System;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.UnitTests.MapperTests
{
    public class ActivityEnvelopeMapperShould
    {
        private ActivityEnvelopeMapper _sut;

        public ActivityEnvelopeMapperShould()
        {
            _sut = new ActivityEnvelopeMapper();
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
    }
}
