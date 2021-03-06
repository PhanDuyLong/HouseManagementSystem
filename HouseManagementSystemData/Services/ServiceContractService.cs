using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.ServiceContract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HMS.Data.Services
{
    public partial interface IServiceContractService : IBaseService<ServiceContract>
    {
        List<ServiceContractDetailViewModel> GetByContractId(int contractId);
        ServiceContractDetailViewModel GetById(int id);
        Task<ResultResponse> UpdateServiceContractAsync(int roomId, int contractId, UpdateServiceContractViewModel model);
        Task<ResultResponse> DeleteServiceContractAsync(int serviceContractId);
        Task<ResultResponse> CreateServiceContractAsync(int roomId, int contractId, CreateServiceContractViewModel model);
        Task<ResultResponse> CreateServiceContractsAsync(int roomId, int contractId, List<CreateServiceContractViewModel> model);
        Task<ResultResponse> UpdateServiceContractsAsync(int roomId, int contractId, List<UpdateServiceContractViewModel> updateServiceContracts);
    }
    public partial class ServiceContractService : BaseService<ServiceContract>, IServiceContractService
    {
        private readonly IMapper _mapper;
        private readonly IServiceService _serviceService;
        private readonly IClockService _clockService;
        private readonly IClockValueService _clockValueService;
        public ServiceContractService(DbContext dbContext, IServiceContractRepository repository, IMapper mapper
            , IServiceService serviceService, IClockService clockService, IClockValueService clockValueService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _serviceService = serviceService;
            _clockService = clockService;
            _clockValueService = clockValueService;
        }

        public async Task<ResultResponse> CreateServiceContractAsync(int roomId, int contractId, CreateServiceContractViewModel model)
        {
            var check = _serviceService.CheckService(model.ServiceId);
            if (!check.IsSuccess)
            {
                return check;
            }
            var serviceContract = _mapper.Map<ServiceContract>(model);

            var service = _serviceService.GetById(model.ServiceId);
            if (service.ServiceType == ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_DIFFERENT)
            {
                var clockId = _clockService.GetIdByServiceIdAndRoomId(service.Id, roomId);
                var createClockValueViewModel = new CreateClockValueViewModel
                {
                    ClockId = clockId,
                    IndexValue = model.StartClockValue,
                    CreateDate = DateTime.Now,
                    RecordDate = DateTime.Now
                };
                await _clockValueService.CreateClockValueAsync(createClockValueViewModel);
                serviceContract.ClockId = clockId;
            }

            serviceContract.ContractId = contractId;
            serviceContract.Status = ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE;
            await CreateAsyn(serviceContract);

            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "ServiceContract " }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> CreateServiceContractsAsync(int roomId, int contractId, List<CreateServiceContractViewModel> models)
        {
            foreach (var model in models)
            {
                var check = await CreateServiceContractAsync(roomId, contractId, model);
                if (!check.IsSuccess)
                    return check;
            }

            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "ServiceContracts" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> DeleteServiceContractAsync(int serviceContractId)
        {
            var serviceContractModel = GetById(serviceContractId);
            if (serviceContractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "ServiceContract" }).Value,
                    IsSuccess = false,
                };
            }
            var serviceContract = await GetAsyn(serviceContractId);
            serviceContract.Status = ServiceContractConstants.SERVICE_CONTRACT_IS_INACTIVE;
            Update(serviceContract);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "ServiceContract" }).Value,
                IsSuccess = true
            };
        }

        public List<ServiceContractDetailViewModel> GetByContractId(int contractId)
        {
            List<ServiceContractDetailViewModel> contract;
            contract = Get().Where(sc => sc.ContractId == contractId && sc.Status == ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE).ProjectTo<ServiceContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return contract;
        }

        public ServiceContractDetailViewModel GetById(int id)
        {
            var serviceContract = Get().Where(sc => sc.Id == id && sc.Status == ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE).ProjectTo<ServiceContractDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return serviceContract;
        }

        public async Task<ResultResponse> UpdateServiceContractAsync(int roomId, int contractId, UpdateServiceContractViewModel model)
        {
            var serviceContractId = model.Id;
            var serviceContractModel = GetById(serviceContractId.Value);
            if (serviceContractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "ServiceContract" }).Value,
                    IsSuccess = false
                };
            }
            var serviceContract = GetById(model.Id.Value);
            var service = serviceContract.Service;
            if (service.ServiceType == ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_DIFFERENT)
            {
                var clockId = _clockService.GetIdByServiceIdAndRoomId(service.Id, roomId);
                var createClockValueViewModel = new CreateClockValueViewModel
                {
                    ClockId = clockId,
                    IndexValue = model.StartClockValue,
                    CreateDate = DateTime.Now,
                    RecordDate = DateTime.Now
                };
                await _clockValueService.CreateClockValueAsync(createClockValueViewModel);
            }
            var sContract = await GetAsyn(model.Id.Value);
            if (model.UnitPrice != null)
            {
                sContract.UnitPrice = model.UnitPrice.Value;
            }

            sContract.Status = ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE;
            Update(sContract);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "ServiceContract" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> UpdateServiceContractsAsync(int roomId, int contractId, List<UpdateServiceContractViewModel> updateServiceContracts)
        {
            ResultResponse check;
            var oldServiceContracts = GetByContractId(contractId);
            foreach(var oldServiceContract in oldServiceContracts)
            {
                bool find = false;
                foreach(var serviceContract in updateServiceContracts)
                {
                    if (oldServiceContract.ServiceId.Equals(serviceContract.ServiceId))
                    {
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    check = await DeleteServiceContractAsync(oldServiceContract.Id);
                    if (!check.IsSuccess)
                    {
                        return check;
                    }
                }
            }
            foreach (var serviceContract in updateServiceContracts)
            {
                bool find = false;
                int value = 50; 

                foreach(var oldServiceContract in oldServiceContracts)
                {
                    if (oldServiceContract.ServiceId.Equals(serviceContract.ServiceId))
                    {
                        find = true;
                        serviceContract.Id = oldServiceContract.Id;
                        break;
                    }
                }
                if (find)
                {
                    serviceContract.StartClockValue = value;
                   check = await UpdateServiceContractAsync(roomId, contractId, serviceContract);
                }
                else
                {
                    var createModel = new CreateServiceContractViewModel
                    {
                        ServiceId = serviceContract.ServiceId,
                        StartClockValue = serviceContract.StartClockValue.Value,
                        UnitPrice = serviceContract.UnitPrice.Value
                    };
                    check = await CreateServiceContractAsync(roomId, contractId, createModel);

                }
                if (!check.IsSuccess)
                    return check;
            }

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "ServiceContracts" }).Value,
                IsSuccess = true,
            };
        }
    }


}

