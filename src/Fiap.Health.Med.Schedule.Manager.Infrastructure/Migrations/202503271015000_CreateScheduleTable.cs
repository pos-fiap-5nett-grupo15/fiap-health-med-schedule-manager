using FluentMigrator;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Migrations;

[Migration(202503271015000)]
public class CreateScheduleTable : Migration{
    public override void Up()
    {
        Create.Schema("Schedule");
        
        Create.Table("Schedule")
            .InSchema("Schedule")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("DoctorId").AsInt32().NotNullable()
            .WithColumn("PatientId").AsInt32().NotNullable()
            .WithColumn("IsActive").AsBoolean().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().NotNullable()
            .WithColumn("ScheduleTime").AsDateTime().NotNullable()
            .WithColumn("DoctorName").AsString(255).NotNullable()
            .WithColumn("PatientName").AsString(255).NotNullable()
            .WithColumn("IsConfirmed").AsBoolean().NotNullable();
        
        Create.Index("IX_Schedule_DoctorId")
            .OnTable("Schedule")
            .InSchema("Schedule")
            .OnColumn("DoctorId")
            .Ascending();
        
        Create.Index("IX_Schedule_PatientId")
            .OnTable("Schedule")
            .InSchema("Schedule")
            .OnColumn("PatientId")
            .Ascending();
        
        Create.Index("IX_Schedule_ScheduleTime")
            .OnTable("Schedule")
            .InSchema("Schedule")
            .OnColumn("ScheduleTime")
            .Ascending();
    }

    public override void Down()
    {
        Delete.Table("Schedule").InSchema("Schedule");
        Delete.Schema("Schedule");
    }
}