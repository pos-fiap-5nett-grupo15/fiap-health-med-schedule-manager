using FluentMigrator;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Migrations;

[Migration(202503271015000)]
public class CreateScheduleTable : Migration
{
    public override void Up()
    {
        Create.Schema("Schedule");
        
        Create.Table("Schedule")
            .InSchema("Schedule")
            .WithColumn(nameof(Domain.Models.Schedule.Id)).AsInt64().PrimaryKey().Identity()
            .WithColumn(nameof(Domain.Models.Schedule.DoctorId)).AsInt32().NotNullable()
            .WithColumn(nameof(Domain.Models.Schedule.PatientId)).AsInt32().NotNullable()
            .WithColumn(nameof(Domain.Models.Schedule.CreatedAt)).AsDateTime().NotNullable()
            .WithColumn(nameof(Domain.Models.Schedule.UpdatedAt)).AsDateTime().Nullable()
            .WithColumn(nameof(Domain.Models.Schedule.ScheduleTime)).AsDateTime().NotNullable()
            .WithColumn(nameof(Domain.Models.Schedule.Status)).AsInt64().NotNullable();

        Create.Index("IX_Schedule_DoctorId")
            .OnTable("Schedule")
            .InSchema("Schedule")
            .OnColumn(nameof(Domain.Models.Schedule.DoctorId))
            .Ascending();
        
        Create.Index("IX_Schedule_PatientId")
            .OnTable("Schedule")
            .InSchema("Schedule")
            .OnColumn(nameof(Domain.Models.Schedule.PatientId))
            .Ascending();
        
        Create.Index("IX_Schedule_ScheduleTime")
            .OnTable("Schedule")
            .InSchema("Schedule")
            .OnColumn(nameof(Domain.Models.Schedule.ScheduleTime))
            .Ascending();
    }

    public override void Down()
    {
        Delete.Table("Schedule").InSchema("Schedule");
        Delete.Schema("Schedule");
    }
}