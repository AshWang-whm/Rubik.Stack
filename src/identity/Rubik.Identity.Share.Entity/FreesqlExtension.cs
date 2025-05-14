using Rubik.Identity.Share.Extension;
using System.Reflection;
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
                typeof(TbUser),typeof(TbRole),
                typeof(TbApiResource),typeof(TbApiScope)
            ];
            foreach (var type in types)
            {
                if (!freeSql.DbFirst.ExistsTable(type.GetCustomAttribute<TableAttribute>()!.Name))
                {
                    freeSql.CodeFirst.SyncStructure(type);
                }
            }


            // 初始化管理员账号
            var admin = new TbUser
            {
                Code = "admin",
                Name = "管理员",
                Password = PasswordEncryptExtension.GeneratePasswordHash("admin", "admin"),
                AddDate = DateTime.Now,
                IsDelete = false
            };
            // 
            if (!await freeSql.Select<TbUser>().AnyAsync(a => a.Code == "admin"))
                await freeSql.Insert(admin).ExecuteAffrowsAsync();
            System.Console.WriteLine("初始化完成");
        }
    }
}
