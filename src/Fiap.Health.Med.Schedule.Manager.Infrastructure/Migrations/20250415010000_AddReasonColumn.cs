using FluentMigrator;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Migrations
{
    [Migration(202504150100000)]
    public class AddReasonColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("Schedule")
                 .InSchema("Schedule")
                 .AddColumn(nameof(Domain.Models.Schedule.CancelReason)).AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Column(nameof(Domain.Models.Schedule.CancelReason))
                  .FromTable("Schedule")
                  .InSchema("Schedule");
        }
    }
}
