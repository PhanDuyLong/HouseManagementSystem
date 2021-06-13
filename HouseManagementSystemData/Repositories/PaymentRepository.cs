using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Repositories
{
    public partial interface IPaymentRepository : IBaseRepository<Payment>
    {
    }
    public partial class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
