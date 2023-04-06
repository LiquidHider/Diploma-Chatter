using Chatter.Security.API.Options;

namespace Chatter.Security.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddChatterCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOptions = configuration?.GetSection(CorsOptions.ConfigurationSectionName).Get<CorsOptions>();

            services.AddCors(options =>
            {
                options.AddPolicy(CorsOptions.PolicyName,
                    corsBuilder =>
                    {
                        if (corsOptions != null)
                        {
                            if (corsOptions.AllowedOrigins?.Length > 0)
                            {
                                ConfigureOrigins(corsBuilder, corsOptions.AllowedOrigins);
                            }

                            if (corsOptions.AllowedHeaders?.Length > 0)
                            {
                                ConfigureHeaders(corsBuilder, corsOptions.AllowedHeaders);
                            }

                            if (corsOptions.AllowedMethods?.Length > 0)
                            {
                                ConfigureMethods(corsBuilder, corsOptions.AllowedMethods);
                            }
                        }
                    });
            });

            return services;
        }

        private static void ConfigureOrigins(Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder corsBuilder, string[] allowedOrigins)
        {
            var corsCompliant = true;

            if (allowedOrigins.Any(x => x == CorsOptions.AllowAllOption))
            {
                corsCompliant = false;
                corsBuilder.AllowAnyOrigin();
            }
            else
            {
                corsBuilder.WithOrigins(allowedOrigins);
            }

            if (corsCompliant)
            {
                corsBuilder.AllowCredentials();
            }
        }

        private static void ConfigureHeaders(Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder corsBuilder, string[] allowedHeaders)
        {
            if (allowedHeaders.Any(x => x == CorsOptions.AllowAllOption))
            {
                corsBuilder.AllowAnyHeader();
            }
            else
            {
                corsBuilder.WithHeaders(allowedHeaders);
            }
        }

        private static void ConfigureMethods(Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder corsBuilder, string[] allowedMethods)
        {
            if (allowedMethods.Any(x => x == CorsOptions.AllowAllOption))
            {
                corsBuilder.AllowAnyMethod();
            }
            else
            {
                corsBuilder.WithMethods(allowedMethods);
            }
        }
    }
}
