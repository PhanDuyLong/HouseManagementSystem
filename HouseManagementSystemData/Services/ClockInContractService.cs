using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Services
{
    public partial interface IClockInContractService : IBaseService<ClockInContract>
    {
    }
    public partial class ClockInContractService : BaseService<ClockInContract>, IClockInContractService
    {
        private readonly IMapper _mapper;
        public ClockInContractService(DbContext dbContext, IClockInContractRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
