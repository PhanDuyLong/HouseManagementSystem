using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories;
using HouseManagementSystem.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HouseManagementSystem.Data.DependencyInjection
{
    public static class DependencyInjectionResolverGen
    {
        public static void IntializerDI(this IServiceCollection services)
        {
            services.AddScoped<DbContext, House_ManagementContext>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IHouseInfoService, HouseInfoService>();
            services.AddScoped<IHouseInfoRepository, HouseInfoRepository>();

            services.AddScoped<IHouseRepository, HouseRepository>();
            services.AddScoped<IHouseService, HouseService>();

            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
        }
    }
}
