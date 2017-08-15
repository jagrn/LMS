namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectedActivitymodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activities", "ActivityType", c => c.Int(nullable: false));
            DropColumn("dbo.Activities", "ActitvityType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "ActitvityType", c => c.Int(nullable: false));
            DropColumn("dbo.Activities", "ActivityType");
        }
    }
}
