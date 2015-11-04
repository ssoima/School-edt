using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Models.Planning;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.Models.Utils;

namespace NextLAP.IP1.Storage.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<NextLAP.IP1.Storage.EntityFramework.Ip1Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NextLAP.IP1.Storage.EntityFramework.Ip1Context context)
        {
            //  This method will be called after migrating to the latest version.

            #region Test Data

            #region Equipment

            context.EquipmentTypes.AddOrUpdate(x => x.Name, new[]
            {
                new EquipmentType() {Name = "Shelf Regular", Description = "An equipment for pick activities."},
                new EquipmentType() {Name = "Shelf Pick-by-light", Description = "An equipment for pick activities."},
                new EquipmentType() {Name = "Shelf Pick-by-voice", Description = "An equipment for pick activities."},
                new EquipmentType() {Name = "Scanner", Description = "An equipment for scan activities."},
                new EquipmentType() {Name = "Screwdriver", Description = "An equipment for screw activities."},
                new EquipmentType() {Name = "Screwdriver EC", Description = "An equipment for screw activities."},
                new EquipmentType() {Name = "Screwdriver EC & Accu", Description = "An equipment for screw activities."},
                new EquipmentType()
                {
                    Name = "AR google glass",
                    Description = "An equipment for augmented reality supported activities."
                },
                new EquipmentType() {Name = "Other tool", Description = "Any other equipment."},
            });
            context.SaveChanges();

            var equipmentTypes = context.EquipmentTypes.ToList();

            #region Equipment Drivers

            context.EquipmentDrivers.AddOrUpdate(x => new { x.Name, x.ClrType }, new[]
            {
                new EquipmentDriver()
                {
                    Name = "Standard Shelf Regular",
                    Description = "A noop equipment driver for a simple no input no out shelf.",
                    EquipmentType = equipmentTypes.FirstOrDefault(x => x.Name == "Shelf Regular"),
                    ClrType = "NextLAP.IP1.EquipmentDrivers.Shelf.NullShelfImplementation, NextLAP.IP1.EquipmentDrivers"
                },
                new EquipmentDriver()
                {
                    Name = "Standard Shelf Pick-by-light",
                    Description = "A noop equipment driver for a simple no input no out shelf.",
                    EquipmentType = equipmentTypes.FirstOrDefault(x => x.Name == "Shelf Pick-by-light"),
                    ClrType = "NextLAP.IP1.EquipmentDrivers.Shelf.NullShelfImplementation, NextLAP.IP1.EquipmentDrivers"
                },
                new EquipmentDriver()
                {
                    Name = "Standard Shelf Pick-by-voice",
                    Description = "A noop equipment driver for a simple no input no out shelf.",
                    EquipmentType = equipmentTypes.FirstOrDefault(x => x.Name == "Shelf Pick-by-voice"),
                    ClrType = "NextLAP.IP1.EquipmentDrivers.Shelf.NullShelfImplementation, NextLAP.IP1.EquipmentDrivers"
                },
                new EquipmentDriver()
                {
                    Name = "Simulator PBL Shelf",
                    Description = "A simulated pick by light shelf.",
                    EquipmentType = equipmentTypes.FirstOrDefault(x => x.Name == "Shelf Pick-by-light"),
                    ClrType =
                        "NextLAP.IP1.EquipmentDrivers.Shelf.PickByLightSimulatorImpl, NextLAP.IP1.EquipmentDrivers"
                },
                new EquipmentDriver()
                {
                    Name = "Standard Scanner",
                    Description = "A noop equipment driver for a scanner.",
                    EquipmentType = equipmentTypes.FirstOrDefault(x => x.Name == "Scanner"),
                    ClrType =
                        "NextLAP.IP1.EquipmentDrivers.Scanner.NullScannerImplementation, NextLAP.IP1.EquipmentDrivers"
                },
                new EquipmentDriver()
                {
                    Name = "Standard Screwdriver",
                    Description = "A noop equipment driver for a simple screwdriver.",
                    EquipmentType = equipmentTypes.FirstOrDefault(x => x.Name == "Screwdriver"),
                    ClrType =
                        "NextLAP.IP1.EquipmentDrivers.Screwdriver.NullScrewdriverImplementation, NextLAP.IP1.EquipmentDrivers"
                },
            });
            context.SaveChanges();

            #endregion

            #endregion

            #region Structure

            #region Company

            context.Companies.AddOrUpdate(x => new { x.Name },
                new Company() { Name = "Audi AG", Description = "" });
            context.SaveChanges();
            var company = context.Companies.FirstOrDefault(x => x.Name == "Audi AG");
            if (company == null) throw new InvalidOperationException("Company 'Audi AG' does not exist.");

            #endregion

            #region Location

            context.Locations.AddOrUpdate(x => new { x.Street, x.City },
                new Location()
                {
                    Street = "",
                    City = "Ingolstadt",
                    PostalCode = "85045",
                    Country = "Germany"
                });
            context.SaveChanges();
            var location = context.Locations.FirstOrDefault(x => x.Street == "");
            if (location == null) throw new InvalidOperationException("Location does not exist.");

            #endregion

            #region Factory

            context.Factories.AddOrUpdate(x => new { x.Name },
                new Factory()
                {
                    Name = "Plant Ingolstadt",
                    Description = "Production of models A3, A4, A5, Q5",
                    CompanyId = company.Id,
                    LocationId = location.Id
                });
            context.SaveChanges();
            var factory = context.Factories.FirstOrDefault(x => x.Name == "Plant Ingolstadt");
            if (factory == null) throw new InvalidOperationException("Factory 'Plant Ingolstadt' does not exist.");

            #endregion

            #region Assembly Line

            context.AssemblyLines.AddOrUpdate(x => new { x.Name }, new AssemblyLine[]
            {
                new AssemblyLine()
                {
                    Name = "Assembly B1",
                    Description = "",
                    Time = TimeSpan.FromSeconds(1760),
                    Type = "Main",
                    FactoryId = factory.Id
                },
                new AssemblyLine()
                {
                    Name = "Assembly B2",
                    Description = "",
                    Time = TimeSpan.FromSeconds(1760),
                    Type = "Main",
                    FactoryId = factory.Id
                },
                new AssemblyLine()
                {
                    Name = "Assembly A",
                    Description = "",
                    Time = TimeSpan.FromSeconds(17600),
                    Type = "Main",
                    FactoryId = factory.Id
                }
            });
            context.SaveChanges();
            var assemblyLine = context.AssemblyLines.FirstOrDefault(x => x.Name == "Assembly B1");
            if (assemblyLine == null)
                throw new InvalidOperationException("Assembly line 'Assembly B1' does not exist.");

            #endregion

            #region Section

            context.Sections.AddOrUpdate(x => new { x.Name }, new Section[]
            {
                new Section() {Name = "Section 1", AssemblyLineId = assemblyLine.Id},
                new Section() {Name = "Section 2", AssemblyLineId = assemblyLine.Id},
                new Section() {Name = "Section 3", AssemblyLineId = assemblyLine.Id},
                new Section() {Name = "Section 4", AssemblyLineId = assemblyLine.Id},
                new Section() {Name = "Section 5", AssemblyLineId = assemblyLine.Id}
            });
            context.SaveChanges();

            var sectionNames = new[]
            {
                "Section 1", "Section 2", "Section 3", "Section 4", "Section 5"
            };
            var sections = context.Sections.Where(x => sectionNames.Contains(x.Name)).ToList();
            if (sections.Count != sectionNames.Length) throw new InvalidOperationException("Not all sections exists.");
            // now we will link them together
            for (var i = 1; i < sectionNames.Length; i++)
            {
                var sectionToUpdate = sections.First(x => x.Name == sectionNames[i]);
                var sectionPredecessor = sections.First(x => x.Name == sectionNames[i - 1]);
                sectionToUpdate.Predecessor = sectionPredecessor;
            }
            context.SaveChanges();
            var sectionOne = sections.FirstOrDefault(x => x.Name == "Section 1");
            if (sectionOne == null)
                throw new InvalidOperationException("Section 'Section 1' does not exist.");

            #endregion

            #region Station Types

            context.StationTypes.AddOrUpdate(x => new { x.Name },
                new StationType[]
                {
                    new StationType() {Name = "Worker", Length = 588},
                    new StationType() {Name = "Buffer", Length = 1200},
                    new StationType() {Name = "Robot", Length = 588}
                });
            context.SaveChanges();
            var stationType = context.StationTypes.FirstOrDefault(x => x.Name == "Worker");
            if (stationType == null) throw new InvalidOperationException("Station Type 'Worker' does not exist.");

            #endregion

            #region Station (will create only two for now)

            context.Stations.AddOrUpdate(x => new { x.Name }, new Station[]
            {
                new Station() {Name = "Station 1", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                new Station() {Name = "Station 2", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                new Station() {Name = "Station 3", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                new Station() {Name = "Station 4", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 5", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 6", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 7", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 8", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 9", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 10", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 11", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 12", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 13", SectionId = sectionOne.Id, StationTypeId = stationType.Id},
                //new Station(){Name = "Station 14", SectionId = sectionOne.Id, StationTypeId = stationType.Id}
            });
            context.SaveChanges();
            var stationNames = new[]
            {
                "Station 1", "Station 2", "Station 3", "Station 4"
                //, "Station 5", "Station 6"
                //, "Station 7", "Station 8",
                //"Station 9", "Station 10", "Station 11", "Station 12", "Station 13", "Station 14"
            };
            var stations = context.Stations.ToList();
            // now we will link them together
            stations.First(x => x.Name == stationNames[0]).Predecessor = null;
            for (var i = 1; i < stationNames.Length; i++)
            {
                var stationToUpdate = stations.First(x => x.Name == stationNames[i]);
                var stationPredecessor = stations.First(x => x.Name == stationNames[i - 1]);
                stationToUpdate.Predecessor = stationPredecessor;
            }
            context.SaveChanges();

            #endregion

            #region WorkStation

            int j;

            for (var i = 0; i < stations.Count; i++)
            {
                j = 1;

                // now assign workstations to each station
                context.WorkStations.AddOrUpdate(x => new { x.Name, x.StationId }, new Workstation[]
                {
                    new Workstation()
                    {
                        Name = "Workstation " + (j++),
                        Position = 0,
                        Side = "left",
                        Type = "human",
                        StationId = stations[i].Id
                    },
                    new Workstation()
                    {
                        Name = "Workstation " + (j++),
                        Position = 0,
                        Side = "right",
                        Type = "human",
                        StationId = stations[i].Id
                    },
                });
            }
            context.SaveChanges();

            #endregion

            #endregion

            #region Planning

            #region Process Plan (or is it Work Plan?)

            context.ProcessPlans.AddOrUpdate(x => new { x.Name }, new ProcessPlan()
            {
                Name = "Erster Arbeitsplan / Prozessplan",
                CurrentVersion = 1,
                FactoryId = factory.Id
            });
            context.SaveChanges();
            var processPlan = context.ProcessPlans.FirstOrDefault(x => x.Name == "Erster Arbeitsplan / Prozessplan");
            if (processPlan == null) throw new InvalidOperationException("Process plan does not exist.");

            #endregion

            #region Parts

            context.Parts.AddOrUpdate(x => new { x.Number }, new Part[]
            {
                new Part() {Number = "8T0823359", Name = "Gasfeder", Description = "Gasfeder"},
                new Part() {Number = "8T8833707F", Name = "Türdichtung", Description = "Türdichtung"},
                new Part() {Number = "8K5853345D", Name = "Dachzierleiste", Description = "Dachzierleiste"},
                new Part() {Number = "8K5857807AJ", Name = "Sicherheitsgurt", Description = "Sicherheitsgurt"},

                //new Part() {Number = "8T0035412D", Name = "Subwoofer", Description = "Subwoofer"},
                //new Part() {Number = "8K5877203C", Name = "Wasserablaufschlauch", Description = "Wasserablaufschlauch"},
                //new Part() {Number = "8K0857791A", Name = "Gurtführung", Description = "Gurtführung"},
                //new Part() {Number = "8K0907063DE", Name = "Body Computer 1", Description = "Body Computer 1"},
                //new Part() {Number = "8T8971849", Name = "Kabelhalter", Description = "Kabelhalter"},
                //new Part() {Number = "8K0863335B", Name = "Dämpfung", Description = "Dämpfung"},
                //new Part() {Number = "8T8945097", Name = "Bremsleuchte", Description = "Bremsleuchte"},
                //new Part() {Number = "4G0906093H", Name = "Strg Kraftstoffpumpe", Description = "Strg Kraftstoffpumpe"},
                //new Part() {Number = "8K0857833C", Name = "Gurthöhenverstellung", Description = "Gurthöhenverstellung"},

//	new Part(){ Number="M0000001", Name = "Engine", Description = "Motor"},
//	new Part(){ Number="S0000001", Name = "Seats", Description = "Sitze"},
//	new Part(){ Number="G0000001", Name = "Gear", Description = "Getriebe"},
//	new Part(){ Number="L0000001", Name = "Steering", Description = "Lenkrad"},
//	new Part(){ Number="M1000001", Name = "Center", Description = "Mittelkonsole"},
//	new Part(){ Number="F0000001", Name = "Chassis", Description = "Fahrwerk"},
//	new Part(){ Number="A0000001", Name = "Instruments", Description = "Armatur"},
//	new Part(){ Number="A2000001", Name = "Airbag", Description = "Airbag"},
//	new Part(){ Number="T0000001", Name = "Doors", Description = "Türen"},
//	new Part(){ Number="R1000001", Name = "Radio", Description = "Radio"},

            });
            context.SaveChanges();

            #endregion

            #region Tasks

            context.Tasks.AddOrUpdate(x => new { x.Name }, new Task[]
            {
                new Task()
                {
                    Name = "Pick",
                    Action = Models.Planning.Action.Pick,
                    NeedsEquipment = true,
                    ShowInTerminal = true
                },
                new Task()
                {
                    Name = "Scan",
                    Action = Models.Planning.Action.Scan,
                    NeedsEquipment = true,
                    ShowInTerminal = true
                },
                new Task() {Name = "Mount", Action = Models.Planning.Action.Mount, ShowInTerminal = true},
                new Task()
                {
                    Name = "Screw",
                    Action = Models.Planning.Action.Screw,
                    NeedsEquipment = true,
                    ShowInTerminal = true
                },
                new Task() {Name = "Clip", Action = Models.Planning.Action.Clip, ShowInTerminal = true},
                new Task() {Name = "Check", Action = Models.Planning.Action.Review, ShowInTerminal = true},
                new Task() {Name = "Wait", Action = Models.Planning.Action.DoNothing, ShowInTerminal = false},
                new Task() {Name = "Walk", Action = Models.Planning.Action.Go, ShowInTerminal = false}
            });
            context.SaveChanges();

            #endregion

            #region Manufacturing Model

            context.ManufacturingModels.AddOrUpdate(x => new { x.Name }, new ManufacturingModel[]
            {
                new ManufacturingModel() {Name = "Audi A3", Code = "A3", Family = "A3"}
            });
            context.SaveChanges();
            var a3Model = context.ManufacturingModels.FirstOrDefault(x => x.Name == "Audi A3");
            if (a3Model == null) throw new InvalidOperationException("Audi A3 does not exist");

            #endregion

            #region Task Assignments

            var tasks = context.Tasks.ToList();
            var parts = context.Parts.ToList();
            if (!context.TaskAssignments.Any())
            {
                var assignments = new TaskAssignment[]
                {
                    new TaskAssignment()
                    {
                        Name = "Pick Gasfeder",
                        Part = parts.FirstOrDefault(x => x.Name == "Gasfeder"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Mount Gasfeder",
                        Part = parts.FirstOrDefault(x => x.Name == "Gasfeder"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Mount"),
                        Time = TimeSpan.FromSeconds(30)
                    },
/*
                    new TaskAssignment()
                    {
                        Name = "Pick Kabelhalter",
                        Part = parts.FirstOrDefault(x => x.Name == "Kabelhalter"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(15)
                    },
                    new TaskAssignment()
                    {
                        Name = "Mount Kabelhalter",
                        Part = parts.FirstOrDefault(x => x.Name == "Kabelhalter"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Mount"),
                        Time = TimeSpan.FromSeconds(20)
                    },

                    new TaskAssignment()
                    {
                        Name = "Pick Dämpfung",
                        Part = parts.FirstOrDefault(x => x.Name == "Dämpfung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(15)
                    },
                    new TaskAssignment()
                    {
                        Name = "Mount Dämpfung",
                        Part = parts.FirstOrDefault(x => x.Name == "Dämpfung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Mount"),
                        Time = TimeSpan.FromSeconds(25)
                    },
*/
                    new TaskAssignment()
                    {
                        Name = "Pick Türdichtung",
                        Part = parts.FirstOrDefault(x => x.Name == "Türdichtung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Mount Türdichtung",
                        Part = parts.FirstOrDefault(x => x.Name == "Türdichtung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Mount"),
                        Time = TimeSpan.FromSeconds(35)
                    },
/*
                    new TaskAssignment()
                    {
                        Name = "Pick Wasserablaufschlauch",
                        Part = parts.FirstOrDefault(x => x.Name == "Wasserablaufschlauch"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Scan Wasserablaufschlauch",
                        Part = parts.FirstOrDefault(x => x.Name == "Wasserablaufschlauch"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Scan"),
                        Time = TimeSpan.FromSeconds(5)
                    },
                    new TaskAssignment()
                    {
                        Name = "Clip Wasserablaufschlauch",
                        Part = parts.FirstOrDefault(x => x.Name == "Wasserablaufschlauch"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Clip"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment() {
                        Name = "Check Wasserablaufschlauch",
                        Part = parts.FirstOrDefault(x => x.Name == "Wasserablaufschlauch"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Check"),
                        Time = TimeSpan.FromSeconds(5)
                    },
*/
                    new TaskAssignment()
                    {
                        Name = "Pick Dachzierleiste",
                        Part = parts.FirstOrDefault(x => x.Name == "Dachzierleiste"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Mount Dachzierleiste",
                        Part = parts.FirstOrDefault(x => x.Name == "Dachzierleiste"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Mount"),
                        Time = TimeSpan.FromSeconds(35)
                    },
/*
                    new TaskAssignment()
                    {
                        Name = "Pick Gurtführung",
                        Part = parts.FirstOrDefault(x => x.Name == "Gurtführung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(20)
                    },
                    new TaskAssignment()
                    {
                        Name = "Screw Gurtführung",
                        Part = parts.FirstOrDefault(x => x.Name == "Gurtführung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Screw"),
                        Time = TimeSpan.FromSeconds(30)
                    },
                    new TaskAssignment()
                    {
                        Name = "Check Gurtführung",
                        Part = parts.FirstOrDefault(x => x.Name == "Gurtführung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Check"),
                        Time = TimeSpan.FromSeconds(10)
                    },

                    new TaskAssignment()
                    {
                        Name = "Pick Body Computer 1",
                        Part = parts.FirstOrDefault(x => x.Name == "Body Computer 1"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(15)
                    },
                    new TaskAssignment()
                    {
                        Name = "Screw Body Computer 1",
                        Part = parts.FirstOrDefault(x => x.Name == "Body Computer 1"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Screw"),
                        Time = TimeSpan.FromSeconds(35)
                    },
*/
                    new TaskAssignment()
                    {
                        Name = "Pick Sicherheitsgurt",
                        Part = parts.FirstOrDefault(x => x.Name == "Sicherheitsgurt"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(15)
                    },
                    new TaskAssignment()
                    {
                        Name = "Scan Sicherheitsgurt",
                        Part = parts.FirstOrDefault(x => x.Name == "Sicherheitsgurt"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Scan"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Screw Sicherheitsgurt",
                        Part = parts.FirstOrDefault(x => x.Name == "Sicherheitsgurt"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Screw"),
                        Time = TimeSpan.FromSeconds(25)
                    },
                    new TaskAssignment()
                    {
                        Name = "Check Sicherheitsgurt",
                        Part = parts.FirstOrDefault(x => x.Name == "Sicherheitsgurt"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Check"),
                        Time = TimeSpan.FromSeconds(15)
                    },
/*
                    new TaskAssignment()
                    {
                        Name = "Pick Subwoofer",
                        Part = parts.FirstOrDefault(x => x.Name == "Subwoofer"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Screw Subwoofer",
                        Part = parts.FirstOrDefault(x => x.Name == "Subwoofer"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Screw"),
                        Time = TimeSpan.FromSeconds(25)
                    },
                    new TaskAssignment()
                    {
                        Name = "Check Subwoofer",
                        Part = parts.FirstOrDefault(x => x.Name == "Subwoofer"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Check"),
                        Time = TimeSpan.FromSeconds(15)
                    },

                    new TaskAssignment()
                    {
                        Name = "Pick Bremsleuchte",
                        Part = parts.FirstOrDefault(x => x.Name == "Bremsleuchte"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Clip Bremsleuchte",
                        Part = parts.FirstOrDefault(x => x.Name == "Bremsleuchte"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Clip"),
                        Time = TimeSpan.FromSeconds(15)
                    },

                    new TaskAssignment()
                    {
                        Name = "Pick Strg Kraftstoffpumpe",
                        Part = parts.FirstOrDefault(x => x.Name == "Strg Kraftstoffpumpe"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Screw Strg Kraftstoffpumpe",
                        Part = parts.FirstOrDefault(x => x.Name == "Strg Kraftstoffpumpe"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Screw"),
                        Time = TimeSpan.FromSeconds(30)
                    },
                    new TaskAssignment()
                    {
                        Name = "Check Strg Kraftstoffpumpe",
                        Part = parts.FirstOrDefault(x => x.Name == "Strg Kraftstoffpumpe"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Check"),
                        Time = TimeSpan.FromSeconds(10)
                    },

                    new TaskAssignment()
                    {
                        Name = "Pick Gurthöhenverstellung",
                        Part = parts.FirstOrDefault(x => x.Name == "Gurthöhenverstellung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Pick"),
                        Time = TimeSpan.FromSeconds(10)
                    },
                    new TaskAssignment()
                    {
                        Name = "Screw Gurthöhenverstellung",
                        Part = parts.FirstOrDefault(x => x.Name == "Gurthöhenverstellung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Screw"),
                        Time = TimeSpan.FromSeconds(15)
                    },
                    new TaskAssignment()
                    {
                        Name = "Check Gurthöhenverstellung",
                        Part = parts.FirstOrDefault(x => x.Name == "Gurthöhenverstellung"),
                        Task = tasks.FirstOrDefault(x => x.Name == "Check"),
                        Time = TimeSpan.FromSeconds(5)
                    },
*/
                };

                // go through assignments in order and set predecessor
                var currentPart = "";
                TaskAssignment predecessor = null;
                for (var i = 0; i < assignments.Length; i++)
                {
                    if (currentPart != assignments[i].Part.Number)
                    {
                        currentPart = assignments[i].Part.Number;
                        predecessor = assignments[i];
                        continue;
                    }
                    assignments[i].Predecessor = predecessor;
                    predecessor = assignments[i];
                }
                // now add them to taskassignments
                context.TaskAssignments.AddRange(assignments);
                context.SaveChanges();
            }


            #endregion

            #endregion

            #endregion
        }
    }
}
