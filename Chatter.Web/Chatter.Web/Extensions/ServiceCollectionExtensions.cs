using Chatter.Web.Interfaces;
using Chatter.Web.Services;

namespace Chatter.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChatterServices(this IServiceCollection services) 
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IChatUserService, ChatUserService>();

            return services;
        }
    }
}
