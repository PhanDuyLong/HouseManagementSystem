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
        Task<ResultResponse> SetBillIsWaitingAsync(int billId, bool status);
        Task<ResultResponse> SetBillStatusAsync(int billId, bool status);
        int CountBills(string userId, BillParameters billParameters);
        Task<ResultResponse> ConfirmBillAsync(ConfirmBillViewModel model);
        ResultResponse CheckBill(int id);
        //Task ScanBill();

    }
    public partial class BillService : BaseService<Bill>, IBillService
    {
        private readonly IMapper _mapper;
        private readonly IContractService _contractService;
        private readonly IClockValueService _clockValueService;
        private readonly IBillItemService _billItemService;
        private readonly IClockService _clockService;
        private readonly IFirebaseNotificationService _firebaseNotificationService;
        private readonly IServiceContractService _serviceContractService;
        private readonly IServiceService _serviceService;
        private readonly IHouseService _houseService;
        private readonly IAccountService _accountService;
        public BillService(DbContext dbContext, IBillRepository repository, IMapper mapper,
            IContractService contractService, IClockValueService clockValueService, IBillItemService billItemService, IClockService clockService,  IFirebaseNotificationService firebaseNotificationService, IServiceService serviceService, IServiceContractService serviceContractService, IHouseService houseService, IAccountService accountService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contractService = contractService;
            _clockValueService = clockValueService;
            _billItemService = billItemService;
            _clockService = clockService;
            _firebaseNotificationService = firebaseNotificationService;
            _serviceService = serviceService;
            _serviceContractService = serviceContractService;
            _houseService = houseService;
            _accountService = accountService;
        }

        public async Task<BillDetailViewModel> CreateBillAsync(CreateBillViewModel createModel)
        {
            var bill = _mapper.Map<Bill>(createModel);
            bill.Status = BillConstants.BILL_IS_NOT_PAID;
            bill.IsDeleted = BillConstants.BILL_IS_NOT_DELETED;
            bill.IsSent = BillConstants.BILL_IS_NOT_SENT;

            var billItems = new ArrayList();
            var contract = _contractService.GetById(createModel.ContractId);
            if (contract == null)
            {
                return null;
            }
            var serviceContracts = contract.ServiceContracts.ToList();
            double total = 0;
            var roomPriceBillItem = await CountRoomPriceAsync(contract);
            billItems.Add(roomPriceBillItem);
            total += roomPriceBillItem.TotalPrice.Value;
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

        private async Task<BillItem> CountRoomPriceAsync(ContractDetailViewModel contract)
        {
            var roomPriceService = new Service()
            {
                Name = "Phòng",
                CalculationUnit = "tháng",
                Price = contract.RoomPrice,
                ServiceTypeId = 2,
                Status = ServiceConstants.SERVICE_IS_ACTIVE
            };
            await _serviceService.CreateAsyn(roomPriceService);
            var roomPriceServiceContract = new ServiceContract()
            {
                ServiceId = roomPriceService.Id,
                UnitPrice = contract.RoomPrice,
                Status = ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE,
            };
            await _serviceContractService.CreateAsyn(roomPriceServiceContract);
            var roomPriceBillItem = new BillItem
            {
                TotalPrice = contract.RoomPrice,
                Status = BillItemConstants.BILL_ITEM_IS_NOT_DELETED,
                ServiceContractId = roomPriceServiceContract.Id
            };

            return roomPriceBillItem;
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
            var roomId = billParameters.RoomId;
            if(roomId != null)
            {
                bills = GetByRoomId(billParameters);
            }
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
                var isWaiting = billParameters.IsWaiting;
                if (isWaiting != null)
                {
                    bills = bills.Where(b => b.IsWaiting == isWaiting).ToList();
                }
            }
            return bills;
        }

        private List<ShowBillViewModel> GetByRoomId(BillParameters billParameters)
        {
            List<ContractDetailViewModel> contracts = _contractService.GetByRoomId(billParameters.RoomId.Value);
            var contractIds = contracts.Select(c => c.Id).ToList();
            if (contractIds != null && contractIds.Count != 0)
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
            if(bill != null)
            {
                foreach(var billItem in bill.BillItems)
                {
                    if(billItem.ServiceContract.ClockId != 0 && billItem.ServiceContract.Clock.ClockValues != null)
                    {
                        billItem.ServiceContract.Clock.ClockValues = billItem.ServiceContract.Clock.ClockValues.Where(value => value.Status == ClockValueConstants.CLOCK_VALUE_IS_MILESTONE).ToList();
                    }
                }
            }
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
            if (updateBillItems.Count != 0)
            {
                await _billItemService.UpdateBillItemsAsync(bill.Id, updateBillItems.ToList());
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
                UserId = "all",
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

        public async Task<ResultResponse> SetBillIsWaitingAsync(int billId, bool status)
        {
            var check = CheckBill(billId);
            if (!check.IsSuccess)
            {
                return check;
            }
            var bill = await GetAsyn(billId);

            if (bill.IsWaiting == status)
            {
                return new ResultResponse
                {
                    IsSuccess = true
                };
            }

            bill.IsWaiting = status;
            Update(bill);
            return new ResultResponse
            {
                Message = new MessageResult("BR04", new string[] { "Bill", "paid in full" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> SetBillStatusAsync(int billId, bool status)
        {
            var check = CheckBill(billId);
            if (!check.IsSuccess)
            {
                return check;
            }

            var bill = await GetAsyn(billId);

            bill.Status = status;
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

        public async Task<ResultResponse> ConfirmBillAsync(ConfirmBillViewModel model)
        {
            var check = CheckBill(model.Id);
            if (!check.IsSuccess)
            {
                return check;
            }

            var billModel = GetById(model.Id);


            /*if (billModel.IsSent == BillConstants.BILL_IS_SENT)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR08", new string[] { "Bill", "confirmed" }).Value,
                    IsSuccess = false
                };
            }*/




            var serviceContractWithClock = billModel.BillItems.Where(item => item.ServiceContract.ClockId != 0).ToList();
            if(serviceContractWithClock != null && serviceContractWithClock.Count != 0)
            {
                foreach (var serviceContract in serviceContractWithClock)
                {
                    foreach(var billItem in billModel.BillItems)
                    {
                        if (billItem.ServiceContractId.Equals(serviceContract.ServiceContractId))
                        {
                            var clockId = _clockService.GetIdByServiceIdAndRoomId(billItem.ServiceContract.ServiceId.Value, billModel.Contract.RoomId);
                            var createClockValueModel = new CreateClockValueViewModel
                            {
                                ClockId = clockId,
                                CreateDate = DateTime.Now,
                                RecordDate = DateTime.Now,
                                IndexValue = (int)billItem.EndValue.Value
                            };
                            await _clockValueService.CreateClockValueAsync(createClockValueModel);
                        }
                    }
                }
            }
            var contract = _contractService.GetById(billModel.ContractId.Value);
            var tenant = _accountService.GetByUserId(contract.TenantUserId);
            string userId = "'all'";
            string title = tenant.Name;
            string message = NotificationConstants.HAVE_NEW_BILL;

            var result = await SendBillNotificationAsync(userId, title, message, model.Id);
            if (!result.IsSuccess)
            {
                return check;
            }
            else
            {
                var bill = await GetAsyn(model.Id);
                bill.Note = model.Note;
                bill.Status = BillConstants.BILL_IS_NOT_PAID;
                bill.IsWaiting = BillConstants.BILL_IS_NOT_WAITING;
                Update(bill);
                return new ResultResponse
                {
                    Message = new MessageResult("OK04", new string[] { "Confirm bill" }).Value,
                    IsSuccess = true
                };
            }
        }

        public List<BillDetailViewModel> GetBillInactive()
        {
            var bills = Get().Where(bill => bill.Status == BillConstants.BILL_IS_NOT_PAID &&  bill.IsDeleted == BillConstants.BILL_IS_NOT_DELETED).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return bills;
        }

        public async Task<ResultResponse> SetBillIsSent(int billId)
        {
            var bill = await GetAsyn(billId);
            bill.IsSent = BillConstants.BILL_IS_SENT;
            Update(bill);
            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Bill" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> SendBillNotificationAsync(string title, string userId, string message, int billId)
        {
            MobileNotification firebaseNotification = new MobileNotification
            {
                UserId = userId,
                Title = title,
                Body = message,
                Data = new Dictionary<string, string>
                {
                    { "billId", billId.ToString()},
                }
            };

            await _firebaseNotificationService.PushNotificationAsync(firebaseNotification);
            return new ResultResponse
            {
                Message = new MessageResult("OK04", new string[] { "Bill" }).Value,
                IsSuccess = true
            };
        }

        public async Task ScanBill()
        {
            var bills = GetBillInactive();
            foreach(var bill in bills)
            {
                var house = _contractService.GetHouseByContractId(bill.ContractId.Value);
                var billEndDate = bill.EndDate;
                var today = DateTime.Now;
                if((billEndDate.Value.Month == today.Month || today.Month - billEndDate.Value.Month == 1) && billEndDate.Value.Year == today.Year && today >= bill.EndDate)
                {
                    if(today.Day == house.HouseInfo.PaidDeadline)
                    {
                        var contract = _contractService.GetById(bill.ContractId.Value);
                        var tenant = _accountService.GetByUserId(contract.TenantUserId);

                        await SendBillNotificationAsync(tenant.Name, "all", NotificationConstants.TODAY_IS_ROOM_BILL_PAID_DEADLINE, bill.Id);
                        await SetBillIsSent(bill.Id);
                    }

                    if(today.Day > house.HouseInfo.PaidDeadline)
                    {
                        var contract = _contractService.GetById(bill.ContractId.Value);
                        var owner = _accountService.GetByUserId(contract.OwnerUserId);

                        await SendBillNotificationAsync(owner.Name, "all", NotificationConstants.ROOM_BILL_PAID_DEADLINE_IS_PASSED, bill.Id);
                        await SetBillIsSent(bill.Id);
                    }
                }
            }
        }
    }
}
