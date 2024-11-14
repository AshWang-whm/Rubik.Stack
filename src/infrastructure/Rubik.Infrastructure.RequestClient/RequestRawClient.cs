using Rubik.Infrastructure.Contract.Extensions;
using System.Text;
using System.Text.Json;

namespace Rubik.Infrastructure.RequestClient
{
    public partial class RequestClient
    {
        internal async Task<string> CallApiRawContent(string url, HttpContent? content, HttpMethodType method = HttpMethodType.POST, string? clientname = null)
        {
            try
            {
                var hrm = await CallHttpResponseMessage(url, content, method, clientname);
                if (hrm == null)
                {
                    throw new Exception("HttpResponseMessage Is Null!");
                }
                else if (!hrm.IsSuccessStatusCode)
                {
                    throw new Exception(hrm.ReasonPhrase);
                }
                var json = await hrm.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception($"Url:{url}, Err:{ex.Message}");
            }
            finally
            {
                content?.Dispose();
            }
        }

        public async Task<string> PostApiForRawContent(string url, object? parameter = null, string mediaType = "application/json", string clientname = ClientName)
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, mediaType);
            return await CallApiRawContent(url,content, HttpMethodType.POST, clientname);
        }

        public async Task<string> GetApiForRawContent(string url, object? parameter = null, HttpMethodType method = HttpMethodType.GET, string clientname = ClientName)
        {
            var rawurl = parameter.ToQueryString(url);
            return await CallApiRawContent(rawurl,null, HttpMethodType.GET, clientname);
        }

        public async Task<string> GetRestApiForRawContent(string url, object? parameter = null, HttpMethodType method = HttpMethodType.GET, string clientname = ClientName)
        {
            var rawurl = parameter.ToRestQueryString(url);
            return await CallApiRawContent(rawurl, null, HttpMethodType.GET, clientname);
        }

        public async Task<TResult?> PostRawApi<TResult>(string url, object? parameter = null, string mediaType = "application/json", string clientname = ClientName)
            where TResult : class
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, mediaType);
            return await InternalCallApi<TResult>(url, content, HttpMethodType.POST, clientname);
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
        public async Task<TResult?> GetRawApi<TResult>(string url,object? parameter = null, HttpMethodType method = HttpMethodType.GET, string clientname = ClientName)
            where TResult : class
        {
            var rawurl = parameter.ToQueryString(url);
            return await InternalCallApi<TResult>(rawurl, null, method, clientname);
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
        public async Task<TResult?> GetRestApi<TResult>(string url, object? parameter = null, HttpMethodType method = HttpMethodType.GET, string clientname = ClientName)
            where TResult : class
        {
            var rawurl = parameter.ToRestQueryString(url);
            return await InternalCallApi<TResult>(rawurl, null, method, clientname);
        }

    }
}
