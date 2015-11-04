namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Processplanextended : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProcessPlan", "CurrentVersion", c => c.Int(nullable: false));
            AddColumn("dbo.ProcessPlan", "FactoryId", c => c.Long());
            CreateIndex("dbo.ProcessPlan", "FactoryId");
            AddForeignKey("dbo.ProcessPlan", "FactoryId", "dbo.Factory", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProcessPlan", "FactoryId", "dbo.Factory");
            DropIndex("dbo.ProcessPlan", new[] { "FactoryId" });
            DropColumn("dbo.ProcessPlan", "FactoryId");
            DropColumn("dbo.ProcessPlan", "CurrentVersion");
        }
    }
}
