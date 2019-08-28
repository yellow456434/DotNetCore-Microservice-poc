using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace productService.Services
{
    public class Utils
    {
        public static string preLog(HttpContext context, bool isResponse = false)
        {
            //var claim = JwtAuth.GetClaim(context.User);
            //, UserId: {claim.uid}, GroupID: {claim.groupId}"

            return ((!isResponse) ? "req" : "res") + $": {context.TraceIdentifier}" + Environment.NewLine;
        }
    }
}
