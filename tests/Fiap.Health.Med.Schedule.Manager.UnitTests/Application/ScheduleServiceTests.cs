
using Fiap.Health.Med.Schedule.Manager.Application.Services;
using FluentAssertions;
using Moq;

namespace Fiap.Health.Med.Schedule.Manager.UnitTests.Application
{
    public class ScheduleServiceTests
    {
        private IScheduleService _target;
        public ScheduleServiceTests()
        {
            this._target = new Mock<IScheduleService>().Object;
        }

        [Fact]
        public async Task CreateSchedule_WhenUsingCorrectTimeRange_ShouldPass()
        {
            //set
            var model = ModelHelper.CreateSchedule();

            //act
            var act = async () => await this._target.CreateSchedule(model,CancellationToken.None);

            //assert
            await act.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateSchedule_WhenUsingOverlapedTimeRange_ShouldThrowInvalidOperationException()
        {
            //set
            var model = ModelHelper.CreateSchedule();

            //act
            var act = async () => await this._target.CreateSchedule(model, CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateSchedule_WhenUsingPassedTimeRange_ShouldThrowInvalidOperationException()
        {
            //set
            var model = ModelHelper.CreateSchedule();

            //act
            var act = async () => await this._target.CreateSchedule(model, CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }

    public static class ModelHelper
    {
        public static Domain.Models.Schedule CreateSchedule()
        {
            var model = new Domain.Models.Schedule();
            return model;
        }
    }
}
