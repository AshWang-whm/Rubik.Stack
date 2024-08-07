
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_relation_role_user")]
    public class TbRelationRoleUser:BaseNewEntity
    {
        public int RoleID { get; set; }

        public int UserID { get; set; }
    }
}
