using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.Contract.HttpRequest
{
    public class QueryDate
    {
        public DateTime Date { get; set; }
    }

    public class QueryDate<T>:QueryDate where T : class,new()
    {
        public T Query { get; set; } = new T();
    }
}
