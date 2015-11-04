namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskassignmentImage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskAssignmentImage",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Hash = c.String(),
                        Type = c.String(),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.StationTaskAssignment", "ShowInTerminal", c => c.Boolean(nullable: false));
            AddColumn("dbo.StationTaskAssignment", "TaskAssignmentImageId", c => c.Long());
            AddColumn("dbo.TaskAssignment", "TaskAssignmentImageId", c => c.Long());
            AddColumn("dbo.Task", "ShowInTerminal", c => c.Boolean(nullable: false));
            CreateIndex("dbo.StationTaskAssignment", "TaskAssignmentImageId");
            CreateIndex("dbo.TaskAssignment", "TaskAssignmentImageId");
            AddForeignKey("dbo.TaskAssignment", "TaskAssignmentImageId", "dbo.TaskAssignmentImage", "Id");
            AddForeignKey("dbo.StationTaskAssignment", "TaskAssignmentImageId", "dbo.TaskAssignmentImage", "Id");
            DropColumn("dbo.Task", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Task", "Type", c => c.String());
            DropForeignKey("dbo.StationTaskAssignment", "TaskAssignmentImageId", "dbo.TaskAssignmentImage");
            DropForeignKey("dbo.TaskAssignment", "TaskAssignmentImageId", "dbo.TaskAssignmentImage");
            DropIndex("dbo.TaskAssignment", new[] { "TaskAssignmentImageId" });
            DropIndex("dbo.StationTaskAssignment", new[] { "TaskAssignmentImageId" });
            DropColumn("dbo.Task", "ShowInTerminal");
            DropColumn("dbo.TaskAssignment", "TaskAssignmentImageId");
            DropColumn("dbo.StationTaskAssignment", "TaskAssignmentImageId");
            DropColumn("dbo.StationTaskAssignment", "ShowInTerminal");
            DropTable("dbo.TaskAssignmentImage");
        }
    }
}
