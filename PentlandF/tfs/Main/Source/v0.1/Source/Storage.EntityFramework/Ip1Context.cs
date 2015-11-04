using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Models.Manufacturing;
using NextLAP.IP1.Models.Planning;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.Models.Utils;
using Pentlandfirth.Common;

namespace NextLAP.IP1.Storage.EntityFramework
{
    public class Ip1Context : DbContext
    {
        public Ip1Context()
            : base("Name=IP1DbConnection")
        {
#if DEBUG
            Database.Log = DebugHelper.LogToDebug;
#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Conventions Override

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #endregion

            #region Fluent API for Defining Relations

            modelBuilder.Entity<TaskAssignment>()
                .HasOptional(x => x.ParentDependency)
                .WithMany(x => x.Dependencies)
                .HasForeignKey(x => x.ParentDependencyId);
            modelBuilder.Entity<TaskAssignment>()
                .HasOptional(x => x.Predecessor)
                .WithMany()
                .HasForeignKey(x => x.PredecessorId);

            modelBuilder.Entity<EquipmentDriverConfigurationValue>()
                .HasRequired(x => x.EquipmentDriverConfiguration)
                .WithMany(x => x.Values)
                .WillCascadeOnDelete(true);

            #endregion

            base.OnModelCreating(modelBuilder);
        }

        #region DbSets for "Structure"

        public DbSet<Company> Companies { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<AssemblyLine> AssemblyLines { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<StationType> StationTypes { get; set; }
        public DbSet<Workstation> WorkStations { get; set; }
        public DbSet<Worker> Workers { get; set; }

        #endregion

        #region DbSets for "Planning"

        public DbSet<ManufacturingModel> ManufacturingModels { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<StationTaskAssignment> StationTaskAssignments { get; set; }
        public DbSet<TaskAssignmentImage> TaskAssignmentImages { get; set; }
        public DbSet<ProcessPlan> ProcessPlans { get; set; }

        #endregion

        #region DbSets for Equipment

        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<EquipmentConfiguration> EquipmentConfigurations { get; set; }
        public DbSet<EquipmentDriver> EquipmentDrivers { get; set; }
        public DbSet<EquipmentDriverConfiguration> EquipmentDriverConfigurations { get; set; }

        #endregion

        #region DbSets for Manufacturing

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPartItem> OrderPartItems { get; set; }

        #endregion

        
        /// <summary>
        /// Method for resetting the database and reseed (used for POC)
        /// </summary>
        public static void ResetDB()
        {
            using (var context = new NextLAP.IP1.Storage.EntityFramework.Ip1Context())
            {
                context.Database.ExecuteSqlCommand(@"while(exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='FOREIGN KEY'))
begin
 declare @sql nvarchar(2000)
 SELECT TOP 1 @sql=('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME
 + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
 FROM information_schema.table_constraints
 WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
 exec (@sql)
 PRINT @sql
end");
                context.Database.ExecuteSqlCommand(@"while(exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'dbo'))
begin
 declare @sql nvarchar(2000)
 SELECT TOP 1 @sql=('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME
 + ']')
 FROM INFORMATION_SCHEMA.TABLES
 WHERE TABLE_SCHEMA = 'dbo'
exec (@sql)
 PRINT @sql
end");
            }
            NextLAP.IP1.Storage.EntityFramework.Migrations.Configuration config = new NextLAP.IP1.Storage.EntityFramework.Migrations.Configuration();
            System.Data.Entity.Migrations.DbMigrator migrator = new System.Data.Entity.Migrations.DbMigrator(config);

            foreach (string s in migrator.GetPendingMigrations().OrderBy(x => x))
            {
                migrator.Update(s);
            }
        }
    }
}
