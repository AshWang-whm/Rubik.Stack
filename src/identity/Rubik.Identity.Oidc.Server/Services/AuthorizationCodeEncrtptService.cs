using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Rubik.Identity.Oidc.Core.Services
{
    public class AuthorizationCodeEncrtptService(IDataProtectionProvider protectionProvider, HttpContextService httpContextService)
    {
        private readonly IDataProtector dataProtector = protectionProvider.CreateProtector("oidc");

        public string GeneratorCode()
        {
            var code_query = httpContextService.ToCodeQueryParameter();
            var code = dataProtector.Protect(JsonSerializer.Serialize(code_query));
            return code;
        }

        public string GenerateCode(AuthorizationEndpointParameter parameter) 
        {
            var code = dataProtector.Protect(JsonSerializer.Serialize(parameter));
            return code;
        }

        public bool VerifyCode(string? code,string? verify,out AuthorizationEndpointParameter? challengeModel)
        {
            challengeModel = null;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(verify))
                return false;

            var plaintext = dataProtector.Unprotect(code!);

            challengeModel = JsonSerializer.Deserialize<AuthorizationEndpointParameter>(plaintext);
            if (challengeModel == null)
                return false;

            var hash = SHA256.HashData(Encoding.ASCII.GetBytes(verify!));
            var verify_txt = Base64UrlTextEncoder.Encode(hash);

            return verify_txt == challengeModel?.CodeChallenge;
        }

        /// <summary>
        /// Creates the hash for the various hash claims (e.g. c_hash, at_hash or s_hash).
        /// </summary>
        /// <param name="value">The value to hash.</param>
        /// <param name="tokenSigningAlgorithm">The token signing algorithm</param>
        /// <returns></returns>
        public static string CreateHashClaimValue(string value, string tokenSigningAlgorithm)
        {
            using var sha = GetHashAlgorithmForSigningAlgorithm(tokenSigningAlgorithm);
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(value));
            var size = (sha.HashSize / 8) / 2;

            var leftPart = new byte[size];
            Array.Copy(hash, leftPart, size);

            return Base64Url.Encode(leftPart);
        }

        /// <summary>
        /// Returns the matching hashing algorithm for a token signing algorithm
        /// </summary>
        /// <param name="signingAlgorithm">The signing algorithm</param>
        /// <returns></returns>
        public static HashAlgorithm GetHashAlgorithmForSigningAlgorithm(string signingAlgorithm)
        {
            var signingAlgorithmBits = int.Parse(signingAlgorithm[^3..]);

            return signingAlgorithmBits switch
            {
                256 => SHA256.Create(),
                384 => SHA384.Create(),
                512 => SHA512.Create(),
                _ => throw new InvalidOperationException($"Invalid signing algorithm: {signingAlgorithm}"),
            };
        }

    }
}
