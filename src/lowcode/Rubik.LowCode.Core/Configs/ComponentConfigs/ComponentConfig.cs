using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.LowCode.Core.Configs.ComponentConfigs
{
    public record ComponentConfig
    {
        public int ID { get; set; }

        public required string Name { get; set; }


    }
}
