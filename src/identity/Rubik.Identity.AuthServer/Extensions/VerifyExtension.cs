using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;

namespace Rubik.Identity.AuthServer.Extensions
{
    public static class VerifyExtension
    {
        /// <summary>
        /// 验证code , exprie 时间, 是否多次使用 , etc...
        /// </summary>
        /// <param name="code"></param>
        /// <param name="verifyer"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static bool VerifyCode(this IDataProtectionProvider dataProtectionProvider,string code, string verifyer, out AuthCodeModel authcode)
        {
            var protector = dataProtectionProvider.CreateProtector("oauth");
            var auth = System.Text.Json.JsonSerializer.Deserialize<AuthCodeModel>(protector.Unprotect(code));

            var sha256 = SHA256.HashData(Encoding.ASCII.GetBytes(verifyer));

            var verfiy = Base64UrlTextEncoder.Encode(sha256);
            authcode = auth;
            return verfiy == auth?.CodeChallenge;
        }
    }

    public class AuthCodeModel
    {
        public required string? ClientID { get; set; }

        public required string Nonce { get; set; }
        public required string? CodeChallenge { get; set; }

        //public required string? CodeChallengeMethod { get; set; }

        //public required string? RedirectUri { get; set; }

        public required DateTime Expriy { get; set; }

        public required string Scope { get; set; }
    }
}
