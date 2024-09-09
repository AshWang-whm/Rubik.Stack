using FreeSql.DataAnnotations;
using System.Text.Json;

namespace Rubik.LowCode.Share.Entity
{
    [Table(Name ="tb_component_css")]
    public class TbComponentCssEntity:BaseComponentConfigEntity
    {
        public string? CssConfig { get; set; }

        [Column(IsIgnore = true)]
        public Dictionary<string, string> Dictionary => JsonSerializer.Deserialize<Dictionary<string, string>>(CssConfig ?? "{}") ?? [];
    }
}
