using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Oidc.Core.Configs
{
    public class RsaKeyConfig
    {
        public string RsaKeyFilePath { get; set; } = "oidc";

        public string? RsaKeyFileFullPath { get; set; }

        public string RsaKid { get; set; } = "rubik.oidc.server";
    }
}
