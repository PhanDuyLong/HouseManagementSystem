using AutoMapper;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Payment;
using HMS.FirebaseNotification;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IPaymentService : IBaseService<Payment>
    {
        Task<ResultResponse> Payment(PayBillViewModel model);
        Task<ResultResponse> ConfirmPayment(ConfirmPaymentViewModel model);
    }
    public partial class PaymentService : BaseService<Payment>, IPaymentService
    {
        private readonly IMapper _mapper;
        private readonly IBillService _billService;
        private readonly IFirebaseNotificationService _firebaseNotificationService;
        private readonly IContractService _contractService;
        private readonly IAccountService _accountService;
        public PaymentService(DbContext dbContext, IPaymentRepository repository, IMapper mapper
            , IFirebaseNotificationService firebaseNotificationService, IBillService billService, IContractService contractService, IAccountService accountService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _firebaseNotificationService = firebaseNotificationService;
            _billService = billService;
            _contractService = contractService;
            _accountService = accountService;
        }

        public async Task<ResultResponse> ConfirmPayment(ConfirmPaymentViewModel model)
        {
            var check = _billService.CheckBill(model.BillId);
            if (!check.IsSuccess)
            {
                return check;
            }

            var billModel = _billService.GetById(model.BillId);
            if (billModel.Status == BillConstants.BILL_IS_PAID)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR08", new string[] { "Bill", "paid" }).Value,
                    IsSuccess = false
                };
            }

            var payment = _mapper.Map<Payment>(model);
            payment.IsConfirmed = false;
            payment.Amount = billModel.TotalPrice;
            await CreateAsyn(payment);

            var bill = await _billService.GetAsyn(model.BillId);
            bill.IsWaiting = BillConstants.BILL_IS_NOT_WAITING;

            if(payment.Status == PaymentConstants.PAYMENT_IS_ACTIVE)
            {
                var result = await _billService.SetBillStatusAsync(model.BillId, BillConstants.BILL_IS_PAID);
                if (!result.IsSuccess)
                {
                    return result;
                }
            }
            var resultBill = await _billService.SetBillIsWaitingAsync(model.BillId, BillConstants.BILL_IS_NOT_WAITING);
            if (!resultBill.IsSuccess)
            {
                return resultBill;
            }


            /*var contract = _contractService.GetById(billModel.ContractId.Value);
            var tenant = _accountService.GetByUserId(contract.TenantUserId);
            var tenantId = "all";
            string roomDetail = "phòng " + contract.RoomName + "thuộc nhà " + contract.HouseName;
            string tenantMessage = string.Format(NotificationConstants.APPROVE_PAYMENT, roomDetail);
            if(model.Status == PaymentConstants.PAYMENT_IS_INACTIVE)
            {
                tenantMessage = string.Format(NotificationConstants.DENY_PAYMENT, roomDetail);
            }*/

           /* await SendPaymentNotificationAsync(tenant.Name, tenantId, tenantMessage, billModel.Id);*/

            return new ResultResponse
            {
                Message = new MessageResult("OK04", new string[] { "Pay bill" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> SendPaymentNotificationAsync(string title, string userId, string message, int billId)
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
                Message = new MessageResult("OK04", new string[] { "ContractId" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> Payment(PayBillViewModel model)
        {
            var check = _billService.CheckBill(model.BillId);
            if (!check.IsSuccess)
            {
                return check;
            }

            var billModel = _billService.GetById(model.BillId);
            if (billModel.Status == BillConstants.BILL_IS_PAID)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR08", new string[] { "Bill", "paid!" }).Value,
                    IsSuccess = false
                };
            }



            var payment = _mapper.Map<Payment>(model);
            payment.Note = "Thanh toán";
            payment.Amount = billModel.TotalPrice;
            payment.IsConfirmed = false;
            await CreateAsyn(payment);

            
           /* var result = await _billService.SetBillIsWaitingAsync(model.BillId, BillConstants.BILL_IS_WAITING);
            if (!result.IsSuccess)
            {
                return result;
            }*/


            /*            if (billModel.TotalPrice == payment.Amount)
                        {
                            result = await _billService.SetBillIsPaidFullAsync(model.BillId);
                            if (!result.IsSuccess)
                            {
                                return result;
                            }
                        }*/

            var contract = _contractService.GetById(billModel.ContractId.Value);

            var ownerUserId = "all";
            string roomDetail = "phòng " + contract.RoomName + "thuộc nhà " + contract.HouseName;
            string ownerMessage = string.Format(NotificationConstants.HAVE_PAYMENT, roomDetail);
            await SendPaymentNotificationAsync(contract.OwnerName, ownerUserId, ownerMessage, billModel.Id);

            return new ResultResponse
            {
                Message = new MessageResult("OK04", new string[] { "Pay bill" }).Value,
                IsSuccess = true
            };
        }
    }
}
