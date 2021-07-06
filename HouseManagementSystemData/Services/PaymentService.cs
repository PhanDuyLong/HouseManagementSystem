using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IPaymentService : IBaseService<Payment>
    {
        Task<string> Payment(BillPaymentViewModel model);
    }
    public partial class PaymentService : BaseService<Payment>, IPaymentService
    {
        private readonly IMapper _mapper;
        public PaymentService(DbContext dbContext, IPaymentRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public async Task<string> Payment(BillPaymentViewModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
