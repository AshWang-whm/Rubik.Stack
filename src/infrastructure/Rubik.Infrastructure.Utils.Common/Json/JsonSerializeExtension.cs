using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubik.Infrastructure.Utils.Common.Json
{
    public static class JsonSerializeExtension
    {
        /// <summary>
        /// 默认序列化配置,大小写不敏感,日期格式:DateTimeConverterUsingDateTimeParse
        /// </summary>
        public static JsonSerializerOptions DefaultJsonSerializerOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = null,
        };
        /// <summary>
        /// 自定义序列化配置
        /// </summary>
        public static JsonSerializerOptions CustomJsonSerializerOption = new JsonSerializerOptions();
        static JsonSerializeExtension()
        {
            DefaultJsonSerializerOption.Converters.Add(new DateTimeConverterUsingDateTimeParse());
        }

        public static string Serialize<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, DefaultJsonSerializerOption);
        }

        public static T? Deserialize<T>(this string json)
            where T : class
        {
            return JsonSerializer.Deserialize<T>(json,DefaultJsonSerializerOption);
        }

        public static string CustomSerialize<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, CustomJsonSerializerOption);
        }

        public static T? CustomDeserialize<T>(this string json)
            where T: class
        {
            return JsonSerializer.Deserialize<T>(json, CustomJsonSerializerOption);
        }
    }

    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? string.Empty, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
