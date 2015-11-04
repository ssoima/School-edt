namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedequipmententities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EquipmentConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EquipmentTypeId = c.Long(),
                        UsedDriverId = c.Long(),
                        WorkStationId = c.Long(),
                        IpAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentType", t => t.EquipmentTypeId)
                .ForeignKey("dbo.Workstation", t => t.WorkStationId)
                .ForeignKey("dbo.EquipmentDriver", t => t.UsedDriverId)
                .Index(t => t.EquipmentTypeId)
                .Index(t => t.UsedDriverId)
                .Index(t => t.WorkStationId);
            
            CreateTable(
                "dbo.EquipmentType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EquipmentDriver",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EquipmentTypeId = c.Long(),
                        ClrType = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentType", t => t.EquipmentTypeId)
                .Index(t => t.EquipmentTypeId);
            
            CreateTable(
                "dbo.EquipmentDriverConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EquipmentDriverConfigurationValue",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EquipmentDriverConfigurationId = c.Long(nullable: false),
                        Name = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentDriverConfiguration", t => t.EquipmentDriverConfigurationId, cascadeDelete: true)
                .Index(t => t.EquipmentDriverConfigurationId);
            
            CreateTable(
                "dbo.WorkstationTerminal",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConnectionIdentifier = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TaskAssignment", "NeededEquipmentTypeId", c => c.Long());
            AddColumn("dbo.TaskAssignment", "ProposedDriverConfigurationId", c => c.Long());
            AddColumn("dbo.StationTaskAssignment", "EquipmentConfigurationId", c => c.Long());
            AddColumn("dbo.StationTaskAssignment", "EquipmentDriverConfigurationId", c => c.Long());
            AddColumn("dbo.Workstation", "WorkStationTerminalId", c => c.Long());
            AddColumn("dbo.Task", "NeedsEquipment", c => c.Boolean(nullable: false));
            CreateIndex("dbo.StationTaskAssignment", "EquipmentConfigurationId");
            CreateIndex("dbo.StationTaskAssignment", "EquipmentDriverConfigurationId");
            CreateIndex("dbo.TaskAssignment", "NeededEquipmentTypeId");
            CreateIndex("dbo.TaskAssignment", "ProposedDriverConfigurationId");
            CreateIndex("dbo.Workstation", "WorkStationTerminalId");
            AddForeignKey("dbo.TaskAssignment", "NeededEquipmentTypeId", "dbo.EquipmentType", "Id");
            AddForeignKey("dbo.TaskAssignment", "ProposedDriverConfigurationId", "dbo.EquipmentDriverConfiguration", "Id");
            AddForeignKey("dbo.StationTaskAssignment", "EquipmentConfigurationId", "dbo.EquipmentConfiguration", "Id");
            AddForeignKey("dbo.StationTaskAssignment", "EquipmentDriverConfigurationId", "dbo.EquipmentDriverConfiguration", "Id");
            AddForeignKey("dbo.Workstation", "WorkStationTerminalId", "dbo.WorkstationTerminal", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EquipmentConfiguration", "UsedDriverId", "dbo.EquipmentDriver");
            DropForeignKey("dbo.Workstation", "WorkStationTerminalId", "dbo.WorkstationTerminal");
            DropForeignKey("dbo.EquipmentConfiguration", "WorkStationId", "dbo.Workstation");
            DropForeignKey("dbo.StationTaskAssignment", "EquipmentDriverConfigurationId", "dbo.EquipmentDriverConfiguration");
            DropForeignKey("dbo.StationTaskAssignment", "EquipmentConfigurationId", "dbo.EquipmentConfiguration");
            DropForeignKey("dbo.TaskAssignment", "ProposedDriverConfigurationId", "dbo.EquipmentDriverConfiguration");
            DropForeignKey("dbo.EquipmentDriverConfigurationValue", "EquipmentDriverConfigurationId", "dbo.EquipmentDriverConfiguration");
            DropForeignKey("dbo.TaskAssignment", "NeededEquipmentTypeId", "dbo.EquipmentType");
            DropForeignKey("dbo.EquipmentConfiguration", "EquipmentTypeId", "dbo.EquipmentType");
            DropForeignKey("dbo.EquipmentDriver", "EquipmentTypeId", "dbo.EquipmentType");
            DropIndex("dbo.Workstation", new[] { "WorkStationTerminalId" });
            DropIndex("dbo.EquipmentDriverConfigurationValue", new[] { "EquipmentDriverConfigurationId" });
            DropIndex("dbo.TaskAssignment", new[] { "ProposedDriverConfigurationId" });
            DropIndex("dbo.TaskAssignment", new[] { "NeededEquipmentTypeId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "EquipmentDriverConfigurationId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "EquipmentConfigurationId" });
            DropIndex("dbo.EquipmentDriver", new[] { "EquipmentTypeId" });
            DropIndex("dbo.EquipmentConfiguration", new[] { "WorkStationId" });
            DropIndex("dbo.EquipmentConfiguration", new[] { "UsedDriverId" });
            DropIndex("dbo.EquipmentConfiguration", new[] { "EquipmentTypeId" });
            DropColumn("dbo.Task", "NeedsEquipment");
            DropColumn("dbo.Workstation", "WorkStationTerminalId");
            DropColumn("dbo.StationTaskAssignment", "EquipmentDriverConfigurationId");
            DropColumn("dbo.StationTaskAssignment", "EquipmentConfigurationId");
            DropColumn("dbo.TaskAssignment", "ProposedDriverConfigurationId");
            DropColumn("dbo.TaskAssignment", "NeededEquipmentTypeId");
            DropTable("dbo.WorkstationTerminal");
            DropTable("dbo.EquipmentDriverConfigurationValue");
            DropTable("dbo.EquipmentDriverConfiguration");
            DropTable("dbo.EquipmentDriver");
            DropTable("dbo.EquipmentType");
            DropTable("dbo.EquipmentConfiguration");
        }
    }
}
