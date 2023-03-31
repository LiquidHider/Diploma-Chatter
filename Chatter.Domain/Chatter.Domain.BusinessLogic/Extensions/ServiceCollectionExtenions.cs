using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Services;
using Chatter.Domain.DataAccess.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Domain.BusinessLogic.Extensions
{
    public static class ServiceCollectionExtenions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccess(configuration);

            services.AddScoped<IPrivateChatService, PrivateChatService>();

            return services;
        }

        
    }
}
