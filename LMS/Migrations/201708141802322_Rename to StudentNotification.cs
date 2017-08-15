namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenametoStudentNotification : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserNotifications", newName: "StudentNotifications");
            DropIndex("dbo.StudentNotifications", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.StudentNotifications", "ApplicationUserId");
            RenameColumn(table: "dbo.StudentNotifications", name: "ApplicationUser_Id", newName: "ApplicationUserId");
            AlterColumn("dbo.StudentNotifications", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.StudentNotifications", "ApplicationUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StudentNotifications", new[] { "ApplicationUserId" });
            AlterColumn("dbo.StudentNotifications", "ApplicationUserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.StudentNotifications", name: "ApplicationUserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.StudentNotifications", "ApplicationUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.StudentNotifications", "ApplicationUser_Id");
            RenameTable(name: "dbo.StudentNotifications", newName: "UserNotifications");
        }
    }
}
