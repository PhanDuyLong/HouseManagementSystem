using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IServiceService : IBaseService<Service>
    {
        List<ServiceViewModel> GetByHouseId(string houseId);
        ServiceViewModel GetByID(int id);
        Task<ServiceViewModel> CreateService(CreateServiceViewModel model);
        ServiceViewModel UpdateService(Service service, UpdateServiceViewModel model);
        string DeleteService(Service service);
        Task<int> CreateDefaultServicesAsync(string houseId);
    }
    public partial class ServiceService : BaseService<Service>, IServiceService
    {
        private readonly IMapper _mapper;
        private readonly IServiceTypeService _serviceTypeService;
        public ServiceService(DbContext dbContext, IServiceRepository repository, IMapper mapper
            , IServiceTypeService serviceTypeService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _serviceTypeService = serviceTypeService;
        }

        public async Task<ServiceViewModel> CreateService(CreateServiceViewModel model)
        {
            var serviceTypes = await _serviceTypeService.GetServiceTypes();
            var service = _mapper.Map<Service>(model);
            service.Status = ServiceConstants.SERVICE_IS_ACTIVE;
            service.ServiceTypeId = serviceTypes.Where(serviceType => serviceType.Name.Equals(ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_DIFFERENT)).FirstOrDefault().Id;
            await CreateAsyn(service);
            return GetByID(service.Id);
        }

        public string DeleteService(Service service)
        {
            throw new System.NotImplementedException();
        }

        public List<ServiceViewModel> GetByHouseId(string houseId)
        {
            throw new System.NotImplementedException();
        }

        public ServiceViewModel GetByID(int id)
        {
            var service = Get().Where(s => s.Id == id && s.Status == ServiceConstants.SERVICE_IS_ACTIVE).ProjectTo<ServiceViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return service;
        }

        public ServiceViewModel UpdateService(Service service, UpdateServiceViewModel model)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> CreateDefaultServicesAsync(string houseId)
        {
            var defaultServices = new ArrayList();
            var houseService = new CreateServiceViewModel
            {
                HouseId = houseId,
                Name = "Tiền Nhà",
                CalculationUnit = "tháng",
                ServiceTypeName = ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_FIXED
            };
            defaultServices.Add(houseService);
            var eletricService = new CreateServiceViewModel
            {
                HouseId = houseId,
                Name = "Tiền Điện",
                CalculationUnit = "kWH",
                ServiceTypeName = ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_DIFFERENT
            };
            defaultServices.Add(eletricService);
            var waterService = new CreateServiceViewModel
            {
                HouseId = houseId,
                Name = "Tiền Nước",
                CalculationUnit = "m^3",
                ServiceTypeName = ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_DIFFERENT
            };
            defaultServices.Add(waterService);
            int count = 0;
            foreach(CreateServiceViewModel model in defaultServices)
            {
                await CreateService(model);
                count++;
            }
            return count;
        }
    }
}
