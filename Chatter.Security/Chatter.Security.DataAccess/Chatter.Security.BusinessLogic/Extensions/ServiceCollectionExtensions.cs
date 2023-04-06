using Chatter.Security.Core.Encryptors;
using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Services;
using Chatter.Security.DataAccess.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Security.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityCore(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDataAccess(configuration);

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ISignInService, SignInService>();
            services.AddTransient<IHMACEncryptor, HMACSHA256Encryptor>();

            return services;
        }
    }
}
