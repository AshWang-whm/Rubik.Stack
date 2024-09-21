using System.Text.Json.Serialization;

namespace Rubik.Infrastructure.Contract.HttpResponse
{
    /// <summary>
    /// 大写http response
    /// </summary>
    public interface IOutputHttpResponse
    {
        public int Code { get; set; }
        [JsonIgnore]
        public bool Success { set; get; }

        public string? Msg { get; set; }
    }

    /// <summary>
    /// 大写http response
    /// </summary>
    public interface IOutputHttpResponse<T>:IOutputHttpResponse
    {
        public T? Data { get;set; }
    }

    public interface IOutputHttpPageResponse<T> : IOutputHttpResponse
    {
        public long Total { get; set; }

        public IEnumerable<T>? Data { get; set; }
    }

    public class OutputHttpResponse: IOutputHttpResponse
    {
        public int Code { get; set; }
        [JsonIgnore]
        public bool Success { set; get; }

        public string? Msg { get; set; }

        public static OutputHttpResponse Ok(string? msg = null) => new() { Success = true, Msg = msg };

        public static OutputHttpResponse NotOk(string? msg = null) => new() { Success = false, Msg = msg };
    }

    public class OutputHttpResponse<T> : IOutputHttpResponse<T>
    {
        public int Code { get; set; }
        [JsonIgnore]
        public bool Success { set; get; }

        public string? Msg { get; set; }
        public T? Data { get; set; }

        public static OutputHttpResponse<T> Ok(T? data = default, string? msg = null, bool success = true)
        {
            return new OutputHttpResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg
            };
        }

        public static OutputHttpResponse<T> NotOk(T? data = default, string? msg = null, bool success = false)
        {
            return new OutputHttpResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg
            };
        }
    }

    public class OutputHttpPageResponse<T> : IOutputHttpPageResponse<T>
        where T : class
    {
        public int Code { get; set; }

        [JsonIgnore]
        public bool Success { get => Code == 200; set { Code = value ? 200 : 501; } }

        public IEnumerable<T>? Data { get; set; }

        public long Total { get; set; }

        public string? Msg { get; set; }

        public static OutputHttpPageResponse<T> Ok(IEnumerable<T>? data, long total, string? msg = null, bool success = true)
        {
            return new OutputHttpPageResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg,
                Total = total
            };
        }

        public static OutputHttpPageResponse<T> NotOk(IEnumerable<T>? data = default, long total = 0, string? msg = null, bool success = true)
        {
            return new OutputHttpPageResponse<T>()
            {
                Success = success,
                Data = data,
                Msg = msg,
                Total = total
            };
        }
    }

    public partial class HttpOutput
    {
        public static IOutputHttpResponse<T> Result<T>(T data, string? msg, bool success) where T : class
        {
            return success ? OutputHttpResponse<T>.Ok(data, msg) : OutputHttpResponse<T>.NotOk(data, msg);
        }

        public static IOutputHttpResponse<T> Ok<T>(T? data = default, string? msg = null) where T : class
        {
            return OutputHttpResponse<T>.Ok(data, msg);
        }
        public static IOutputHttpResponse<T> NotOk<T>(T? data = default, string? msg = null) where T : class
        {
            return OutputHttpResponse<T>.NotOk(data, msg);
        }

        public static IOutputHttpResponse Ok(string? msg = null) => OutputHttpResponse.Ok(msg);
        public static IOutputHttpResponse NotOk(string? msg = null) => OutputHttpResponse.NotOk(msg);

        public static IOutputHttpPageResponse<T> PageOk<T>(IEnumerable<T>? data, long total, string? msg = null) where T : class
        {
            return OutputHttpPageResponse<T>.Ok(data, total, msg);
        }

        public static IOutputHttpPageResponse<T> PageNotOk<T>(IEnumerable<T>? data = default, long total = 0, string? msg = null) where T : class
        {
            return OutputHttpPageResponse<T>.Ok(data, total, msg);
        }
    }
}
