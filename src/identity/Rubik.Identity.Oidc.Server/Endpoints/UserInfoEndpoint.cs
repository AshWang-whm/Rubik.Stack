using Microsoft.AspNetCore.Http;

namespace Rubik.Identity.Oidc.Core.Endpoints
{
    internal class UserInfoEndpoint
    {
        public static async Task<IResult> GetUserInfo()
        {
            await Task.Delay(100);

            return Results.Json(new { });
        }
    }
}
