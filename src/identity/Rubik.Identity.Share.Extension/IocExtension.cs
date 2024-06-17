using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Avd.Infrastructure.Freesql
{
    public static class IocExtension
    {
        /// <summary>
        /// 添加freesql
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="conkey"></param>
        /// <param name="dataType"></param>
        public static IFreeSql AddFreesql(this WebApplicationBuilder builder, string conkey, DataType dataType = DataType.MySql
            ,Action<System.Data.Common.DbCommand> executing=null
            ,EventHandler<FreeSql.Aop.AuditValueEventArgs> aop=null)
        {
            var fsql = new FreeSqlBuilder()
                    .UseConnectionString(dataType, builder.Configuration.GetConnectionString(conkey))
            .UseMonitorCommand(cmd =>
            {
                executing?.Invoke(cmd);
            })
            .Build();
            fsql.Aop.AuditValue += aop;
            builder.Services.AddSingleton(fsql);
            return fsql;
        }

        /// <summary>
        /// 添加带flag的freesql
        /// </summary>
        /// <typeparam name="TFlag">Flag类型</typeparam>
        /// <param name="builder"></param>
        /// <param name="conkey"></param>
        /// <param name="dataType"></param>
        public static IFreeSql<TFlag> AddFreesql<TFlag>(this WebApplicationBuilder builder,string conkey, DataType dataType = DataType.MySql
            , Action<System.Data.Common.DbCommand> executing = null
            , EventHandler<FreeSql.Aop.AuditValueEventArgs> aop = null)
        {
            var fsql = new FreeSqlBuilder()
                    .UseConnectionString(dataType, builder.Configuration.GetConnectionString(conkey))
            .UseMonitorCommand(cmd =>
            {
                executing?.Invoke(cmd);
            })
            .Build<TFlag>();
            fsql.Aop.AuditValue += aop;
            builder.Services.AddSingleton(fsql);
            return fsql;
        }
    }
}
