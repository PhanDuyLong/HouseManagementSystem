using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Data.DependencyInjection
{
    public static class DependencyInjectionResolverGen
    {
        public static void IntializerDI(this IServiceCollection services)
        {
            services.AddScoped<DbContext, HMSDBContext>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IHouseInfoService, HouseInfoService>();
            services.AddScoped<IHouseInfoRepository, HouseInfoRepository>();

            services.AddScoped<IHouseRepository, HouseRepository>();
            services.AddScoped<IHouseService, HouseService>();

            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IServiceRepository, ServiceRepository>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
        }
    }
}
