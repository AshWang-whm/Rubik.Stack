﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.UserIdentity
{
    public static class HttpContextAccessorExtension
    {
        public static string? CurrentUserName(this IHttpContextAccessor httpContextAccessor)
                => httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(a => a.Type == JwtIdentityClaimConstants.Name)?.Value;
        public static string? CurrentUserCode(this IHttpContextAccessor httpContextAccessor)
        {
            var oidc_id = httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier);
            if (oidc_id != null)
                return oidc_id.Value;
            return httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(a => a.Type == JwtIdentityClaimConstants.Sub)?.Value;
        }

        public static string? CurrentUserRole(this IHttpContextAccessor httpContextAccessor)
            => httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(a => a.Type == JwtIdentityClaimConstants.Role)?.Value;

        public static string? CurrentUserDept(this IHttpContextAccessor httpContextAccessor)
            => httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(a => a.Type == JwtIdentityClaimConstants.Dept)?.Value;

        public static bool CheckRole(this IHttpContextAccessor httpContextAccessor, string role, string separator = ",") => httpContextAccessor.CheckRoles(role.Split(separator));

        public static bool CheckRoles(this IHttpContextAccessor httpContextAccessor, params string[] roles) => httpContextAccessor.CheckClaims(JwtIdentityClaimConstants.Role, roles);

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="claim"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CheckClaims(this IHttpContextAccessor httpContextAccessor, string claim, string[] source)
        {
            if (source.Length == 0)
                return false;
            var value = httpContextAccessor.Claim(claim);
            if (value == null)
                return false;
            foreach (var item in source)
            {
                if (value.Contains(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 完全匹配
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="claim"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool MapClaims(this IHttpContextAccessor httpContextAccessor, string claim, string[] source)
        {
            if (source.Length == 0)
                return false;
            var value = httpContextAccessor.Claim(claim);
            if (value == null)
                return false;
            foreach (var item in source)
            {
                if (value.Equals(item))
                    return true;
            }
            return false;
        }

        public static string? Claim(this IHttpContextAccessor httpContextAccessor, string claim)
            => httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(a => a.Type == claim)?.Value;
    }
}
