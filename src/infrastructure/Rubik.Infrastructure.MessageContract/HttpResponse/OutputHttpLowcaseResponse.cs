using System.Text.Json.Serialization;

namespace Rubik.Infrastructure.Contract.HttpResponse
{
    /// <summary>
    /// 大写http response
    /// </summary>
    public interface IOutputHttpLowcaseResponse
    {
        public int Code { get; set; }

        [JsonIgnore]
        public bool Success { set; get; }

        public string? Msg { get; set; }
    }

    /// <summary>
    /// 大写http response
    /// </summary>
    public interface IOutputHttpLowcaseResponse<T>:IOutputHttpLowcaseResponse
    {
        public T? Data { get;set; }
    }

    public interface IOutputHttpPageLowcaseResponse<T> : IOutputHttpLowcaseResponse
    {
        public long Total { get; set; }

        public IEnumerable<T>? Data { get; set; }
    }

    public class OutputHttpLowcaseResponse: IOutputHttpLowcaseResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonIgnore]
        public bool Success { set; get; }

        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        public static OutputHttpLowcaseResponse Ok(string? msg = null) => new() { Success = true, Msg = msg };

        public static OutputHttpLowcaseResponse NotOk(string? msg = null) => new() { Success = false, Msg = msg };
    }

    public class OutputHttpLowcaseResponse<T> : IOutputHttpLowcaseResponse<T>
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonIgnore]
        public bool Success { set; get; }

        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public static OutputHttpLowcaseResponse<T> Ok(T? data = default, string? msg = null, bool success = true)
        {
            return new OutputHttpLowcaseResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg
            };
        }

        public static OutputHttpLowcaseResponse<T> NotOk(T? data = default, string? msg = null, bool success = false)
        {
            return new OutputHttpLowcaseResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg
            };
        }
    }

    public class OutputHttpPageLowcaseResponse<T> : IOutputHttpPageLowcaseResponse<T>
        where T : class
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonIgnore]
        public bool Success { get => Code == 200; set { Code = value ? 200 : 501; } }

        [JsonPropertyName("data")]
        public IEnumerable<T>? Data { get; set; }

        [JsonPropertyName("total")]
        public long Total { get; set; }

        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        public static OutputHttpPageLowcaseResponse<T> Ok(IEnumerable<T>? data, long total, string? msg = null, bool success = true)
        {
            return new OutputHttpPageLowcaseResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg,
                Total = total
            };
        }

        public static OutputHttpPageLowcaseResponse<T> NotOk(IEnumerable<T>? data = default, long total = 0, string? msg = null, bool success = true)
        {
            return new OutputHttpPageLowcaseResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg,
                Total = total
            };
        }
    }

    public partial class HttpLowcaseOutput
    {
        public static IOutputHttpLowcaseResponse<T> Result<T>(T data, string? msg, bool success) where T : class
        {
            return success ? OutputHttpLowcaseResponse<T>.Ok(data, msg) : OutputHttpLowcaseResponse<T>.NotOk(data, msg);
        }

        public static IOutputHttpLowcaseResponse<T> Ok<T>(T? data = default, string? msg = null) where T : class
        {
            return OutputHttpLowcaseResponse<T>.Ok(data, msg);
        }
        public static IOutputHttpLowcaseResponse<T> NotOk<T>(T? data = default, string? msg = null) where T : class
        {
            return OutputHttpLowcaseResponse<T>.NotOk(data, msg);
        }

        public static IOutputHttpLowcaseResponse Ok(string? msg = null) => OutputHttpLowcaseResponse.Ok(msg);
        public static IOutputHttpLowcaseResponse NotOk(string? msg = null) => OutputHttpLowcaseResponse.NotOk(msg);

        public static OutputHttpPageLowcaseResponse<T> PageOk<T>(IEnumerable<T>? data, long total, string? msg = null) where T : class
        {
            return OutputHttpPageLowcaseResponse<T>.Ok(data, total, msg);
        }

        public static OutputHttpPageLowcaseResponse<T> PageNotOk<T>(IEnumerable<T>? data = default, long total = 0, string? msg = null) where T : class
        {
            return OutputHttpPageLowcaseResponse<T>.Ok(data, total, msg);
        }
    }
}
