using FreeSql.DataAnnotations;
using Rubik.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubik.LowCode.Share.Entity
{
    /// <summary>
    /// 组件初始化参数配置
    /// </summary>
    [Table(Name = "tb_component_config")]
    public class TbComponentParameterEntity : BaseComponentConfigEntity
    {
        public string? ParameterConfig { get; set; }

        [Column(IsIgnore = true)]
        public Dictionary<string, object> Dictionary => JsonSerializer.Deserialize<Dictionary<string, object>>(ParameterConfig ?? "{}") ??[];
    }


}
