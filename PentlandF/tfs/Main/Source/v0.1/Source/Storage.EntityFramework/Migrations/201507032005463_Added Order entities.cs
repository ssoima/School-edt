namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrderentities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderPartItem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(),
                        PartId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.Part", t => t.PartId)
                .Index(t => t.OrderId)
                .Index(t => t.PartId);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderNumber = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        PlannedDeliveryDate = c.DateTime(),
                        CustomerName = c.String(),
                        Model = c.String(),
                        Variant = c.String(),
                        Status = c.Int(nullable: false),
                        StatusChanged = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderPartItem", "PartId", "dbo.Part");
            DropForeignKey("dbo.OrderPartItem", "OrderId", "dbo.Order");
            DropIndex("dbo.OrderPartItem", new[] { "PartId" });
            DropIndex("dbo.OrderPartItem", new[] { "OrderId" });
            DropTable("dbo.Order");
            DropTable("dbo.OrderPartItem");
        }
    }
}
