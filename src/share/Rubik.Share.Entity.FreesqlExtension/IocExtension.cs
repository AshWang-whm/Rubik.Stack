using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rubik.Infrastructure.Entity.BaseEntity;
using Rubik.Infrastructure.Freesql;

namespace Rubik.Share.Entity.FreesqlExtension
{
    public static class IocExtension
    {
        /// <summary>
        /// 添加freesql
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="conkey"></param>
        /// <param name="dataType"></param>
        public static IFreeSql AddFreesql(this WebApplicationBuilder builder, string conkey, DataType dataType = DataType.PostgreSQL
            ,Action<System.Data.Common.DbCommand>? executing=null)
        {
            var provider = builder.Services.BuildServiceProvider();

            var freesql = builder.Services.AddFreesqlOrm(connectionstring: builder.Configuration.GetConnectionString(conkey)!
                ,dataType: dataType
                ,executing: executing
                ,aop: (obj, args) =>
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
                });

            return freesql!;
        }
    }
}
