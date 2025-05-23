﻿using Rubik.Identity.AuthServer.Models;
using Rubik.Identity.Oidc.Core.Services;
using Rubik.Identity.Oidc.Core.Stores;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;
using Rubik.Identity.UserIdentity;
using Rubik.Infrastructure.Utils.Common.String;
using System.Security.Claims;

namespace Rubik.Identity.AuthServer.Stores
{
    public class UserStore (IFreeSql freeSql) : IUserStore
    {
        public async Task<bool> ValidateUser(string usercode, string password)
        {
            var user = await freeSql.Select<TbUser>()
                .Where(a => a.Code == usercode)
                .FirstAsync();

            if (user == null)
            {
                return false;
            }

            var pwd = PasswordEncryptExtension.GeneratePasswordHash(usercode, password);

            return user.Password== pwd;
        }

        public async Task<List<Claim>> GetUserClaims(string usercode, string clientid, IEnumerable<string?> claimtypes)
        {
            // 抓取内置的scope & 用户数据
            var claims = new List<Claim>();
            // 默认带上code&name
            var userinfo = await freeSql.Select<TbUser>()
                .Where(a => a.Code == usercode && a.IsDelete == false)
                .FirstAsync() ?? throw new Exception($"[{usercode}]不存在!");

            claims.Add(new Claim(JwtIdentityClaimConstants.Name, userinfo.Name!));
            claims.Add(new Claim(JwtIdentityClaimConstants.Sub, userinfo.Code!));

            if (claimtypes.Contains(JwtIdentityClaimConstants.Role))
            {
                var roles = await freeSql.Select<TbUser, TbRelationRoleUser, TbRole,TbRelationRolePermission,TbApplicationPermission,TbApplication>()
                    .LeftJoin((a, b, c,d,e,f) => a.ID == b.UserID)
                    .LeftJoin((a, b, c, d, e, f) => b.RoleID==c.ID)
                    .LeftJoin((a, b, c, d, e, f) => c.ID==d.RoleID)
                    .LeftJoin((a, b, c, d, e, f) => d.PermissionID==e.ID)
                    .LeftJoin((a, b, c, d, e, f) => e.ApplicationID==f.ID)
                    .Where((a, b, c, d, e, f) => f.Code==clientid&&a.Code==usercode)
                    .ToListAsync((a,b,c, d, e, f) => c.Code);
                claims.Add(new Claim(JwtIdentityClaimConstants.Role, roles?.StringJoin()??""));
            }
            if(claimtypes.Contains(JwtIdentityClaimConstants.Job))
            {
                var jobs = await freeSql.Select<TbUser, TbRelationJobUser, TbOrganizationJob>()
                    .LeftJoin((a, b, c) => a.ID == b.UserID)
                    .LeftJoin((a, b, c) => b.JobID==c.ID)
                    .Where((a,b,c)=>a.Code==usercode)
                    .ToListAsync((a,b,c)=>c.Code);
                claims.Add(new Claim(JwtIdentityClaimConstants.Job, jobs?.StringJoin() ?? ""));
            }
            if (claimtypes.Contains(JwtIdentityClaimConstants.Position))
            {
                var pos = await freeSql.Select<TbUser,TbRelationPositionUser,TbPosition>()
                    .LeftJoin((a, b, c) => a.ID == b.UserID)
                    .LeftJoin((a, b, c) => b.PositionID == c.ID)
                    .Where((a,b,c)=>a.Code==usercode)
                    .ToListAsync((a, b, c) => c.Code);
                claims.Add(new Claim(JwtIdentityClaimConstants.Position, pos?.StringJoin() ?? ""));
            }
            if (claimtypes.Contains(JwtIdentityClaimConstants.Dept))
            {
                var depts = await freeSql.Select<TbUser,TbRelationOrganizeUser,TbOrganization>()
                    .LeftJoin((a, b, c) => a.ID == b.UserID)
                    .LeftJoin((a, b, c) => b.OrganizationID == c.ID)
                    .Where((a, b, c) => a.Code == usercode)
                    .ToListAsync((a, b, c) => c.Code);
                claims.Add(new Claim(JwtIdentityClaimConstants.Dept, depts?.StringJoin() ?? ""));
            }

            return claims;
        }

        public Task<IEnumerable<string>> UserProfilesClaims()
        {
            return Task.FromResult<IEnumerable<string>>([JwtIdentityClaimConstants.Role, JwtIdentityClaimConstants.Job, JwtIdentityClaimConstants.Position, JwtIdentityClaimConstants.Dept]);
        }

        //public async Task<UserInfoVO> GetUserInfo()
        //{
        //    var userid = httpContextService.
        //    var user = await freeSql.Select<TbUser>()
        //        .Where(a=>)
        //}
    }
}
