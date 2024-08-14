
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
    [Table(Name = "tb_organization_job")]
    public class TbOrganizationJob: BaseFullEntity
    {
        [Column(IsNullable =false)]
        public int OrganizationID { get; set; }

        [Navigate(nameof(OrganizationID))]
        public TbOrganization? Organization { get; set; }
    }
}
