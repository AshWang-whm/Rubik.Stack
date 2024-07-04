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
    [Table(Name ="tb_department_job")]
    public class TbDepartmentJob: BaseTreeEntity
    {
        public int DepartmentID { get; set; }

        public TbDepartment? Department { get; set; }
    }
}
