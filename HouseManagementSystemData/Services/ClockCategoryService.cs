using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IClockCategoryService : IBaseService<ClockCategory>
    {
        Task<List<ClockCategory>> GetClockCategoriesAsync();
    }
    public partial class ClockCategoryService : BaseService<ClockCategory>, IClockCategoryService
    {
        private readonly IMapper _mapper;
        public ClockCategoryService(DbContext dbContext, IClockCategoryRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<ClockCategory>> GetClockCategoriesAsync()
        {
            return await Get().ToListAsync();
        }
    }
}
