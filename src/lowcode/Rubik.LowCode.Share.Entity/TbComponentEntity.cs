using FreeSql.DataAnnotations;
using Rubik.Infrastructure.Entity.BaseEntity;

namespace Rubik.LowCode.Share.Entity
{
    [Table(Name ="tb_component")]
    public class TbComponentEntity : BaseTreeEntity<TbComponentEntity>
    {
        [Column(IsNullable = true, MapType = typeof(sbyte?))]
        public ComponentType ComponentType { get; set; }


        [Navigate(nameof(TbComponentParameterEntity.ComponentID))]
        public List<TbComponentParameterEntity> Parameters { get; set; }=[];

        //TbComponentCssEntity
        [Navigate(nameof(TbComponentCssEntity.ComponentID))]
        public List<TbComponentCssEntity> Css { get; set; } = [];

        public string? ToPageParameter()
        {
            if (Parameters.Count ==0)
                return null;
            return string.Join("/", Parameters[0].Dictionary.Select(a => a.Value));
        }

        public Dictionary<string,object>? ToBlazorComponentParameter()
        {
            return Parameters.Count==0?null : Parameters[0].Dictionary;
        }
    }

    public enum ComponentType
    {
        PageGroup,
        Page,
        Table,
        Form,
        List,
        Row,
        Input,
        Select,
        CheckBox,
        RadioGroup,
        Button,
        PopComfirm
    }
}
