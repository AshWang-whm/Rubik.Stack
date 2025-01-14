using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name = "tb_role")]
    [Index("uq_code", "Code", IsUnique = true)]
    public class TbRole :  BaseTreeEntity<TbRole>
    {
        [Column(IsNullable = true)]
        public string? Description { get; set; }
    }
}
