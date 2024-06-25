using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    /// <summary>
    /// 部门岗位
    /// </summary>
    [Table(Name ="tb_department_post")]
    public class TbDepartmentPost:BaseFullEntity
    {
        [Column(IsNullable =false,Position =2)]
        public string? Name { get; set; }

        public int DepartmentID { get; set; }

        [Column(IsNullable =true)]
        public int? ParentID { get; set; }

        public TbDepartment? Department { get; set; }
    }
}
