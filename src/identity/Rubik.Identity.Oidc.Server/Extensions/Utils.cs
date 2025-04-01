using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Extensions
{
    internal static class Utils
    {
        public static string GetMd5Hash(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes);

            // Convert byte array to a string
            StringBuilder sb = new();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2")); // Convert to hexadecimal
            }
            return sb.ToString();
        }

        public static bool ArrayExcept(IEnumerable<string?>? left ,IEnumerable<string?>? right)
        {
            if (left == null || right == null)
            {
                return true;
            }

            if (left.Count() != right.Count())
            {
                return true;
            }
            return (left.Except(right).Any()) || (right.Except(left).Any());
        }
    }


}
