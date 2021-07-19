using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.BillItem;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IBillItemService : IBaseService<BillItem>
    {
        Task<ResultResponse> CreateBillItemAsync(CreateBillItemViewModel model);
        Task<ResultResponse> UpdateBillItemAsync(UpdateBillItemViewModel model);
        Task<ResultResponse> DeleteBillItemAsync(int billItem);
        Task<ResultResponse> UpdateBillItemsAsync(int billId, List<UpdateBillItemViewModel> models);
        BillItemDetailViewModel GetById(int id);
        List<BillItemDetailViewModel> GetByBillId(int billId);
    }
    public partial class BillItemService : BaseService<BillItem>, IBillItemService
    {
        private readonly IMapper _mapper;
        private readonly IServiceContractService _serviceContractService;
        public BillItemService(DbContext dbContext, IBillItemRepository repository, IMapper mapper
            , IServiceContractService serviceContractService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _serviceContractService = serviceContractService;
        }

        public async Task<ResultResponse> CreateBillItemAsync(CreateBillItemViewModel model)
        {
            var item = _mapper.Map<BillItem>(model);
            item.Status = BillItemConstants.BILL_ITEM_IS_NOT_DELETED;

            var serviceContract = _serviceContractService.GetById(model.ServiceContractId.Value);

            item.TotalPrice = (model.EndValue - model.StartValue) * serviceContract.UnitPrice;

            await CreateAsyn(item);
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Clock" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> DeleteBillItemAsync(int billItem)
        {
            var check = CheckBillItem(billItem);
            if (!check.IsSuccess)
            {
                return check;
            };

            var item = await GetAsyn(billItem);
            item.Status = BillItemConstants.BILL_ITEM_IS_DELETED;
            Update(item);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "BillItem" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> UpdateBillItemAsync(UpdateBillItemViewModel model)
        {
            var check = CheckBillItem(model.Id.Value);
            if (!check.IsSuccess)
            {
                return check;
            };

            var item = await GetAsyn(model.Id);

            if (model.StartValue != null)
            {
                 item.StartValue = model.StartValue;
            }
            if(model.EndValue != null)
            {
                item.EndValue = model.EndValue;
            }

            item.Status = ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE;
            Update(item);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "BillItem" }).Value,
                IsSuccess = true
            };
        }

        public List<BillItemDetailViewModel> GetByBillId(int billId)
        {
            var billItems = Get().Where(b => b.BillId == billId && b.Status == BillItemConstants.BILL_ITEM_IS_NOT_DELETED).ProjectTo<BillItemDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            if(billItems != null && billItems.Count != 0)
            {
                foreach(var billItem in billItems)
                {
                    if (billItem.ServiceContract.Clock != null)
                    {
                        billItem.ServiceContract.Clock.ClockValues = billItem.ServiceContract.Clock.ClockValues.Where(value => value.Status == ClockValueConstants.CLOCK_VALUE_IS_MILESTONE).ToList();
                    }
                }
            }
            return billItems;
        }

        public BillItemDetailViewModel GetById(int id)
        {
            var billItem = Get().Where(b => b.Id == id && b.Status == BillItemConstants.BILL_ITEM_IS_NOT_DELETED).ProjectTo<BillItemDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            if(billItem.ServiceContract.Clock != null)
            {
                billItem.ServiceContract.Clock.ClockValues = billItem.ServiceContract.Clock.ClockValues.Where(value => value.Status == ClockValueConstants.CLOCK_VALUE_IS_MILESTONE).ToList();
            }
            return billItem;
        }

        public async Task<ResultResponse> UpdateBillItemsAsync(int billId, List<UpdateBillItemViewModel> updateModels)
        {
            ResultResponse check;
            var oldBillItems = GetByBillId(billId);
            foreach (var oldBillItem in oldBillItems)
            {
                bool find = false;
                foreach (var billItem in updateModels)
                {
                    if (oldBillItem.Id.Equals(billItem.Id))
                    {
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    oldBillItems.Remove(oldBillItem);
                    check = await DeleteBillItemAsync(oldBillItem.Id);
                    if (!check.IsSuccess)
                    {
                        return check;
                    }
                }
            }
            foreach (var billItem in updateModels)
            {
                bool find = false;
                foreach (var oldBillItem in oldBillItems)
                {
                    if (oldBillItem.Id.Equals(billItem.Id))
                    {
                        find = true;
                        break;
                    }
                }
                if (find)
                {
                    check = await UpdateBillItemAsync(billItem);
                }
                else
                {
                    var createModel = new CreateBillItemViewModel
                    {
                        BillId = billId,
                        EndValue = billItem.EndValue.Value,
                        StartValue = billItem.StartValue.Value,
                        ServiceContractId = billItem.ServiceContractId
                    };
                    check = await CreateBillItemAsync(createModel);

                }
                if (!check.IsSuccess)
                    return check;
            }

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "BillItem" }).Value,
                IsSuccess = true,
            };
        }

        public ResultResponse CheckBillItem(int id)
        {
            var item = GetById(id);
            if (item == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "BillItem" }).Value,
                    IsSuccess = false
                };
            }
            return new ResultResponse
            {
                IsSuccess = true
            };
        }
    }
}
