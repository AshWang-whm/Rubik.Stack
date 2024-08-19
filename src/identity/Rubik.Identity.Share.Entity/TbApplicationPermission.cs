using System.ComponentModel;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_application_permission")]
    public class TbApplicationPermission: BaseTreeEntity<TbApplicationPermission>
    {
        [Column(IsNullable =false)]
        public PermissionType Type { get; set; }

        [Column(IsNullable =false)]
        public int ApplicationID { get; set; }

        [Column(IsNullable =false,Position =8)]
        public PageType PageType { get; set; }

        [Column(IsNullable = true, Position = 10)]
        public string? Url { get; set; }

        [Column(IsNullable =true,Position =11)]
        public string? Icon {  get; set; }

        [Column(IsNullable =true,Position =12)]
        public string? Description { get; set; }
    }

    public enum PermissionType
    {
        [Description("分组")]
        Group,
        [Description("页面")]
        Page,
        [Description("权限点")]
        Node
    }

    public enum PageType
    {
        [Description("系统页面")]
        Internal,
        [Description("外部页面")]
        External
    }
}
