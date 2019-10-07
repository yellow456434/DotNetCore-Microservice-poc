using Microsoft.AspNetCore.Http;
using productService.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static string GetEnumDescription<T>(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            EnumDescriptionAttribute[] attributes =
                (EnumDescriptionAttribute[])fi.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else return value.ToString();
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
