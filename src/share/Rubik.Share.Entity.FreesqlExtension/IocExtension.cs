using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rubik.Identity.UserIdentity;
using Rubik.Infrastructure.Entity.BaseEntity;
using Rubik.Infrastructure.Orm.Freesql;

namespace Rubik.Identity.FreesqlExtension
{
    public static class IocExtension
    {
        /// <summary>
        /// 添加freesql以及UserIdentityAccessor的aop注入，控制insert/update的操作用户
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="conkey"></param>
        /// <param name="dataType"></param>
        public static IFreeSql AddFreesqlWithIdentityAop(this WebApplicationBuilder builder, string conkey, DataType dataType = DataType.PostgreSQL
            ,Action<System.Data.Common.DbCommand>? executing=null)
        {
            builder.Services.AddUserIdentity();

            var provider = builder.Services.BuildServiceProvider();

            var userIdentity = provider.GetRequiredService<UserIdentityAccessor>();

            executing ??= (cmd) =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
            };

            var freesql = builder.Services.AddFreesqlOrm(connectionstring: builder.Configuration.GetConnectionString(conkey)!
                ,dataType: dataType
                ,executing: executing
                ,aop: (obj, args) =>
                {
                    var usercode = userIdentity?.UserCode;
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
                });

            return freesql!;
        }
    }
}
