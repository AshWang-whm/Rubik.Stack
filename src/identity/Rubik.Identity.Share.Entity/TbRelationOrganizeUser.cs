using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name = "tb_relation_organization_user")]
    public class TbRelationOrganizeUser: BaseNewEntity
    {
        public int OrganizationID { get; set; }

        public int UserID { get; set; }
    }
}
