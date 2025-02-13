using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.OAuth
{
    public static class Extensions
    {
        public static void AddUserIdentity(this IServiceCollection services)
        {
            services.AddScoped<Identity>();
        }
    }
}
