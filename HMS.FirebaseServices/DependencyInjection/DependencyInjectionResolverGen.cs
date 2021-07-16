using HMS.FirebaseNotification;
using HMS.FirebaseServices.Authen.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.FirebaseServices.DependencyInjection
{
    public static class DependencyInjectionResolverGen
    {
        public static void IntializerFirebaseDI(this IServiceCollection services)
        {
            services.AddScoped<IFirebaseAuthenService, FirebaseAuthenService>();
            services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();
        }
    }
}
