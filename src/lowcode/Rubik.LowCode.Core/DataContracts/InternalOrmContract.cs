using Rubik.LowCode.Core.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.LowCode.Core.DataWrapper
{
    /// <summary>
    /// Freesql 查询结果
    /// </summary>
    [Description("内置Orm协议")]
    public class InternalOrmContract : IContractConvert
    {
        public DataSourceContract DataSourceConvert()
        {
            throw new NotImplementedException();
        }
    }
}
