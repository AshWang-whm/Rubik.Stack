using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Vos
{
    internal class ErrorResponseVO
    {
        public ErrorResponseVO()
        {
            
        }

        public ErrorResponseVO(string error)
        {
            this.Error = error;
        }

        public ErrorResponseVO(string error,string desc)
        {
            this.Error=error;
            this.ErrorDescription=desc;
        }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription {  get; set; }
    }
}
