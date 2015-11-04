namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Extendedequipmentconfigurationwithname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EquipmentConfiguration", "Name", c => c.String());
            AddColumn("dbo.EquipmentConfiguration", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EquipmentConfiguration", "Description");
            DropColumn("dbo.EquipmentConfiguration", "Name");
        }
    }
}
