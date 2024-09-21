using System.Linq;
using System.Text;

namespace Rubik.Infrastructure.Contract.Extensions
{
    public static class QueryStringExtension
    {
        public static string ToQueryString<T>(this T? obj,string url)
            where T : class
        {
            var parameter = ToQueryString(obj as object);
            var query = $"{url.TrimEnd('/')}{parameter}";
            return query;
        }

        public static string ToQueryString<T>(this T? obj)
            where T : class
        {
            return ToQueryString(obj as object);
        }

        public static string ToQueryString(object? obj)
        {
            if (obj == null)
                return "";
            var props = obj.GetType().GetProperties().ToList();
            // 默认长度是属性数量*10
            var sb = new StringBuilder(props.Count * 20);

            foreach (var item in props)
            {
                var prefix = props.IndexOf(item) == 0 ? "?" : "&";
                sb.Append($"{prefix}{item.Name}={item.GetValue(obj)?.ToString()}");
            }
            return sb.ToString();
        }

        public static string ToRestQueryString<T>(this T? obj,string url)
            where T: class
        {
            var parameter = ToRestQueryString(obj as object);
            var query = $"{url.TrimEnd('/')}{parameter}";
            return query;
        }

        public static string ToRestQueryString<T>(this T? obj)
            where T : class
        {
            return ToRestQueryString(obj as object);
        }

        public static string ToRestQueryString(object? obj)
        {
            if (obj == null)
                return "/";
            var props = obj.GetType().GetProperties()
                .Select(a => a.GetValue(obj))
                .Where(a=>a!=null);
            return $"/{string.Join("/", props)}";
        }


    }
}
