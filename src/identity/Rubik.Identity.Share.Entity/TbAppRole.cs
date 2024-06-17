using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_approle")]
    public class TbAppRole:BaseFullEntity
    {
        [Column(IsNullable =false)]
        public int AppID { get; set; }

        [Column(IsNullable =false)]
        public string? Name { get; set; }

        [Column(IsNullable =false)]
        public string? Code { get; set; }

        [Column(IsNullable =true)]
        public int? ParentID { get; set; }
    }
}
