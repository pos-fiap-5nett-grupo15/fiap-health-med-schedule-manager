using FluentMigrator;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Migrations
{
    [Migration(202505052230000)]
    public class AddSchedulePrice : Migration
    {
        public override void Up()
        {
            Alter.Table("Schedule")
                 .InSchema("Schedule")
                 .AddColumn(nameof(Domain.Models.Schedule.Price)).AsDecimal().WithDefaultValue(1.00);
        }
        public override void Down()
        {
            Delete.Column(nameof(Domain.Models.Schedule.Price))
                  .FromTable("Schedule")
                  .InSchema("Schedule");
        }
    }
}
