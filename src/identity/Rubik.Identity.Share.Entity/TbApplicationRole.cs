using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_application_role")]
    public class TbApplicationRole:BaseFullEntity
    {
        [Column(IsNullable =false)]
        public string? Name { get; set; }

        [Column(IsNullable =false)]
        public int ApplicationID { get; set; }
    }
}
