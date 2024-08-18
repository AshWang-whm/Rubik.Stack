using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name = "tb_relation_job_user")]
    public class TbRelationJobUser : BaseNewEntity
    {
        public int JobID { get; set; }

        public int UserID { get; set; }
    }
}
