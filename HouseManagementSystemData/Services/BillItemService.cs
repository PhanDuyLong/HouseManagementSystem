using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Services
{
    public partial interface IBillItemService : IBaseService<BillItem>
    {
    }
    public partial class BillItemService : BaseService<BillItem>, IBillItemService
    {
        private readonly IMapper _mapper;
        public BillItemService(DbContext dbContext, IBillItemRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
