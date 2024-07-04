using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_department")]
    public class TbDepartment: BaseTreeEntity
    {
        
        [Column(IsNullable =true)]
        public string? Description { get; set; }

        public List<TbDepartmentJob>? Posts { get; set; }
    }
}
