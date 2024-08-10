using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rubik.Share.Entity.BaseEntity;
using System.Security.Principal;

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
            ,Action<System.Data.Common.DbCommand>? executing=null)
        {
            var fsql = new FreeSqlBuilder()
                    .UseConnectionString(dataType, builder.Configuration.GetConnectionString(conkey))
            .UseMonitorCommand(cmd =>
            {
                executing?.Invoke(cmd);
            })
            .Build();

            var provider = builder.Services.BuildServiceProvider();
            //var identity = provider.GetService<Identity>();

            fsql.Aop.AuditValue += (obj, args) =>
            {
                var usercode = "Test";
                //var usercode = identity?.UserCode;
                // 审计数据
                if (args.Object is IFullEntity @entity)
                {
                    if (args.AuditValueType == FreeSql.Aop.AuditValueType.Insert)
                    {
                        @entity.AddDate = DateTime.Now;
                        @entity.AddUser = usercode;
                        @entity.ModifyUser = usercode;
                        entity.ModifyDate = DateTime.Now;
                    }
                    else if (args.AuditValueType == FreeSql.Aop.AuditValueType.Update)
                    {
                        @entity.ModifyUser = usercode;
                        entity.ModifyDate = DateTime.Now;
                    }
                }
                else if (args.Object is INewEntity @new && args.AuditValueType == FreeSql.Aop.AuditValueType.Insert)
                {
                    @new.AddDate = DateTime.Now;
                    @new.AddUser = usercode;
                }
                else if (args.Object is IModifyEntity @modify && args.AuditValueType == FreeSql.Aop.AuditValueType.Update)
                {
                    modify.ModifyDate = DateTime.Now;
                    modify.ModifyUser = usercode;
                }

                args.ObjectAuditBreak = true;
            };


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
