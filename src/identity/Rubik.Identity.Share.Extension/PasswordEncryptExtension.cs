using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Extension
{
    /// <summary>
    /// 简单md5加密
    /// </summary>
    public class PasswordEncryptExtension
    {
        /// <summary>
        /// 固定盐
        /// </summary>
        const string Salt = "Rubik.Identity";
        public static string GeneratePasswordHash(string code, string password)
        {
            byte[] passwordAndSaltBytes = Encoding.UTF8.GetBytes(code + password + Salt);
            byte[] hashBytes = MD5.HashData(passwordAndSaltBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
