namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Documentfieldsallownulls : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Documents", "CourseId", c => c.Int());
            AlterColumn("dbo.Documents", "ModuleId", c => c.Int());
            AlterColumn("dbo.Documents", "ActivityId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Documents", "ActivityId", c => c.Int(nullable: false));
            AlterColumn("dbo.Documents", "ModuleId", c => c.Int(nullable: false));
            AlterColumn("dbo.Documents", "CourseId", c => c.Int(nullable: false));
        }
    }
}
