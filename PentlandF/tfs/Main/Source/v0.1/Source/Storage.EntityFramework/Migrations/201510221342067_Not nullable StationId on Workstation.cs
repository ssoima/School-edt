namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotnullableStationIdonWorkstation : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Workstation", new[] { "StationId" });
            AlterColumn("dbo.Workstation", "StationId", c => c.Long(nullable: false));
            CreateIndex("dbo.Workstation", "StationId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workstation", new[] { "StationId" });
            AlterColumn("dbo.Workstation", "StationId", c => c.Long());
            CreateIndex("dbo.Workstation", "StationId");
        }
    }
}
