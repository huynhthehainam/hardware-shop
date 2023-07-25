

using System.Net;
using System.Security.Claims;
using System.Text.Json;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HardwareShop.WebApi.Middleware
{
    public sealed class FillContextUserMiddleware
    {
        private readonly RequestDelegate next;
        public FillContextUserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context, IJwtService jwtService)
        {
            string? authHeader = context.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader))
            {
                authHeader = authHeader.Replace(JwtBearerDefaults.AuthenticationScheme, "").Trim();
                var user = await jwtService.GetUserFromTokenAsync(authHeader);
                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
                var claims = new Claim[]{
                    new Claim("UserDetail", JsonSerializer.Serialize(user))
                };
                context.User.AddIdentity(new ClaimsIdentity(claims, "basic"));
            }
            await next(context);
        }
    }
}