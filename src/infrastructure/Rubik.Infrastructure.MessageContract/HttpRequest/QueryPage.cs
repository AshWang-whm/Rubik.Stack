using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.Contract.HttpRequest
{
    public class QueryPage
    {
        public int Page { get; set; } = 1;

        public int Size { get; set; } = 200;
    }

    public class QueryPage<T>:QueryPage where T : class,new ()
    {
        public T Query { get; set; }=new T();
    }

    public class QueryPageDate : QueryDate
    {
        public int Page { get; set; } = 1;

        public int Size { get; set; } = 200;
    }

    public class QueryPageDate<T> : QueryPageDate where T:class,new ()
    {
        public T Query { get; set; } = new T();

    }


    public class QueryPageTimeRange : QueryTimeRange
    {
        public int Page { get; set; } = 1;

        public int Size { get; set; } = 200;
    }

    public class QueryPageTimeRange<T> :QueryPageTimeRange where T : class, new()
    {
        public T Query { get; set; } = new T();
    }
}
