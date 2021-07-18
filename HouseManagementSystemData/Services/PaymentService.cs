using AutoMapper;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.FirebaseNotification;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IPaymentService : IBaseService<Payment>
    {
        Task<ResultResponse> Payment(PayBillViewModel model);
    }
    public partial class PaymentService : BaseService<Payment>, IPaymentService
    {
        private readonly IMapper _mapper;
        private readonly IBillService _billService;
        private readonly IFirebaseNotificationService _firebaseNotificationService;
        public PaymentService(DbContext dbContext, IPaymentRepository repository, IMapper mapper
            , IFirebaseNotificationService firebaseNotificationService, IBillService billService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _firebaseNotificationService = firebaseNotificationService;
            _billService = billService;
        }

        public async Task<ResultResponse> Payment(PayBillViewModel model)
        {
            var check = _billService.CheckBill(model.BillId);
            if (!check.IsSuccess)
            {
                return check;
            }

            var billModel = _billService.GetById(model.BillId);
            if (billModel.Status == BillConstants.BILL_IS_PAID_IN_FULL)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR08", new string[] { "Bill", "paid in full!" }).Value,
                    IsSuccess = false
                };
            }

            var payment = _mapper.Map<Payment>(model);
            payment.IsSend = PaymentConstants.PAYMENT_IS_SENT;
            payment.Status = PaymentConstants.PAYMENT_IS_ACTIVE;
            await CreateAsyn(payment);

            var result = await _billService.SetBillIsPaidAsync(model.BillId);
            if (!result.IsSuccess)
            {
                return result;
            }
            

            if (billModel.TotalPrice == payment.Amount)
            {
                result = await _billService.SetBillIsPaidFullAsync(model.BillId);
                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            MobileNotification firebaseNotification = new MobileNotification
            {
                Title = "Payment",
                Body = "New Payment",
                Data = new Dictionary<string, string>
                {
                    { "billId", model.BillId.ToString()},
                }
            };

            await _firebaseNotificationService.PushNotificationAsync(firebaseNotification);
            return new ResultResponse
            {
                Message = new MessageResult("OK04", new string[] { "Pay bill" }).Value,
                IsSuccess = true
            };
        }
    }
}
