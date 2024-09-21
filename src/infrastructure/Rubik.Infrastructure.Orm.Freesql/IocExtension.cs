using FreeSql;
using Microsoft.Extensions.DependencyInjection;

namespace Rubik.Infrastructure.Freesql
{
    public static class IocExtension
    {
        /// <summary>
        /// 添加freesql
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="conkey"></param>
        /// <param name="dataType"></param>
        public static IFreeSql AddFreesqlOrm(this IServiceCollection services, string connectionstring, DataType dataType = DataType.MySql
            ,Action<System.Data.Common.DbCommand>? executing=null
            ,EventHandler<FreeSql.Aop.AuditValueEventArgs>? aop=null)
        {
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
        public static IFreeSql<TFlag> AddFreesqlOrm<TFlag>(this IServiceCollection services, string connectionstring, DataType dataType = DataType.MySql
            , Action<System.Data.Common.DbCommand>? executing = null
            , EventHandler<FreeSql.Aop.AuditValueEventArgs>? aop = null)
        {
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
