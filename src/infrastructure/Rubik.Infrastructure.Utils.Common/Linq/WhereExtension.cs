using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubik.Infrastructure.Utils.Common.Linq
{
    public static class WhereExtension
    {
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
    }
}
