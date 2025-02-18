using Microsoft.Extensions.DependencyInjection;

namespace Rubik.Identity.UserIdentity
{
    public static class Extensions
    {
        public static void AddUserIdentity(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<UserIdentityAccessor>();
        }
    }
}
