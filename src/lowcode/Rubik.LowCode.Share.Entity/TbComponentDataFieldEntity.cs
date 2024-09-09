using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.LowCode.Share.Entity
{
    [Table(Name ="tb_component_datafield")]
    public class TbComponentDataFieldEntity:BaseComponentConfigEntity
    {
        public string? DataField { get; set; }

    }
}
