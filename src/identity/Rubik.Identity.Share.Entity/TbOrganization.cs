
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name = "tb_organization")]
    public class TbOrganization: BaseTreeEntity<TbOrganization>
    {
        [Column(MapType = typeof(int), IsNullable = false)]
        public OrganizationType OrganizationType { get; set; } = OrganizationType.Department;
        
        [Column(IsNullable =true)]
        public string? Description { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        [Column(IsIgnore =true)]
        public List<TbOrganizationJob>? Jobs { get; set; }
    }

    public enum OrganizationType
    {

        [Display(Name="部门")]
        Department,

        [Display(Name ="协会")]
        Association,

    }
}
