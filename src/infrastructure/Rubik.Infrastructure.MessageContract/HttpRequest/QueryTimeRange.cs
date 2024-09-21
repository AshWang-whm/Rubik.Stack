using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.Contract.HttpRequest
{
    public class QueryTimeRange
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? Begin { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? End { get; set; }
    }

    public class QueryTimeRange<T>:QueryTimeRange where T : class,new()
    {
        public T Query { get; set; } = new T();
    }
}
