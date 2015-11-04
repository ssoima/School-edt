namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedconfigurationtoequipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EquipmentConfiguration", "Configuration", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EquipmentConfiguration", "Configuration");
        }
    }
}
