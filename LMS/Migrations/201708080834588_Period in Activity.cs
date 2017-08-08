namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PeriodinActivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activities", "ActivityPeriod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activities", "ActivityPeriod");
        }
    }
}
