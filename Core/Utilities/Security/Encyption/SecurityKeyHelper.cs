using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Utilities.Security.Encyption
{
    public static class SecurityKeyHelper
    {
        public static SecurityKey CreateSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

        public static string GetRandomStringNumber(int digits)
        {
            var randombyte = new Byte[digits];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(randombyte);
            return Convert.ToBase64String(randombyte);
        }
    }
}