
namespace Rubik.Infrastructure.Utils.Common.String
{
    public static class StringUtil
    {
        public static bool IsNull(this string? str) 
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNull(this string? str)
            => !IsNull(str);

        public static string StringJoin(this IEnumerable<string?> list, string separator=",") 
            => string.Join(separator, list.Where(a=>a.IsNotNull()));

        public static string StringJoin(this IEnumerable<string?> list, char separator)
            => string.Join(separator, list.Where(a => a.IsNotNull()));
    }
}
