using FreeSql.DataAnnotations;

namespace Rubik.Identity.Admin.Extensions
{
    [ExpressionCall]
    public static class FreesqlExpressions
    {
        //必要定义 static + ThreadLocal
        static ThreadLocal<ExpressionCallContext> context = new ThreadLocal<ExpressionCallContext>();

        public static string StringAgg(this string? that, string separator)
        {
            var up = context.Value;
            if (up.DataType == FreeSql.DataType.PostgreSQL) //重写内容
                up.Result = $"STRING_AGG({up.ParsedContent["that"]},'{separator}')";
            return that;
        }
    }
}
