using HMS.Authen.Services;
using HMS.Authen.Utilities;
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

            

            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IServiceRepository, ServiceRepository>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            
         
            

            

            services.AddScoped<IClockCategoryRepository, ClockCategoryRepository>();
            services.AddScoped<IClockCategoryService, ClockCategoryService>();

            services.AddScoped<IClockRepository, ClockRepository>();
            services.AddScoped<IClockService, ClockService>();

            services.AddScoped<IClockValueRepository, ClockValueRepository>();
            services.AddScoped<IClockValueService, ClockValueService>();

            

            services.AddScoped<IServiceContractRepository, ServiceContractRepository>();
            services.AddScoped<IServiceContractService, ServiceContractService>();

            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IContractService, ContractService>();

            services.AddScoped<IBillItemRepository, BillItemRepository>();
            services.AddScoped<IBillItemService, BillItemService>();

            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IBillService, BillService>();

            services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();

            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IRoomService, RoomService>();

            services.AddScoped<IHouseRepository, HouseRepository>();
            services.AddScoped<IHouseService, HouseService>();
        }
    }
}
