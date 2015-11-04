using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.ExecutionEngine.Models;
using MongoRepository;

namespace NextLAP.IP1.ExecutionEngine.Repositories
{
    internal static class MongoRepositories
    {
        private static readonly Lazy<MongoRepository<EngineSettings>> _engineSettings =
            new Lazy<MongoRepository<EngineSettings>>(() => new MongoRepository<EngineSettings>());

        private static readonly Lazy<MongoRepository<WorkstationTaskConfiguration>>
            _workstationTaskConfigurationRepository =
                new Lazy<MongoRepository<WorkstationTaskConfiguration>>(
                    () => new MongoRepository<WorkstationTaskConfiguration>());

        private static readonly Lazy<MongoRepository<EquipmentConfiguration>>
            _equipmentConfigurationRepository =
                new Lazy<MongoRepository<EquipmentConfiguration>>(
                    () => new MongoRepository<EquipmentConfiguration>());

        private static readonly Lazy<MongoRepository<CurrentWorkstationTaskProgress>>
            _workstationTaskProgressRepository =
                new Lazy<MongoRepository<CurrentWorkstationTaskProgress>>(
                    () => new MongoRepository<CurrentWorkstationTaskProgress>());

        private static readonly Lazy<MongoRepository<ManufacturingOrder>> _manufacturingOrderRepository =
            new Lazy<MongoRepository<ManufacturingOrder>>(() => new MongoRepository<ManufacturingOrder>());
        private static readonly Lazy<MongoRepository<StationConfiguration>> _stationRepository =
            new Lazy<MongoRepository<StationConfiguration>>(() => new MongoRepository<StationConfiguration>());
        private static readonly Lazy<MongoRepository<WorkstationConfiguration>> _workstationRepository =
            new Lazy<MongoRepository<WorkstationConfiguration>>(() => new MongoRepository<WorkstationConfiguration>());

        public static MongoRepository<EngineSettings> EngineSettings { get { return _engineSettings.Value; } }
        public static MongoRepository<WorkstationTaskConfiguration> WorkstationConfigurations { get { return _workstationTaskConfigurationRepository.Value; } }
        public static MongoRepository<EquipmentConfiguration> EquipmentConfigurations { get { return _equipmentConfigurationRepository.Value; } }
        public static MongoRepository<CurrentWorkstationTaskProgress> WorkstationProgress { get { return _workstationTaskProgressRepository.Value; } }
        public static MongoRepository<ManufacturingOrder> ManufacturingOrders { get { return _manufacturingOrderRepository.Value; } }
        public static MongoRepository<StationConfiguration> Stations { get { return _stationRepository.Value; } }
        public static MongoRepository<WorkstationConfiguration> Workstations { get { return _workstationRepository.Value; } } 
    }
}
