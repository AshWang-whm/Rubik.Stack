using FreeSql;
using Microsoft.Extensions.DependencyInjection;

namespace Rubik.Infrastructure.Orm.Freesql
{
    public static class IocExtension
    {
        /// <summary>
        /// 添加freesql
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="conkey"></param>
        /// <param name="dataType"></param>
        public static IFreeSql AddFreesqlOrm(this IServiceCollection services, string connectionstring, DataType dataType = DataType.PostgreSQL
            ,Action<System.Data.Common.DbCommand>? executing=null
            ,EventHandler<FreeSql.Aop.AuditValueEventArgs>? aop=null)
        {
            executing ??= (cmd) =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
            };

            var fsql = new FreeSqlBuilder()
                    .UseConnectionString(dataType, connectionstring)
            .UseMonitorCommand(cmd =>
            {
                executing?.Invoke(cmd);
            })
            .Build();
            fsql.Aop.AuditValue += aop;
            services.AddSingleton(fsql);
            return fsql;
        }

        /// <summary>
        /// 添加带flag的freesql
        /// </summary>
        /// <typeparam name="TFlag">Flag类型</typeparam>
        /// <param name="builder"></param>
        /// <param name="conkey"></param>
        /// <param name="dataType"></param>
        public static IFreeSql<TFlag> AddFreesqlOrm<TFlag>(this IServiceCollection services, string connectionstring, DataType dataType = DataType.PostgreSQL
            , Action<System.Data.Common.DbCommand>? executing = null
            , EventHandler<FreeSql.Aop.AuditValueEventArgs>? aop = null)
        {
            executing ??= (cmd) =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
            };

            var fsql = new FreeSqlBuilder()
                    .UseConnectionString(dataType, connectionstring)
            .UseMonitorCommand(cmd =>
            {
                executing?.Invoke(cmd);
            })
            .Build<TFlag>();
            fsql.Aop.AuditValue += aop;
            services.AddSingleton(fsql);
            return fsql;
        }

    }
}
