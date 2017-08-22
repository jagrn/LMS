namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedICollectionDocumentstocoursemoduleactivityperson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Documents", "CourseId");
            CreateIndex("dbo.Documents", "ModuleId");
            CreateIndex("dbo.Documents", "ActivityId");
            CreateIndex("dbo.Documents", "ApplicationUser_Id");
            AddForeignKey("dbo.Documents", "ActivityId", "dbo.Activities", "Id");
            AddForeignKey("dbo.Documents", "CourseId", "dbo.Courses", "Id");
            AddForeignKey("dbo.Documents", "ModuleId", "dbo.Modules", "Id");
            AddForeignKey("dbo.Documents", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Documents", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Documents", "ActivityId", "dbo.Activities");
            DropIndex("dbo.Documents", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Documents", new[] { "ActivityId" });
            DropIndex("dbo.Documents", new[] { "ModuleId" });
            DropIndex("dbo.Documents", new[] { "CourseId" });
            DropColumn("dbo.Documents", "ApplicationUser_Id");
        }
    }
}
