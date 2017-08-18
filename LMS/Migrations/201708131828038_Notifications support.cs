namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notificationssupport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        ChangeTime = c.DateTime(nullable: false),
                        EndOfRelevance = c.DateTime(nullable: false),
                        ChangeText = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "CourseId", "dbo.Courses");
            DropIndex("dbo.Notifications", new[] { "CourseId" });
            DropTable("dbo.Notifications");
        }
    }
}
