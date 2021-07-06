using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Bill;
using HMS.Data.ViewModels.BillItem;
using HMS.Data.ViewModels.Contract.Base;
using HMS.Data.ViewModels.ServiceContract;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IBillService : IBaseService<Bill>
    {
        List<BillDetailViewModel> GetByContractID(int contractId);
        BillDetailViewModel GetByID(int id);
        Task<BillDetailViewModel> CreateBill(CreateBillViewModel model);
        BillDetailViewModel UpdateBill(Bill bill, UpdateBillViewModel model);
        string DeleteBill(Bill bill);
        List<BillDetailViewModel> GetByUsername(string username);
    }
    public partial class BillService : BaseService<Bill>, IBillService
    {
        private readonly IMapper _mapper;
        private readonly IContractService _contractService;
        private readonly IClockValueService _clockValueService;
        private readonly IBillItemService _billItemService;
        public BillService(DbContext dbContext, IBillRepository repository, IMapper mapper,
            IContractService contractService, IClockValueService clockValueService, IBillItemService billItemService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contractService = contractService;
            _clockValueService = clockValueService;
            _billItemService = billItemService;
        }

        public async Task<BillDetailViewModel> CreateBill(CreateBillViewModel createModel)
        {
            var bill = _mapper.Map<Bill>(createModel);
            bill.Status = BillConstants.BILL_IS_NOT_PAID;
            bill.IsDeleted = BillConstants.BILL_IS_NOT_DELETED;
            bill.IsSent = BillConstants.BILL_IS_NOT_SENT;
            await CreateAsyn(bill);

            var billItems = new ArrayList();
            var contract = _contractService.GetByID(createModel.ContractId);
            var serviceContracts = contract.ServiceContracts.ToList();
            foreach (ServiceContractDetailViewModel serviceContract in serviceContracts)
            {
                var item = new BillItem();
                item.BillId = bill.Id;
                await _billItemService.CreateAsyn(item);
                item.ServiceContractId = serviceContract.Id;
                var createBillItem = createModel.CreateBillItems.ToList().Find(item => item.ServiceContractId.Equals(serviceContract.Id));
                if (createBillItem != null)
                {
                    if (ServiceHasClock(serviceContract))
                    {
                        var startClockValue = serviceContract.Clock.ClockValues.ToList().Find(cV => cV.Status == true).IndexValue;
                        var endClockValue = createBillItem.EndValue;
                        var totalPrice = (endClockValue - startClockValue) * serviceContract.Service.Price;
                        item.StartValue = startClockValue;
                        item.EndValue = endClockValue;
                        item.TotalPrice = totalPrice;
                    }
                    else if (serviceContract.Service.ServiceType.Equals(ServiceTypeConstants.SERVICE_TYPE_IS_CL))
                    {
                        var startValue = createBillItem.StartValue;
                        var endValue = createBillItem.EndValue;
                        var totalPrice = (endValue - startValue) * serviceContract.Service.Price;
                        item.StartValue = startValue;
                        item.EndValue = endValue;
                        item.TotalPrice = totalPrice;
                    }
                }
                else
                {
                    item.TotalPrice = serviceContract.Service.Price;
                }
                item.Status = BillItemConstants.BILL_ITEM_IS_NOT_DELETED;
                billItems.Add(item);
            }

            foreach (BillItem billItem in billItems)
            {
                _billItemService.Update(billItem);
            }

            return GetByID(bill.Id);
        }

        public string DeleteBill(Bill bill)
        {
            bill.Status = BillConstants.BILL_IS_DELETED;
            Update(bill);
            return "Deleted successfully";
        }

        public List<BillDetailViewModel> GetByContractID(int contractId)
        {
            var bills = Get().Where(b => b.ContractId == contractId && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return bills;
        }

        public BillDetailViewModel GetByID(int id)
        {
            var bill = Get().Where(b => b.Id == id && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return bill;
        }

        public List<BillDetailViewModel> GetByUsername(string username)
        {
            List<ContractDetailViewModel> contracts = _contractService.GetByUsername(username);
            var contractIds = contracts.Select(c => c.Id);
            var bills = Get().Where(b => contractIds.Contains(b.ContractId) && b.Status == BillConstants.BILL_IS_NOT_DELETED).OrderByDescending(b => b.IssueDate).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return bills;
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
            return GetByID(bill.Id);
        }


        private bool ServiceHasClock(ServiceContractDetailViewModel serviceContract)
        {
            return serviceContract.ClockId != null;
        }
    }
}
