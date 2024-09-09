using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rubik.LowCode.Core.DataContracts
{
    [Description("内置API协议")]
    public class InternalApiContract : IContractConvert
    {
        public int Code { get; set; }
        [JsonIgnore]
        public bool Success { set; get; }

        public string? Msg { get; }

        public string? Data { get; set; }

        public JsonDocument? JsonDocument => JsonDocument.Parse(Data ??"{}");

        public DataSourceContract DataSourceConvert()
        {
            throw new NotImplementedException();
        }
    }
}
