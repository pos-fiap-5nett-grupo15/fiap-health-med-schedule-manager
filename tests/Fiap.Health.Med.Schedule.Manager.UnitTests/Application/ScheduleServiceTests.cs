
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.UpdateSchedule;
using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Fiap.Health.Med.Schedule.Manager.Domain.Enum;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;
using System.Net;

namespace Fiap.Health.Med.Schedule.Manager.UnitTests.Application
{
    public class ScheduleServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        public IScheduleService _target;
        public ScheduleServiceTests()
        {
            this._unitOfWorkMock = new Mock<IUnitOfWork>();
            this._target = new ScheduleService(this._unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateSchedule_WhenUsingCorrectTimeRange_ShouldPass()
        {
            //setup
            var model = ModelHelper.CreateSchedule();

            this._unitOfWorkMock
                .Setup(x => x.ScheduleRepository.CreateScheduleAsync(
                    It.IsAny<Domain.Models.Schedule>(),
                    CancellationToken.None))
                .ReturnsAsync(() => true);
            this._unitOfWorkMock
                .Setup(x => x.ScheduleRepository.GetScheduleByDoctorIdAsync(
                    It.IsAny<int>(),
                    CancellationToken.None))
                .ReturnsAsync(new List<Domain.Models.Schedule>());

            //act
            var act = async () => await this._target.CreateScheduleAsync(model, CancellationToken.None);

            //assert
            await act.Should().NotThrowAsync<Exception>();
        }

        [Theory]
        [InlineData(60, 0)]
        [InlineData(-60, 0)]
        [InlineData(30, 0)]
        [InlineData(-30, 0)]
        public async Task CreateSchedule_WhenUsingOverlappedTimeRange_ShouldThrowInvalidOperationException(int minutes, int seconds)
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

            //act
            var act = async () => await this._target.CreateScheduleAsync(model, CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateSchedule_WhenUsingPassedTimeRange_ShouldThrowInvalidOperationException()
        {
            //set
            var model = ModelHelper.CreateSchedule();

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

            //act
            var act = async () => await this._target.CreateScheduleAsync(model, CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task AcceptScheduleAsync_WhenRepositoryReturnsError_ShouldFailWithUnprocessableContent()
        {
            // Arrange
            var scheduleId = 1L;
            var doctorId = 1;
            var expectedError = "Erro ao buscar agendamento";

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, CancellationToken.None))
                .ReturnsAsync((null, expectedError));

            // Act
            var result = await _target.AcceptScheduleAsync(scheduleId, doctorId, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.UnprocessableContent);
        }

        [Fact]
        public async Task AcceptScheduleAsync_WhenScheduleIsNull_ShouldFailWithNotFound()
        {
            // Arrange
            var scheduleId = 1L;
            var doctorId = 1;

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, CancellationToken.None))
                .ReturnsAsync((null as Domain.Models.Schedule, null));

            // Act
            var result = await _target.AcceptScheduleAsync(scheduleId, doctorId, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AcceptScheduleAsync_WhenScheduleIsPendingAndTimePassed_ShouldDeleteAndReturnSuccess()
        {
            // Arrange
            var scheduleId = 1L;
            var doctorId = 1;

            var schedule = ModelHelper.CreateSchedule();
            schedule.ScheduleTime = DateTime.Now.AddHours(-1);
            schedule.Status = EScheduleStatus.PENDING_CONFIRMATION;

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, CancellationToken.None))
                .ReturnsAsync((schedule, null));

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.DeleteScheduleStatusAsync(scheduleId, CancellationToken.None))
                .ReturnsAsync((true, null));

            // Act
            var result = await _target.AcceptScheduleAsync(scheduleId, doctorId, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AcceptScheduleAsync_WhenScheduleIsPendingAndTimeFuture_ShouldConfirmAndReturnSuccess()
        {
            // Arrange
            var scheduleId = 1L;
            var doctorId = 1;

            var schedule = ModelHelper.CreateSchedule();
            schedule.ScheduleTime = DateTime.Now.AddHours(1);
            schedule.Status = EScheduleStatus.PENDING_CONFIRMATION;

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, CancellationToken.None))
                .ReturnsAsync((schedule, null));

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.CONFIRMED, CancellationToken.None))
                .ReturnsAsync((true, null));

            // Act
            var result = await _target.AcceptScheduleAsync(scheduleId, doctorId, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AcceptScheduleAsync_WhenScheduleIsCanceledByDoctor_ShouldRefuseAndReturnSuccess()
        {
            // Arrange
            var scheduleId = 1L;
            var doctorId = 1;

            var schedule = ModelHelper.CreateSchedule();
            schedule.Status = EScheduleStatus.CANCELED_BY_DOCTOR;

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, CancellationToken.None))
                .ReturnsAsync((schedule, null));

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.REFUSED, CancellationToken.None))
                .ReturnsAsync((true, null));

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.DeleteScheduleStatusAsync(scheduleId, CancellationToken.None))
                .ReturnsAsync((true, null));

            // Act
            var result = await _target.AcceptScheduleAsync(scheduleId, doctorId, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AcceptScheduleAsync_WhenScheduleHasInvalidStatus_ShouldReturnError()
        {
            // Arrange
            var scheduleId = 1L;
            var doctorId = 1;

            var schedule = ModelHelper.CreateSchedule();
            schedule.Status = EScheduleStatus.CONFIRMED; // qualquer status inválido para aceitar

            _unitOfWorkMock.Setup(x =>
                x.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, CancellationToken.None))
                .ReturnsAsync((schedule, null));

            // Act
            var result = await _target.AcceptScheduleAsync(scheduleId, doctorId, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.UnprocessableContent);
        }


        [Fact]
        public async Task UpdateSchedule_WhenScheduleDoesNotExist_ShouldReturnFailureWithErrorMessage()
        {
            //set
            var updateScheduleRequestDto = new UpdateScheduleRequestDto()
            {
                Id = 1,
                ScheduleTime = DateTime.Now.AddHours(1)
            };
            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.GetScheduleByIdAsync(It.IsAny<long>(), CancellationToken.None)).ReturnsAsync((Domain.Models.Schedule?)null);

            //act
            var result = await this._target.UpdateScheduleAsync(updateScheduleRequestDto, default);

            //assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Agendamento não encontrado", result.Errors);
        }

        [Fact]
        public async Task UpdateSchedule_WhenDataIsCorrect_ShoudReturnSuccess()
        {
            //set
            var updateScheduleRequestDto = new UpdateScheduleRequestDto()
            {
                Id = 1,
                ScheduleTime = DateTime.Now.AddHours(4)
            };

            var dbModel = ModelHelper.CreateSchedule().SetScheduleTime(DateTime.Now.AddDays(2));

            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.UpdateScheduleAsync(It.IsAny<Domain.Models.Schedule>(), default)).ReturnsAsync(1);
            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.GetScheduleByIdAsync(It.IsAny<long>(), default)).ReturnsAsync(dbModel);

            //act
            var result = await this._target.UpdateScheduleAsync(updateScheduleRequestDto, default);

            //assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateSchedule_WhenHasOverlap_ShoudReturnErrorWithMessage()
        {
            //set
            var updateScheduleRequestDto = new UpdateScheduleRequestDto()
            {
                Id = 1,
                ScheduleTime = DateTime.Now.AddHours(4)
            };

            var doctorSchedules = new List<Domain.Models.Schedule>()
            {
                new Domain.Models.Schedule()
                {
                    Id = 1,
                    Status = EScheduleStatus.PENDING_CONFIRMATION,
                    ScheduleTime = DateTime.Now.AddHours(3),
                    CreatedAt = DateTime.Now,
                    DoctorId = 1,
                },
                new Domain.Models.Schedule()
                {
                    Id = 2,
                    Status = EScheduleStatus.AVAILABLE,
                    ScheduleTime = DateTime.Now.AddHours(5),
                    DoctorId = 1,
                },
                new Domain.Models.Schedule()
                {
                    Id = 3,
                    Status = EScheduleStatus.REFUSED,
                    ScheduleTime = DateTime.Now.AddHours(2),
                    DoctorId = 1
                }
            };

            var dbModel = ModelHelper.CreateSchedule().SetScheduleTime(DateTime.Now.AddDays(2));

            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.UpdateScheduleAsync(It.IsAny<Domain.Models.Schedule>(), default)).ReturnsAsync(1);
            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.GetScheduleByIdAsync(It.IsAny<long>(), default)).ReturnsAsync(dbModel);
            this._unitOfWorkMock.Setup(x => x.ScheduleRepository.GetScheduleByDoctorIdAsync(It.IsAny<int>(), default)).ReturnsAsync(doctorSchedules);

            //act
            var result = await this._target.UpdateScheduleAsync(updateScheduleRequestDto, default);

            //assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Data do agendamento em conflito com data(s) existente(s)", result.Errors);
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
                ScheduleTime = date + new TimeSpan(days: 7, 0, 0, 0),
                Status = EScheduleStatus.AVAILABLE
            };
            return model;
        }

        public static Domain.Models.Schedule Displace(this Domain.Models.Schedule model, TimeSpan displacement)
        {
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
