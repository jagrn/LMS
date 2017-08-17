namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Documentmodelmodify : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Documents", "Doc_Name");
            DropColumn("dbo.Documents", "Doc_TransactionContext");
            DropColumn("dbo.Documents", "Doc_Position");
            DropColumn("dbo.Documents", "Doc_ReadTimeout");
            DropColumn("dbo.Documents", "Doc_WriteTimeout");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "Doc_WriteTimeout", c => c.Int(nullable: false));
            AddColumn("dbo.Documents", "Doc_ReadTimeout", c => c.Int(nullable: false));
            AddColumn("dbo.Documents", "Doc_Position", c => c.Long(nullable: false));
            AddColumn("dbo.Documents", "Doc_TransactionContext", c => c.Binary());
            AddColumn("dbo.Documents", "Doc_Name", c => c.String());
        }
    }
}
