namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssemblyLine",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FactoryId = c.Long(),
                        Type = c.String(),
                        Time = c.Time(precision: 7),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Factory", t => t.FactoryId)
                .Index(t => t.FactoryId);
            
            CreateTable(
                "dbo.Factory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LocationId = c.Long(),
                        CompanyId = c.Long(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .Index(t => t.LocationId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Country = c.String(),
                        City = c.String(),
                        Street = c.String(),
                        PostalCode = c.String(),
                        HouseNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Section",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PredecessorId = c.Long(),
                        AssemblyLineId = c.Long(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssemblyLine", t => t.AssemblyLineId)
                .ForeignKey("dbo.Section", t => t.PredecessorId)
                .Index(t => t.PredecessorId)
                .Index(t => t.AssemblyLineId);
            
            CreateTable(
                "dbo.ManufacturingModel",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Family = c.String(),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Part",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.String(),
                        LongName = c.String(),
                        PartType = c.String(),
                        PartGroup = c.String(),
                        ParentPartId = c.Long(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Part", t => t.ParentPartId)
                .Index(t => t.ParentPartId);
            
            CreateTable(
                "dbo.TaskAssignment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PredecessorId = c.Long(),
                        ParentDependencyId = c.Long(),
                        Time = c.Time(precision: 7),
                        TaskId = c.Long(),
                        PartId = c.Long(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TaskAssignment", t => t.ParentDependencyId)
                .ForeignKey("dbo.Part", t => t.PartId)
                .ForeignKey("dbo.TaskAssignment", t => t.PredecessorId)
                .ForeignKey("dbo.Task", t => t.TaskId)
                .Index(t => t.PredecessorId)
                .Index(t => t.ParentDependencyId)
                .Index(t => t.TaskId)
                .Index(t => t.PartId);
            
            CreateTable(
                "dbo.StationTaskAssignment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AssignedProcessPlanId = c.Long(),
                        PredecessorId = c.Long(),
                        WorkstationId = c.Long(),
                        StationId = c.Long(),
                        BasedOnTaskAssignmentId = c.Long(),
                        Time = c.Time(precision: 7),
                        TaskId = c.Long(),
                        PartId = c.Long(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessPlan", t => t.AssignedProcessPlanId)
                .ForeignKey("dbo.TaskAssignment", t => t.BasedOnTaskAssignmentId)
                .ForeignKey("dbo.Part", t => t.PartId)
                .ForeignKey("dbo.StationTaskAssignment", t => t.PredecessorId)
                .ForeignKey("dbo.Station", t => t.StationId)
                .ForeignKey("dbo.Task", t => t.TaskId)
                .ForeignKey("dbo.WorkStation", t => t.WorkstationId)
                .Index(t => t.AssignedProcessPlanId)
                .Index(t => t.PredecessorId)
                .Index(t => t.WorkstationId)
                .Index(t => t.StationId)
                .Index(t => t.BasedOnTaskAssignmentId)
                .Index(t => t.TaskId)
                .Index(t => t.PartId);
            
            CreateTable(
                "dbo.ProcessPlan",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Station",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OffsetTime = c.Time(precision: 7),
                        OffsetLength = c.Int(),
                        PredecessorId = c.Long(),
                        StationTypeId = c.Long(),
                        SectionId = c.Long(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Station", t => t.PredecessorId)
                .ForeignKey("dbo.Section", t => t.SectionId)
                .ForeignKey("dbo.StationType", t => t.StationTypeId)
                .Index(t => t.PredecessorId)
                .Index(t => t.StationTypeId)
                .Index(t => t.SectionId);
            
            CreateTable(
                "dbo.StationType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Length = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkStation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Side = c.String(),
                        Type = c.String(),
                        Position = c.Int(nullable: false),
                        StationId = c.Long(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Station", t => t.StationId)
                .Index(t => t.StationId);
            
            CreateTable(
                "dbo.Task",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Action = c.Int(nullable: false),
                        Type = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ValidFrom = c.DateTime(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Worker",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WorkerId = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Part", "ParentPartId", "dbo.Part");
            DropForeignKey("dbo.TaskAssignment", "TaskId", "dbo.Task");
            DropForeignKey("dbo.StationTaskAssignment", "WorkstationId", "dbo.WorkStation");
            DropForeignKey("dbo.StationTaskAssignment", "TaskId", "dbo.Task");
            DropForeignKey("dbo.StationTaskAssignment", "StationId", "dbo.Station");
            DropForeignKey("dbo.WorkStation", "StationId", "dbo.Station");
            DropForeignKey("dbo.Station", "StationTypeId", "dbo.StationType");
            DropForeignKey("dbo.Station", "SectionId", "dbo.Section");
            DropForeignKey("dbo.Station", "PredecessorId", "dbo.Station");
            DropForeignKey("dbo.StationTaskAssignment", "PredecessorId", "dbo.StationTaskAssignment");
            DropForeignKey("dbo.StationTaskAssignment", "PartId", "dbo.Part");
            DropForeignKey("dbo.StationTaskAssignment", "BasedOnTaskAssignmentId", "dbo.TaskAssignment");
            DropForeignKey("dbo.StationTaskAssignment", "AssignedProcessPlanId", "dbo.ProcessPlan");
            DropForeignKey("dbo.TaskAssignment", "PredecessorId", "dbo.TaskAssignment");
            DropForeignKey("dbo.TaskAssignment", "PartId", "dbo.Part");
            DropForeignKey("dbo.TaskAssignment", "ParentDependencyId", "dbo.TaskAssignment");
            DropForeignKey("dbo.Section", "PredecessorId", "dbo.Section");
            DropForeignKey("dbo.Section", "AssemblyLineId", "dbo.AssemblyLine");
            DropForeignKey("dbo.Factory", "LocationId", "dbo.Location");
            DropForeignKey("dbo.Factory", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.AssemblyLine", "FactoryId", "dbo.Factory");
            DropIndex("dbo.WorkStation", new[] { "StationId" });
            DropIndex("dbo.Station", new[] { "SectionId" });
            DropIndex("dbo.Station", new[] { "StationTypeId" });
            DropIndex("dbo.Station", new[] { "PredecessorId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "PartId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "TaskId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "BasedOnTaskAssignmentId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "StationId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "WorkstationId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "PredecessorId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "AssignedProcessPlanId" });
            DropIndex("dbo.TaskAssignment", new[] { "PartId" });
            DropIndex("dbo.TaskAssignment", new[] { "TaskId" });
            DropIndex("dbo.TaskAssignment", new[] { "ParentDependencyId" });
            DropIndex("dbo.TaskAssignment", new[] { "PredecessorId" });
            DropIndex("dbo.Part", new[] { "ParentPartId" });
            DropIndex("dbo.Section", new[] { "AssemblyLineId" });
            DropIndex("dbo.Section", new[] { "PredecessorId" });
            DropIndex("dbo.Factory", new[] { "CompanyId" });
            DropIndex("dbo.Factory", new[] { "LocationId" });
            DropIndex("dbo.AssemblyLine", new[] { "FactoryId" });
            DropTable("dbo.Worker");
            DropTable("dbo.Task");
            DropTable("dbo.WorkStation");
            DropTable("dbo.StationType");
            DropTable("dbo.Station");
            DropTable("dbo.ProcessPlan");
            DropTable("dbo.StationTaskAssignment");
            DropTable("dbo.TaskAssignment");
            DropTable("dbo.Part");
            DropTable("dbo.ManufacturingModel");
            DropTable("dbo.Section");
            DropTable("dbo.Location");
            DropTable("dbo.Company");
            DropTable("dbo.Factory");
            DropTable("dbo.AssemblyLine");
        }
    }
}
