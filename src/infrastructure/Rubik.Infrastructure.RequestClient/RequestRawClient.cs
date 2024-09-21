using Rubik.Infrastructure.Contract.Extensions;
using System.Text;
using System.Text.Json;

namespace Rubik.Infrastructure.RequestClient
{
    public partial class RequestClient
    {
        public async Task<TResult?> CallApiRawOutput<TResult>(string url, HttpContent? content, string method = "POST", string? clientname = null)
            where TResult : class
        {
            if (clientname == null)
            {
                return default;
            }
            var client = httpClientFactory.CreateClient(clientname) ?? throw new Exception($"Client Option:[{clientname}] Not Found!");
            try
            {
                var hrm = await CallHttpResponseMessage(url, client, content, method);
                if (hrm == null)
                {
                    return default;
                }
                var response = await hrm.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TResult>(response, JsonSerializerOption);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                content?.Dispose();
            }
        }

        public async Task<TResult?> PostRawApi<TResult>(string url, object? parameter = null, string clientname = ClientName)
            where TResult : class
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json");
            return await CallApiRawOutput<TResult>(url, content, clientname);
        }

        /// <summary>
        /// get 原始api
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="mediaType"></param>
        /// <param name="method"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<TResult?> GetRawApi<TResult>(string url,object? parameter = null,string mediaType= "text/plain", string method="GET", string clientname = ClientName)
            where TResult : class
        {
            var rawurl = parameter.ToQueryString(url);
            return await CallApiRawOutput<TResult>(rawurl, null, method, clientname);
        }

        /// <summary>
        /// get restful api
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="mediaType"></param>
        /// <param name="method"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<TResult?> GetRestApi<TResult>(string url, object? parameter = null, string mediaType = "text/plain", string method = "GET", string clientname = ClientName)
            where TResult : class
        {
            var rawurl = parameter.ToQueryString(url);
            return await CallApiRawOutput<TResult>(rawurl, null, method, clientname);
        }
    }
}
