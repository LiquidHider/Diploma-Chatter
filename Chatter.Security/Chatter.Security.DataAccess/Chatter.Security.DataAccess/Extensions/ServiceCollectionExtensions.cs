using Chatter.Security.DataAccess.DbOptions;
using Chatter.Security.DataAccess.Interfaces;
using Chatter.Security.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Security.DataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(options => {
                var connectionString = configuration?
                .GetSection(DatabaseOptions.ConfigurationSectionName)
                .GetSection(DatabaseOptions.ConnectionStringSectionName).Value;
                options.ChatterDbConnection = connectionString;
            });

            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            return services;
        }
    }
}
