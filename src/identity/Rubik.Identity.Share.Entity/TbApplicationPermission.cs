using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_application_permission")]
    public class TbApplicationPermission:BaseFullEntity
    {
        [Column(IsNullable =false)]
        public string? Name { get; set; }

        [Column(IsNullable =true)]
        public string? Code { get;set; }

        [Column(IsNullable =false)]
        public PermissionType Type { get; set; }


        [Column(IsNullable =false)]
        public int ApplicationID { get; set; }

        [Column(IsNullable =true,Position =10)]
        public string? Url { get; set; }

    }

    public enum PermissionType
    {
        [Description("分组")]
        Group,
        [Description("菜单")]
        Menu,
        [Description("权限点")]
        Node
    }
}
