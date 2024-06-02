using Microsoft.AspNetCore.DataProtection;

namespace Server.Endpoints
{
    public class UserInfoEnpoint
    {
        public static async Task<IResult> GetUserInfo(HttpContext httpContext)
        {
            await Task.Delay(100);

            return Results.Json(new { });
        }
    }
}
