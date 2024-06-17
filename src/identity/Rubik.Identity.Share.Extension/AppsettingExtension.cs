using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avd.Infrastructure.Freesql
{
    public static class AppsettingExtension
    {
        /// <summary>
        /// 添加环境变量的json配置文件
        /// </summary>
        /// <param name="builder"></param>
        public static void AddEnvironmentJsonFile(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");
        }

        public static void AddJsonFile(this WebApplicationBuilder builder,string file)
        {
            builder.Configuration.AddJsonFile($"{file}");
        }
    }
}
