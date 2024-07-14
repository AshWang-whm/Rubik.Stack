using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name = "tb_organization")]
    public class TbOrganization: BaseTreeEntity
    {
        [Column(MapType = typeof(int), IsNullable = false)]
        public OrganizationType OrganizationType { get; set; } = OrganizationType.Department;
        
        [Column(IsNullable =true)]
        public string? Description { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public List<TbOrganizationJob>? Jobs { get; set; }
    }

    public enum OrganizationType
    {

        [Description("部门")]
        Department,

        [Description("协会")]
        Association,

    }
}
