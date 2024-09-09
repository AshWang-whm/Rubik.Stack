using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.LowCode.Core.Configs.ComponentConfigs
{
    public record FieldBindingConfig
    {
        [Description("数据字段")]
        public required string DataField { get; set; }


    }
}
