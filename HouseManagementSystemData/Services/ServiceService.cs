using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
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
        ServiceViewModel GetById(int id);
        Task<ResultResponse> CreateServiceAsync(CreateServiceViewModel model);
        Task<ResultResponse> UpdateServiceAsync(UpdateServiceViewModel model);
        Task<ResultResponse> DeleteServiceAsync(int serviceId);
        Task<ResultResponse> CreateDefaultServicesAsync(string houseId);
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

        public async Task<ResultResponse> CreateServiceAsync(CreateServiceViewModel model)
        {
            var serviceTypes = await _serviceTypeService.GetServiceTypes();
            var service = _mapper.Map<Service>(model);
            service.Status = ServiceConstants.SERVICE_IS_ACTIVE;
            service.ServiceTypeId = serviceTypes.Where(serviceType => serviceType.Name.Equals(model.ServiceTypeName)).FirstOrDefault().Id;
            await CreateAsyn(service);
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Service" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> DeleteServiceAsync(int serviceId)
        {

            var serviceModel = GetById(serviceId);
            if (serviceModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Service" }).Value,
                    IsSuccess = false
                };
            }
            var service = await GetAsyn(serviceId);
            service.Status = ServiceConstants.SERVICE_IS_INACTIVE;
            Update(service);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "Service" }).Value,
                IsSuccess = true
            };
           
        }

        public List<ServiceViewModel> GetByHouseId(string houseId)
        {
            List<ServiceViewModel> services;
            services = Get().Where(s => s.HouseId == houseId && s.Status == ServiceConstants.SERVICE_IS_ACTIVE).ProjectTo<ServiceViewModel>(_mapper.ConfigurationProvider).ToList();
            return services;
        }

        public ServiceViewModel GetById(int id)
        {
            var service = Get().Where(s => s.Id == id && s.Status == ServiceConstants.SERVICE_IS_ACTIVE).ProjectTo<ServiceViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return service;
        }

        public async Task<ResultResponse> UpdateServiceAsync(UpdateServiceViewModel model)
        {
            var serviceTypes = await _serviceTypeService.GetServiceTypes();
            var serviceId = model.Id;
            var serviceModel = GetById(serviceId);
            if (serviceModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Service" }).Value,
                    IsSuccess = false
                };
            }
            var service = await GetAsyn(model.Id);
            if (model.Name != null)
            {
                service.Name = model.Name;
            }

            if (model.CalculationUnit != null)
            {
                service.CalculationUnit = model.CalculationUnit;
            }

            if (model.Price != null)
            {
                service.Price = model.Price;
            }

            if (model.ServiceType != null)
            {
                service.ServiceTypeId = serviceTypes.Where(serviceType => serviceType.Name.Equals(model.ServiceType)).FirstOrDefault().Id;
          
            }

            Update(service);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Service" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> CreateDefaultServicesAsync(string houseId)
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
            foreach (CreateServiceViewModel model in defaultServices)
            {
                await CreateServiceAsync(model);
            }
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Default Services" }).Value,
                IsSuccess = true,
            };
        }
    }
}
