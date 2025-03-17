using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name = "tb_oidc_api_scope")]
    public class TbApiScope:BaseFullEntity
    {
        [Column(IsNullable =false)]
        public int ApiID { get; set; }

        public string? Claims { get; set; }

        [Navigate(nameof(ApiID))]
        public TbApiResource? ApiResource { get; set; }
    }
}
