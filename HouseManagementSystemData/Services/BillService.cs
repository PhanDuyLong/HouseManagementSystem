using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Parameters;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Bill;
using HMS.Data.ViewModels.BillItem;
using HMS.Data.ViewModels.Clock;
using HMS.Data.ViewModels.Contract.Base;
using HMS.Data.ViewModels.ServiceContract;
using HMS.FirebaseNotification;
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
        Task<BillDetailViewModel> CreateBillAsync(CreateBillViewModel model);
        Task<ResultResponse> UpdateBillAsync(UpdateBillViewModel model);
        Task<ResultResponse> DeleteBillAsync(int billId);
        Task<ResultResponse> SetBillIsPaidFullAsync(int billId);
        Task<ResultResponse> SetBillIsPaidAsync(int billId);
        int CountBills(string userId, BillParameters billParameters);
        Task<ResultResponse> ConfirmBillAsync(int id);
        ResultResponse CheckBill(int id);
    }
    public partial class BillService : BaseService<Bill>, IBillService
    {
        private readonly IMapper _mapper;
        private readonly IContractService _contractService;
        private readonly IClockValueService _clockValueService;
        private readonly IBillItemService _billItemService;
        private readonly IClockService _clockService;
        private readonly IFirebaseNotificationService _firebaseNotificationService;
        public BillService(DbContext dbContext, IBillRepository repository, IMapper mapper,
            IContractService contractService, IClockValueService clockValueService, IBillItemService billItemService, IClockService clockService, IAccountService accountService, IFirebaseNotificationService firebaseNotificationService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contractService = contractService;
            _clockValueService = clockValueService;
            _billItemService = billItemService;
            _clockService = clockService;
            _firebaseNotificationService = firebaseNotificationService;
        }

        public async Task<BillDetailViewModel> CreateBillAsync(CreateBillViewModel createModel)
        {
            var bill = _mapper.Map<Bill>(createModel);
            bill.Status = BillConstants.BILL_IS_NOT_PAID;
            bill.IsDeleted = BillConstants.BILL_IS_NOT_DELETED;
            bill.IsSent = BillConstants.BILL_IS_NOT_SENT;
            bill.IsPaidInFull = BillConstants.BILL_IS_NOT_PAID_IN_FULL;

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
                    if (serviceContract.Service.ServiceType.Equals(ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_DIFFERENT))
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

        public async Task<ResultResponse> DeleteBillAsync(int billId)
        {
            var check = CheckBill(billId);
            if (!check.IsSuccess)
            {
                return check;
            }


            var bill = await GetAsyn(billId);

            if (bill.IsSent == BillConstants.BILL_IS_SENT && bill.Status == BillConstants.BILL_IS_PAID)
            {
                    return new ResultResponse
                    {
                        Message = new MessageResult("BR08", new string[] { "Bill", "paid" }).Value,
                        IsSuccess = false
                    };
            }
                
            bill.IsDeleted = BillConstants.BILL_IS_DELETED;
            Update(bill);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "Bill" }).Value,
                IsSuccess = true
            };
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
            if (bills != null && bills.Count != 0)
            {
                var status = billParameters.Status;
                if (status != null)
                {
                    bills = bills.Where(b => b.Status == status).ToList();
                }
                var isPaidInFull = billParameters.IsPaidInFull;
                if (isPaidInFull != null)
                {
                    bills = bills.Where(b => b.IsPaidInFull == isPaidInFull).ToList();
                }
            }
            return bills;
        }

        public List<ShowBillViewModel> GetByUserId(string userId, BillParameters billParameters)
        {
            List<ContractDetailViewModel> contracts = _contractService.GetByUserId(userId);
            var contractIds = contracts.Select(c => c.Id).ToList();
            if(contractIds != null && contractIds.Count != 0)
            {
                List<ShowBillViewModel> bills = Get().Where(b => contractIds.Contains(b.ContractId.Value) && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).ProjectTo<ShowBillViewModel>(_mapper.ConfigurationProvider).ToList();
                if (billParameters.IsIssueDateAscending)
                {
                    bills = bills.OrderBy(b => b.IssueDate).ToList();
                }
                else
                {
                    bills = bills.OrderByDescending(b => b.IssueDate).ToList();
                }
                return bills;
            }
            return null;
        }

        public List<ShowBillViewModel> GetByContractId(BillParameters billParameters)
        {
            List<ShowBillViewModel> bills = Get().Where(b => b.ContractId == billParameters.ContractId && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).ProjectTo<ShowBillViewModel>(_mapper.ConfigurationProvider).ToList();
            if (billParameters.IsIssueDateAscending)
            {
                bills = bills.OrderBy(b => b.IssueDate).ToList();
            }
            else
            {
                bills = bills.OrderByDescending(b => b.IssueDate).ToList();
            }
            return bills;
        }

        public BillDetailViewModel GetById(int id)
        {
            var bill = Get().Where(b => b.Id == id && b.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return bill;
        }

        public async Task<ResultResponse> UpdateBillAsync(UpdateBillViewModel model)
        {
            var check = CheckBill(model.Id);
            if (!check.IsSuccess)
            {
                return check;
            }

            var bill = await GetAsyn(model.Id);
            if(model.IssueDate != null)
            {
                bill.IssueDate = model.IssueDate;

            }

            if(model.StartDate != null)
            {
                bill.StartDate = model.StartDate;
            }
            if(model.EndDate != null)
            {
                bill.EndDate = model.EndDate;
            }
            if(model.Note != null)
            {
                bill.Note = model.Note;
            }
            Update(bill);

            var updateBillItems = model.UpdateBillItems;
            var newBillItems = new ArrayList();
            if (updateBillItems.Count != 0)
            {
                var oldBillItems = bill.BillItems.ToList();
                var items = new ArrayList();
                foreach (UpdateBillItemViewModel billItem in model.UpdateBillItems)
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

                return new ResultResponse
                {
                    Message = new MessageResult("OK03", new string[] { "Bill" }).Value,
                    IsSuccess = true
                };
        }

        public async Task<ResultResponse> SendBillAsync(int billId)
        {
            MobileNotification firebaseNotification = new MobileNotification
            {
                Title = "Bill",
                Body = "New Bill",
                Data = new Dictionary<string, string>
                {
                    { "billId", billId.ToString()},
                }
            };

            await _firebaseNotificationService.PushNotificationAsync(firebaseNotification);
            return new ResultResponse
            {
                Message = new MessageResult("OK04", new string[] { "Send bill" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> SetBillIsPaidFullAsync(int billId)
        {
            var check = CheckBill(billId);
            if (!check.IsSuccess)
            {
                return check;
            }
            var bill = await GetAsyn(billId);

            if (bill.IsPaidInFull == BillConstants.BILL_IS_PAID_IN_FULL)
            {
                return new ResultResponse
                {
                    IsSuccess = true
                };
            }

            bill.IsPaidInFull = BillConstants.BILL_IS_PAID_IN_FULL;
            Update(bill);
            return new ResultResponse
            {
                Message = new MessageResult("BR04", new string[] { "Bill", "paid in full" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> SetBillIsPaidAsync(int billId)
        {
            var check = CheckBill(billId);
            if (!check.IsSuccess)
            {
                return check;
            }

            var bill = await GetAsyn(billId);

            if (bill.Status == BillConstants.BILL_IS_PAID)
            {
                return new ResultResponse
                {
                    IsSuccess = true
                };
            }

            bill.Status = BillConstants.BILL_IS_PAID;
            Update(bill);
            return new ResultResponse
            {
                Message = new MessageResult("BR04", new string[] { "Bill", "paid" }).Value,
                IsSuccess = true
            };
        }

        public ResultResponse CheckBill(int billId)
        {
            var bill = GetById(billId);
            if (bill == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Bill" }).Value,
                    IsSuccess = false
                };
            }
            return new ResultResponse
            {
                IsSuccess = true
            };
        }

        public int CountBills(string userId, BillParameters billParameters)
        {
            return FilterByParameter(userId, billParameters).Count;
        }

        public async Task<ResultResponse> ConfirmBillAsync(int billId)
        {
            var check = CheckBill(billId);
            if (!check.IsSuccess)
            {
                return check;
            }

            var bill = await GetAsyn(billId);


            if (bill.IsSent == BillConstants.BILL_IS_SENT)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR08", new string[] { "Bill", "confirmed" }).Value,
                    IsSuccess = false
                };
            }

            var serviceContractWithClock = bill.Contract.ServiceContracts.Where(sC => sC.Service.ServiceType.Equals(ServiceTypeConstants.SERVICE_TYPE_IS_DEFAULT_DIFFERENT)).ToList();
            if(serviceContractWithClock != null && serviceContractWithClock.Count != 0)
            {
                var updateServiceContracts = new List<UpdateServiceContractViewModel>();
                foreach (var serviceContract in serviceContractWithClock)
                {
                    foreach(BillItem billItem in bill.BillItems)
                    {
                        if (billItem.ServiceContractId.Equals(serviceContract.Id))
                        {
                            var clockId = _clockService.GetIdByServiceIdAndRoomId(serviceContract.ServiceId.Value, bill.Contract.RoomId.Value);
                            var createClockValueModel = new CreateClockValueViewModel
                            {
                                ClockId = clockId,
                                CreateDate = DateTime.Now,
                                RecordDate = DateTime.Now,
                                IndexValue = (int?)billItem.EndValue.Value
                            };
                            await _clockValueService.CreateClockValueAsync(createClockValueModel);
                        }
                    }
                }
            }

            var result = await SendBillAsync(billId);
            if (!result.IsSuccess)
            {
                return check;
            }
            else
            {
                return new ResultResponse
                {
                    Message = new MessageResult("OK04", new string[] { "Confirm bill" }).Value,
                    IsSuccess = true
                };
            }
        }
    }
}
