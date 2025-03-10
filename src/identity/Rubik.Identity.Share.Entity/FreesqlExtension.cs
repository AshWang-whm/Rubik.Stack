﻿using System.Reflection;
namespace Rubik.Identity.Share.Entity
{
    public static class FreesqlExtension
    {
        public static async Task DbInitialize(this IFreeSql freeSql)
        {
            if (!freeSql.Ado.MasterPool.IsAvailable)
            {
                Console.WriteLine("数据库不可用");
                return;
            }

            List<Type> types =
            [
                typeof(TbApplication), typeof(TbApplicationPermission),typeof(TbApplicationRole),
                typeof(TbOrganization),typeof(TbOrganizationJob),typeof(TbPosition),
                typeof(TbRelationOrganizeUser),typeof(TbRelationRolePermission),typeof(TbRelationRoleUser),typeof(TbRelationPositionUser),typeof(TbRelationJobUser),
                typeof(TbUser),typeof(TbRole)
            ];
            foreach (var type in types)
            {
                if (!freeSql.DbFirst.ExistsTable(type.GetCustomAttribute<TableAttribute>()!.Name))
                {
                    freeSql.CodeFirst.SyncStructure(type);
                }
            }


            // test
            //var a = freeSql.Select<TbDepartment,TbDepartmentPost>()
            //    .LeftJoin((a,b)=>a.ID==b.DepartmentID)

            //await freeSql.Insert(new TbAppRole
            //{
            //    AddDate = DateTime.Now,
            //    ModifyDate = DateTime.Now,
            //    Code="test",
            //    AppID=1,
            //    Name="name",
                
            //}).ExecuteAffrowsAsync();


            //// 初始化管理员账号
            //var admin = new users
            //{
            //    UserName = "admin",
            //    Name = "管理员",
            //    Permissions = String.Join(",", Enum.GetValues<AdminPermission>().Select(a => a.ToDescriptionString())),
            //    Password = "admin".CreateMd5(),
            //    AddDate = DateTime.Now,
            //    IsDelete = false
            //};
            //// 
            //if (!await freeSql.Select<users>().AnyAsync(a => a.UserName == "admin"))
            //    await freeSql.Insert(admin).ExecuteAffrowsAsync();
            System.Console.WriteLine("初始化完成");
        }
    }
}
