using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public sealed class Ip1Repositories : IDisposable
    {
        private readonly Lazy<CompanyRepository> _companyRepository;
        private readonly Lazy<FactoryRepository> _factoryRepository;
        private readonly Lazy<AssemblyLineRepository> _assemblyLineRepository;
        private readonly Lazy<SectionRepository> _sectionRepository;
        private readonly Lazy<StationRepository> _stationRepository;
        private readonly Lazy<StationTypeRepository> _stationTypeRepository;
        private readonly Lazy<WorkstationRepository> _workstationRepository;
        private readonly Lazy<TaskRepository> _taskRepository;
        private readonly Lazy<PartRepository> _partRepository;
        private readonly Lazy<TaskAssignmentRepository> _taskAssignmentRepository;
        private readonly Lazy<TaskAssignmentImageRepository> _taskAssignmentImageRepository;
        private readonly Lazy<StationTaskAssignmentRepository> _stationTaskAssignmentRepository;
        private readonly Lazy<ProcessPlanRepository> _processPlanRepository;
        private readonly Lazy<EquipmentTypeRepository> _equipmentTypeRepository;
        private readonly Lazy<EquipmentConfigurationRepository> _equipmentConfigurationRepository;
        private readonly Lazy<EquipmentDriverRepository> _equipmentDriverRepository;
        private readonly Lazy<EquipmentDriverConfigurationRepository> _equipmentDriverConfigurationRepository;
        private readonly Lazy<EquipmentDriverConfigurationValuesRepository> _equipmentDriverConfigurationValuesRepository;
        private readonly Lazy<WorkstationTerminalRepository> _workstationTerminalRepository;
        private readonly Lazy<OrderRepository> _orderRepository;
        private readonly Lazy<OrderPartItemRepository> _orderPartItemRepository;

        public Ip1Repositories(Ip1Context context)
        {
            _companyRepository = new Lazy<CompanyRepository>(() => new CompanyRepository(context));
            _factoryRepository = new Lazy<FactoryRepository>(() => new FactoryRepository(context));
            _assemblyLineRepository = new Lazy<AssemblyLineRepository>(() => new AssemblyLineRepository(context));
            _sectionRepository = new Lazy<SectionRepository>(() => new SectionRepository(context));
            _stationRepository = new Lazy<StationRepository>(() => new StationRepository(context));
            _stationTypeRepository = new Lazy<StationTypeRepository>(() => new StationTypeRepository(context));
            _workstationRepository = new Lazy<WorkstationRepository>(() => new WorkstationRepository(context));
            _taskRepository = new Lazy<TaskRepository>(() => new TaskRepository(context));
            _partRepository = new Lazy<PartRepository>(() => new PartRepository(context));
            _taskAssignmentRepository = new Lazy<TaskAssignmentRepository>(() => new TaskAssignmentRepository(context));
            _taskAssignmentImageRepository = new Lazy<TaskAssignmentImageRepository>(() => new TaskAssignmentImageRepository(context));
            _stationTaskAssignmentRepository = new Lazy<StationTaskAssignmentRepository>(() => new StationTaskAssignmentRepository(context));
            _processPlanRepository = new Lazy<ProcessPlanRepository>(() => new ProcessPlanRepository(context));
            _equipmentTypeRepository = new Lazy<EquipmentTypeRepository>(() => new EquipmentTypeRepository(context));
            _equipmentConfigurationRepository = new Lazy<EquipmentConfigurationRepository>(() => new EquipmentConfigurationRepository(context));
            _equipmentDriverRepository = new Lazy<EquipmentDriverRepository>(() => new EquipmentDriverRepository(context));
            _equipmentDriverConfigurationRepository = new Lazy<EquipmentDriverConfigurationRepository>(() => new EquipmentDriverConfigurationRepository(context));
            _equipmentDriverConfigurationValuesRepository = new Lazy<EquipmentDriverConfigurationValuesRepository>(() => new EquipmentDriverConfigurationValuesRepository(context));
            _workstationTerminalRepository = new Lazy<WorkstationTerminalRepository>(() => new WorkstationTerminalRepository(context));
            _orderRepository = new Lazy<OrderRepository>(() => new OrderRepository(context));
            _orderPartItemRepository = new Lazy<OrderPartItemRepository>(() => new OrderPartItemRepository(context));
        }

        public CompanyRepository CompanyRepository { get { return _companyRepository.Value; } }
        public FactoryRepository FactoryRepository { get { return _factoryRepository.Value; } }
        public AssemblyLineRepository AssemblyLineRepository { get { return _assemblyLineRepository.Value; } }
        public SectionRepository SectionRepository { get { return _sectionRepository.Value; } }
        public StationRepository StationRepository { get { return _stationRepository.Value; } }
        public StationTypeRepository StationTypeRepository { get { return _stationTypeRepository.Value; } }
        public WorkstationRepository WorkstationRepository { get { return _workstationRepository.Value; } }
        public TaskRepository TaskRepository { get { return _taskRepository.Value; } }
        public PartRepository PartRepository { get { return _partRepository.Value; } }
        public TaskAssignmentRepository TaskAssignmentRepository { get { return _taskAssignmentRepository.Value; } }
        public TaskAssignmentImageRepository TaskAssignmentImageRepository { get { return _taskAssignmentImageRepository.Value; } }
        public StationTaskAssignmentRepository StationTaskAssignmentRepository { get { return _stationTaskAssignmentRepository.Value; } }
        public ProcessPlanRepository ProcessPlanRepository { get { return _processPlanRepository.Value; } }
        public EquipmentTypeRepository EquipmentTypeRepository { get { return _equipmentTypeRepository.Value; } }
        public EquipmentConfigurationRepository EquipmentConfigurationRepository { get { return _equipmentConfigurationRepository.Value; } }
        public EquipmentDriverRepository EquipmentDriverRepository { get { return _equipmentDriverRepository.Value; } }
        public EquipmentDriverConfigurationRepository EquipmentDriverConfigurationRepository { get { return _equipmentDriverConfigurationRepository.Value; } }
        public EquipmentDriverConfigurationValuesRepository EquipmentDriverConfigurationValuesRepository { get { return _equipmentDriverConfigurationValuesRepository.Value; } }
        public WorkstationTerminalRepository WorkstationTerminalRepository { get { return _workstationTerminalRepository.Value; } }
        public OrderRepository OrderRepository { get { return _orderRepository.Value; } }
        public OrderPartItemRepository OrderPartItemRepository { get { return _orderPartItemRepository.Value; } }
        public void Dispose()
        {
            if (_companyRepository.IsValueCreated) _companyRepository.Value.Dispose();
            if (_factoryRepository.IsValueCreated) _factoryRepository.Value.Dispose();
            if (_assemblyLineRepository.IsValueCreated) _assemblyLineRepository.Value.Dispose();
            if (_sectionRepository.IsValueCreated) _sectionRepository.Value.Dispose();
            if (_stationRepository.IsValueCreated) _stationRepository.Value.Dispose();
            if (_stationTypeRepository.IsValueCreated) _stationTypeRepository.Value.Dispose();
            if (_workstationRepository.IsValueCreated) _workstationRepository.Value.Dispose();
            if (_taskRepository.IsValueCreated) _taskRepository.Value.Dispose();
            if (_partRepository.IsValueCreated) _partRepository.Value.Dispose();
            if (_taskAssignmentRepository.IsValueCreated) _taskAssignmentRepository.Value.Dispose();
            if (_taskAssignmentImageRepository.IsValueCreated) _taskAssignmentImageRepository.Value.Dispose();
            if (_stationTaskAssignmentRepository.IsValueCreated) _stationTaskAssignmentRepository.Value.Dispose();
            if (_processPlanRepository.IsValueCreated) _processPlanRepository.Value.Dispose();
            if (_equipmentTypeRepository.IsValueCreated) _equipmentTypeRepository.Value.Dispose();
            if (_equipmentConfigurationRepository.IsValueCreated) _equipmentConfigurationRepository.Value.Dispose();
            if (_equipmentDriverRepository.IsValueCreated) _equipmentDriverRepository.Value.Dispose();
            if (_equipmentDriverConfigurationRepository.IsValueCreated) _equipmentDriverConfigurationRepository.Value.Dispose();
            if (_equipmentDriverConfigurationValuesRepository.IsValueCreated) _equipmentDriverConfigurationValuesRepository.Value.Dispose();
            if (_workstationTerminalRepository.IsValueCreated) _workstationTerminalRepository.Value.Dispose();
            if (_orderRepository.IsValueCreated) _orderRepository.Value.Dispose();
            if (_orderPartItemRepository.IsValueCreated) _orderPartItemRepository.Value.Dispose();
        }
    }
}
