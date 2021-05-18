using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
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

        public static string DecryptString(string key, string encryptValue)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(encryptValue);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }

                    }
                }


            }

        }
    }
}