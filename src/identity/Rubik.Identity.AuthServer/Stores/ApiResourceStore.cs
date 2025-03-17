using Rubik.Identity.Oidc.Core.Contants;
using Rubik.Identity.Oidc.Core.OidcEntities;
using Rubik.Identity.Oidc.Core.Stores;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.AuthServer.Stores
{
    public class ApiResourceStore(IFreeSql freeSql) : IApiResourceStore
    {
        public async Task<List<ApiResourceEntity>> GetApiResources(string scope)
        {
            var scopes = scope.Split(' ',StringSplitOptions.RemoveEmptyEntries);
            var apis = await freeSql.Select<TbApiResource, TbApiScope>()
                .LeftJoin((a, b) => a.ID == b.ApiID)
                .Where((a, b) => scopes.Contains(a.Code))
                .ToListAsync((a,b)=>new
                {
                    ApiResourceName = a.Name,
                    ApiResourceCode= a.Code,
                    Scope =b.Code,
                    Claims=b.Claims
                });

            return apis.GroupBy(a => new { a.ApiResourceName, a.ApiResourceCode })
                .Select(group => new ApiResourceEntity
                {
                    Name = group.First().ApiResourceName,
                    Code = group.First().ApiResourceCode,
                    Scopes = [.. group.Select(a => new ApiScopeEntity { Name = a.Scope, Claims = a.Claims })]
                }).ToList();
        }
    }
}
