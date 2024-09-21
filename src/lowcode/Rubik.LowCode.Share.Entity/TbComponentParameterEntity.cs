using FreeSql.DataAnnotations;
using System.Text.Json;

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
