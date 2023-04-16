using Chatter.Web.Models;
using Newtonsoft.Json;

namespace Chatter.Web.Middlewares
{
    public class HttpMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            if (request.Cookies["User"] != null)
            {
                var token = JsonConvert.DeserializeObject<SecurityUserResponse>(request.Cookies["User"])?.Token;

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
