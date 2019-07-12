using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace productService.Services
{
    public class CustomTestAuthorizationFilter : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.Headers.Add("S","dfv");
        }
    }
}
