namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedmoduleIdfromnotes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Notifications", "ModuleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "ModuleId", c => c.Int(nullable: false));
        }
    }
}
