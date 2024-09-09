using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.LowCode.Share.Entity
{
    [Table(Name ="tb_component_datasource")]
    public class TbComponentDataSourceEntity:BaseComponentConfigEntity
    {
        [Column(IsNullable =false,DbType ="text")]
        public string? DataSource { get; set; }


    }
}
