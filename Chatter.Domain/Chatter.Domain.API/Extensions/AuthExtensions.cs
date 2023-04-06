using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Chatter.Domain.API.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddChatterAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var authOptions = configuration?.GetSection(AuthOptions.ConfigurationSectionName).Get<AuthOptions>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthOptions.DefaultAuthPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

            services
           .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Auth:JwtTokenKey").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }
    }
}
