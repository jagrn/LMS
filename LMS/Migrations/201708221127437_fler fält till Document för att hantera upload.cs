namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flerfälttillDocumentföratthanteraupload : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "UploadedFileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "UploadedFileName");
        }
    }
}
