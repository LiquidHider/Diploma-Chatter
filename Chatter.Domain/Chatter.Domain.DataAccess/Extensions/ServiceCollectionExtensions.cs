using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Domain.DataAccess.Extensions
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
            
            services.AddTransient<IChatMessageRepository, ChatMessageRepository>();
            services.AddTransient<IChatUserRepository, ChatUserRepository>();
            services.AddTransient<IPhotoRepository, PhotoRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();
            services.AddTransient<IGroupChatRepository, GroupChatRepository>();

            return services;
        }
    }
}
