using Chatter.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Chatter.Web.Middlewares
{
    public class HttpMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration config)
        {
            var request = context.Request;
            if (request.Cookies["User"] != null)
            {
                var token = JsonConvert.DeserializeObject<SecurityUserResponse>(request.Cookies["User"])?.Token;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(config.GetSection("JwtTokenKey").Value);
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                }
                catch (SecurityTokenExpiredException e) 
                {
                    context.Response.Cookies.Delete("User");
                    context.Response.Redirect("/");
                    return;
                }

                request.Headers.Add("Authorization", $"Bearer {token}");

                if (context.Request.Path == "/" || context.Request.Path == "/signUp")
                {
                    context.Response.Redirect("/Chat");
                }


            }
            await _next(context);
        }
    }
}
