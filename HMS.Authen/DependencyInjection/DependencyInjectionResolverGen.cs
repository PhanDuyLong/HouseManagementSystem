using HMS.Authen.Services;
using HMS.Authen.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Authen.DependencyInjection
{
    public static class DependencyInjectionResolverGen
    {
        public static void IntializerIndentityDI(this IServiceCollection services)
        {
            services.AddScoped<JwtHandler>();
            services.AddScoped<IAccountAuthenService, AccountAuthenService>();
        }
    }
}
