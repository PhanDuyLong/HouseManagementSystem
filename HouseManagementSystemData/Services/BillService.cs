using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels.Bill;
using HMS.Data.ViewModels.HouseViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HMS.Data.Services
{
    public partial interface IBillService : IBaseService<Bill>
    {
        List<BillDetailViewModel> GetByContractID(int contractId);
        BillDetailViewModel GetByID(string id);
    }
    public partial class BillService : BaseService<Bill>, IBillService
    {
        private readonly IMapper _mapper;
        public BillService(DbContext dbContext, IBillRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public List<BillDetailViewModel> GetByContractID(int contractId)
        {
            var bills = Get().Where(b => b.ContractId == contractId).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return bills;
        }

        public BillDetailViewModel GetByID(string id)
        {
            var bill = Get().Where(b => b.Id == id).ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return bill;
        }
    }
}
