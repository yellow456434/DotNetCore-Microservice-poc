using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace productService.Services
{
    public class JwtAuth
    {
        private readonly IConfiguration config;

        public JwtAuth(IConfiguration config)
        {
            this.config = config;
        }

        //public string GetJwtSecurityToken(D_User user)
        //{
        //    var token = new JwtSecurityToken(
        //      claims: new List<Claim> {
        //          new Claim("uid", user.UID.ToString()),
        //          new Claim("groupId", user.GroupID),
        //      },
        //      expires: DateTime.Now.AddHours(1),
        //      signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtKey"])), SecurityAlgorithms.HmacSha256)
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //public static GetClaimModel GetClaim(ClaimsPrincipal user)
        //{
        //    var model = new GetClaimModel();

        //    var claims = user.Claims;

        //    //取得使用者權限(目前是不傳也會過)
        //    var uid_KeyValue = claims.Where(x => x.Type == "uid").FirstOrDefault();
        //    var groupId_KeyValue = claims.Where(x => x.Type == "groupId").FirstOrDefault();
        //    //if (uid_KeyValue == null || groupId_KeyValue == null)
        //    //{
        //    //    throw new Exception("登入失敗");
        //    //}
        //    //var uid = uid_KeyValue.Value;
        //    //var groupId = groupId_KeyValue.Value;

        //    //tscancel
        //    model.uid = (uid_KeyValue == null) ? "1" : uid_KeyValue.Value;
        //    model.groupId = (groupId_KeyValue == null) ? "A" : groupId_KeyValue.Value;

        //    return model;
        //}
    }
}
