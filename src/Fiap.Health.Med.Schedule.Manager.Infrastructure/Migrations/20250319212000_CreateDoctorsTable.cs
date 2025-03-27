using FluentMigrator;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Migrations;

[Migration(20250319212000)]
public class CreateDoctorsTable : Migration{
    public override void Up()
    {
        // Create.Schema("Users");
        // Create.Table("Doctors")
        //     .InSchema("Users")
        //     .WithColumn("Id").AsInt32().PrimaryKey().Identity()
        //     .WithColumn("CrmNumber").AsInt32().NotNullable()
        //     .WithColumn("CrmUf").AsFixedLengthString(2).NotNullable()
        //     .WithColumn("Name").AsString(200).NotNullable()
        //     .WithColumn("HashedPassword").AsString(500).NotNullable()
        //     .WithColumn("MedicalSpecialty").AsInt32().NotNullable()
        //     .WithColumn("Email").AsString(200);
        
        // Create.Index("IX_Doctors_CrmNumber_CrmUf")
        //     .OnTable("Doctors")
        //     .InSchema("Users")
        //     .OnColumn("CrmNumber")
        //     .Ascending()
        //     .OnColumn("CrmUf")
        //     .Ascending();

        // Create.Table("DoctorMedicalSpecialty")
        //     .InSchema("Users")
        //     .WithColumn("DoctorId").AsInt32().NotNullable()
        //     .ForeignKey("FK_DoctorMedicalSpecialty_Doctors_DoctorId", "Users" ,"Doctors", "Id")
        //     .WithColumn("MedicalSpecialtyId").AsInt32().NotNullable();


        // Create.PrimaryKey("PK_DoctorMedicalSpecialty")
        //     .OnTable("DoctorMedicalSpecialty")
        //     .WithSchema("Users")
        //     .Columns("DoctorId", "MedicalSpecialtyId");
    }

    public override void Down()
    {
        // Delete.Table("DoctorMedicalSpecialty").InSchema("Users");
        // Delete.Table("Doctors").InSchema("Users");
    }
}