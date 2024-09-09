using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.LowCode.Core.DataContracts
{
    /// <summary>
    /// 渲染DataTable组件的数据协议
    /// </summary>
    public class DataSourceContract
    {
        public DataTable? Data { get; set; }

        public int Total { get; set; }
    }
}
