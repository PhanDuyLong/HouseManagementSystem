using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Parameters;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Bill;
using HMS.Data.ViewModels.BillItem;
using HMS.Data.ViewModels.Clock;
using HMS.Data.ViewModels.Contract.Base;
using HMS.Data.ViewModels.ServiceContract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IBillService : IBaseService<Bill>
    {
        List<ShowBillViewModel> FilterByParameter(string userId, BillParameters billParameters);
        BillDetailViewModel GetById(int id);
        Task<BillDetailViewModel> CreateBill(CreateBillViewModel model);
        BillDetailViewModel UpdateBill(Bill bill, UpdateBillViewModel model);
        string DeleteBill(Bill bill);
    }
    public partial class BillService : BaseService<Bill>, IBillService
    {
        private readonly IMapper _mapper;
        private readonly IContractService _contractService;
        private readonly IClockValueService _clockValueService;
        private readonly IBillItemService _billItemService;
        private readonly IClockService _clockService;
        private readonly IAccountService _accountService;
        public BillService(DbContext dbContext, IBillRepository repository, IMapper mapper,
            IContractService contractService, IClockValueService clockValueService, IBillItemService billItemService, IClockService clockService, IAccountService accountService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contractService = contractService;
            _clockValueService = clockValueService;
            _billItemService = billItemService;
            _clockService = clockService;
            _accountService = accountService;
        }

        public async Task<BillDetailViewModel> CreateBill(CreateBillViewModel createModel)
        {
            var bill = _mapper.Map<Bill>(createModel);
            bill.Status = BillConstants.BILL_IS_NOT_PAID;
            bill.IsDeleted = BillConstants.BILL_IS_NOT_DELETED;
            bill.IsSent = BillConstants.BILL_IS_NOT_SENT;

            var billItems = new ArrayList();
            var contract = _contractService.GetById(createModel.ContractId);
            var serviceContracts = contract.ServiceContracts.ToList();
            double total = 0;
            foreach (ServiceContractDetailViewModel serviceContract in serviceContracts)
            {
                var item = new BillItem();
                item.ServiceContractId = serviceContract.Id;
                var createBillItem = createModel.CreateBillItems.ToList().Find(item => item.ServiceContractId.Equals(serviceContract.Id));
                if (createBillItem != null)
                {
                    if (ServiceHasClock(serviceContract))
                    {
                        ClockDetailViewModel clock = _clockService.GetById(serviceContract.ClockId);
                        var startClockValue = clock.ClockValues.ToList().Find(cV => cV.Status == true).IndexValue;
                        var endClockValue = createBillItem.EndValue;
                        var totalPrice = (endClockValue - startClockValue) * serviceContract.Service.Price;
                        item.StartValue = startClockValue;
                        item.EndValue = endClockValue;
                        item.TotalPrice = totalPrice;
                        total += totalPrice;
                    }
                    else if (serviceContract.Service.ServiceType.Equals(ServiceTypeConstants.SERVICE_TYPE_IS_ADDITIONAL_DIFFERENT))
                    {
                        var startValue = createBillItem.StartValue;
                        var endValue = createBillItem.EndValue;
                        var totalPrice = (endValue - startValue) * serviceContract.Service.Price;
                        item.StartValue = startValue;
                        item.EndValue = endValue;
                        item.TotalPrice = totalPrice;
                        total += totalPrice;
                    }
                }
                else
                {
                    item.TotalPrice = serviceContract.Service.Price;
                    total += serviceContract.Service.Price;
                }
                item.Status = BillItemConstants.BILL_ITEM_IS_NOT_DELETED;
                billItems.Add(item);
            }

            bill.TotalPrice = total;
            await CreateAsyn(bill);
            foreach (BillItem billItem in billItems)
            {
                billItem.BillId = bill.Id;
                await _billItemService.CreateAsyn(billItem);
            }

            return GetById(bill.Id);
        }

        public string DeleteBill(Bill bill)
        {
            bill.Status = BillConstants.BILL_IS_DELETED;
            Update(bill);
            return "Deleted successfully";
        }

        public List<ShowBillViewModel> FilterByParameter(string userId, BillParameters billParameters)
        {
            var bills = new List<ShowBillViewModel>();
            var contractId = billParameters.ContractId;
            if(contractId != null)
            {
                bills = GetByContractId(billParameters);
            }
            else
            {
                bills = GetByUserId(userId, billParameters);   
            }

            var status = billParameters.Status;
            if(status != null)
            {
                bills = bills.Where(b => b.Status == status).ToList();
            }
            return bills;
        }

        public List<ShowBillViewModel> GetByUserId(string userId, BillParameters billParameters)
        {
            List<ShowBillViewModel> bills;
            List<ContractDetailViewModel> contracts = _contractService.GetByUserId(userId);
            var contractIds = contracts.Select(c => c.Id).ToList();

            if (billParameters.IsIssueDateAscending)
            {
                bills = Get().Where(b => contractIds.Contains(b.ContractId.Value) && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).OrderBy(b => b.IssueDate).ProjectTo<ShowBillViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            else
            {
                bills = Get().Where(b => contractIds.Contains(b.ContractId.Value) && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).OrderByDescending(b => b.IssueDate).ProjectTo<ShowBillViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            return bills;
        }

        public List<ShowBillViewModel> GetByContractId(BillParameters billParameters)
        {
            List<ShowBillViewModel> bills;
            if (billParameters.IsIssueDateAscending)
            {
                bills = Get().Where(b => b.ContractId == billParameters.ContractId && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).OrderBy(b => b.IssueDate).ProjectTo<ShowBillViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            else
            {
                bills = Get().Where(b => b.ContractId == billParameters.ContractId && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).OrderByDescending(b => b.IssueDate).ProjectTo<ShowBillViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            return bills;
        }

        public BillDetailViewModel GetById(int id)
        {
            var bill = Get().Where(b => b.Id == id && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return bill;
        }

        public BillDetailViewModel UpdateBill(Bill bill, UpdateBillViewModel updateModel)
        {
            var newBill = _mapper.Map<Bill>(updateModel);
            Update(newBill);

            var updateBillItems = updateModel.UpdateBillItems;
            var newBillItems = new ArrayList();
            if (updateBillItems.Count != 0)
            {
                var oldBillItems = bill.BillItems.ToList();
                var items = new ArrayList();
                foreach (UpdateBillItemViewModel billItem in updateModel.UpdateBillItems)
                {
                    if (oldBillItems.Exists(item => item.ServiceContractId == billItem.ServiceContractId))
                    {
                        var oldBillItem = oldBillItems.Find(item => item.ServiceContractId == billItem.ServiceContractId);
                        if (billItem.isDeleted)
                        {
                            oldBillItem.Status = BillItemConstants.BILL_ITEM_IS_DELETED;
                        }
                        else
                        {
                            oldBillItem.StartValue = billItem.StartValue;
                            oldBillItem.EndValue = billItem.EndValue;
                        }
                        items.Add(oldBillItem);
                    }
                }

                foreach (BillItem item in items)
                {
                    _billItemService.Update(item);
                }
            }
            return GetById(bill.Id);
        }


        private bool ServiceHasClock(ServiceContractDetailViewModel serviceContract)
        {
            return serviceContract.ClockId != 0;
        }

    }
}
