namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documentmodelnewpropDokumenttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "DokumentType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "DokumentType");
        }
    }
}
