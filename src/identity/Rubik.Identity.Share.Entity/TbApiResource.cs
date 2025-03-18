using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_oidc_api_resource")]
    public class TbApiResource: BaseFullEntity
    {

        [Column(IsIgnore =true)]
        public string? Scopes { get; set; }
    }
}
