
using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Fiap.Health.Med.Schedule.Manager.Domain.Enum;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;

namespace Fiap.Health.Med.Schedule.Manager.UnitTests.Application
{
    public class ScheduleServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        public IScheduleService _target;
        private readonly Mock<ILogger<ScheduleService>> _logger;
        private readonly Mock<IProducerSettings> _producer;

        public ScheduleServiceTests()
        {
            this._unitOfWorkMock = new Mock<IUnitOfWork>();
            this._logger = new Mock<ILogger<ScheduleService>>();
            this._producer = new Mock<IProducerSettings>();
            this._target = new ScheduleService(this._unitOfWorkMock.Object, this._producer.Object, this._logger.Object);
        }

        [Fact]
        public async Task HandleCreateAsync_WhenUsingCorrectTimeRange_ShouldPass()
        {
            //setup
            var model = ModelHelper.CreateSchedule().Displace(new TimeSpan(1, 0, 0));

            this._unitOfWorkMock
                .Setup(x => x.ScheduleRepository.CreateScheduleAsync(
                    It.IsAny<Domain.Models.Schedule>(),
                    CancellationToken.None))
                .ReturnsAsync(() => true);

            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.GetScheduleByIdAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(model);

            //act
            var act = async () => await this._target.HandleCreateAsync(new CreateScheduleMessage(123),CancellationToken.None);

            //assert
            await act.Should().NotThrowAsync<Exception>();
        }

        [Theory]
        [InlineData(60, 0)]
        [InlineData(-60, 0)]
        [InlineData(30, 0)]
        [InlineData(-30, 0)]
        public async Task HandleCreateAsync_WhenUsingOverlappedTimeRange_ShouldRefuse(int minutes, int seconds)
        {
            //set
            var model = ModelHelper.CreateSchedule();

            var dbModels = new List<Domain.Models.Schedule>() 
            { 
                ModelHelper.CreateSchedule()
                           .SetScheduleTime(model.ScheduleTime)
                           .Displace(new TimeSpan(0, minutes, seconds))
            };


            this._unitOfWorkMock
                .Setup(x => 
                    x.ScheduleRepository.CreateScheduleAsync(It.IsAny<Domain.Models.Schedule>(), CancellationToken.None))
                .ReturnsAsync(() => true);

            this._unitOfWorkMock
                .Setup(x => x.ScheduleRepository.GetScheduleByDoctorIdAsync(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(dbModels);

            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.GetScheduleByIdAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(model);

            //act
            var act = async () => await this._target.HandleCreateAsync(new CreateScheduleMessage(123), CancellationToken.None);

            //assert
            await act();
            this._unitOfWorkMock.Verify(x => x.ScheduleRepository.UpdatescheduleStatusAsync(model.Id, EScheduleStatus.REFUSED, CancellationToken.None));
            //await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateSchedule_WhenUsingPassedTimeRange_ShouldThrowInvalidOperationException()
        {
            //set
            var model = ModelHelper.CreateSchedule();
            model.ScheduleTime = DateTime.Now;
            model.Displace(new TimeSpan(-8, 0, 0));

            var dbModels = new List<Domain.Models.Schedule>()
            {
                ModelHelper.CreateSchedule()
                           .SetScheduleTime(model.ScheduleTime)
                           .Displace(new TimeSpan(-1, 0, 0))
            };

            this._unitOfWorkMock
                .Setup(x =>
                    x.ScheduleRepository.CreateScheduleAsync(It.IsAny<Domain.Models.Schedule>(), CancellationToken.None))
                .ReturnsAsync(() => true);

            this._unitOfWorkMock
                .Setup(x => x.ScheduleRepository.GetScheduleByDoctorIdAsync(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(dbModels);


            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.GetScheduleByIdAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(model);

            //act
            var act = async () => await this._target.HandleCreateAsync(new CreateScheduleMessage(123), CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }

    public static class ModelHelper
    {
        public static Domain.Models.Schedule CreateSchedule()
        {
            var date = DateTime.Now;
            var model = new Domain.Models.Schedule()
            {
                Id = 1,
                DoctorId = 1,
                PatientId = 1,
                CreatedAt = date,
                UpdatedAt = date,
                ScheduleTime = date + new TimeSpan(days:7,0,0,0),
                Status = EScheduleStatus.AVAILABLE
            };
            return model;
        }

        public static Domain.Models.Schedule Displace(this Domain.Models.Schedule model, TimeSpan displacement) {
            model.ScheduleTime += displacement;
            return model;
        }

        public static Domain.Models.Schedule SetScheduleTime(this Domain.Models.Schedule model, DateTime time)
        {
            model.ScheduleTime = time;
            return model;
        }
    }
}
