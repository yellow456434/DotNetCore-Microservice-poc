using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace productService.Services
{
    public class CustomTestMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomTestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync($"{nameof(CustomTestMiddleware)} in. \r\n");

            await _next(context);

            await context.Response.WriteAsync($"{nameof(CustomTestMiddleware)} out. \r\n");
        }

    }
}
