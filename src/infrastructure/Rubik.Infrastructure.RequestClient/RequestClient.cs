using Microsoft.Extensions.DependencyInjection;
using Rubik.Infrastructure.Contract.Extensions;
using Rubik.Infrastructure.Contract.HttpRequest;
using Rubik.Infrastructure.Contract.HttpResponse;
using System.Text;
using System.Text.Json;

namespace Rubik.Infrastructure.RequestClient
{
    public partial class RequestClient(IHttpClientFactory httpClientFactory,IServiceProvider serviceProvider)
    {
        public const string HandlerPerfix = "__REQUESTCONFIG__";
        public const string ClientName = "__REQUESTCLIENT__";
        private readonly JsonSerializerOptions JsonSerializerOption = new() { PropertyNameCaseInsensitive = true };
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

        internal async Task<HttpResponseMessage?> CallHttpResponseMessage(string url, HttpContent? content, HttpMethodType method =  HttpMethodType.POST, string? clientname = null)
        {
            using var client = httpClientFactory.CreateClient(clientname!) ?? throw new Exception( $"Client Option:[{clientname}] Not Found!");

            var clientHandler = serviceProvider.GetKeyedService<IRequestClientHandler>($"{HandlerPerfix}_{clientname}");
            clientHandler?.HttpClientSetup(client);

            HttpResponseMessage? hrm = null;
            switch (method)
            {
                case HttpMethodType.GET:
                    hrm = await client.GetAsync(url);
                    break;
                case HttpMethodType.POST:
                    hrm = await client.PostAsync(url, content);
                    break;
                case HttpMethodType.PUT:
                    hrm = await client.PutAsync(url, content);
                    break;
                case HttpMethodType.DELETE:
                    hrm = await client.DeleteAsync(url);
                    break;
            }
            return hrm;
        }

        internal async Task<TResult> InternalCallApi<TResult>(string url, HttpContent? content, HttpMethodType method = HttpMethodType.POST, string? clientname = null)
            where TResult : class
        {
            try
            {
                var hrm = await CallHttpResponseMessage(url, content, method,clientname);
                if (hrm == null)
                {
                    throw new Exception("HttpResponseMessage Is Null!");
                }
                else if (!hrm.IsSuccessStatusCode)
                {
                    throw new Exception(hrm.ReasonPhrase);
                }
                var json = await hrm.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TResult>(json, JsonSerializerOption) ?? throw new Exception("Response Json Deserialize Error!");
                return result;
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

        public async Task<IOutputHttpResponse<TResult>> CallApi<TResult>(string url, object? parameter, HttpMethodType method = HttpMethodType.POST,string mediaType= "application/json", string clientname = ClientName)
            where TResult : class, IOutputHttpResponse<TResult>
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, mediaType);
            return await InternalCallApi<OutputHttpResponse<TResult>>(url, content, method, clientname);
        }

        public async Task<IOutputHttpPageResponse<TResult>> CallPageApi<TResult>(string url, object? parameter, HttpMethodType method = HttpMethodType.POST, string mediaType = "application/json", string clientname = ClientName) 
            where TResult : class, IOutputHttpPageResponse<TResult>
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, mediaType);
            return await InternalCallApi<OutputHttpPageResponse<TResult>>(url, content, method, clientname);
        }

        public async Task<IOutputHttpResponse<TResult>> GetApi<TResult>(string url, object? parameter, string clientname = ClientName)
            where TResult : class
        {
            var query = parameter.ToQueryString(url);
            return await InternalCallApi<OutputHttpResponse<TResult>>(query, null, HttpMethodType.GET, clientname);
        }

        public async Task<IOutputHttpResponse> GetApi(string url, object? parameter, string clientname = ClientName)
        {
            var query = parameter.ToQueryString(url);
            return await InternalCallApi<OutputHttpResponse>(query, null,HttpMethodType.GET, clientname);
        }

        public async Task<IOutputHttpResponse<TResult>> GetRestApi<TResult>(string url, object? parameter, string clientname = ClientName)
            where TResult : class
        {
            var query = parameter.ToRestQueryString(url);
            return await InternalCallApi<OutputHttpResponse<TResult>>(query, null, HttpMethodType.GET, clientname);
        }

        public async Task<IOutputHttpResponse> GetRestApi(string url, object? parameter, string clientname = ClientName)
        {
            var query = parameter.ToRestQueryString(url);
            return await InternalCallApi<OutputHttpResponse>(query, null, HttpMethodType.GET, clientname);
        }

        public async Task<IOutputHttpResponse> UploadFile(string url, UploadFileRequest parameter, string clientname = ClientName)
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse<TResult>> UploadFile<TResult>(string url, UploadFileRequest parameter, string clientname = ClientName) 
            where TResult : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse<TResult>>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse> UploadFile<TIn>(string url, UploadFileRequest<TIn> parameter, string clientname = ClientName) where TIn : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse<TResult>> UploadFile<TIn, TResult>(string url, UploadFileRequest<TIn> parameter, string clientname = ClientName)
            where TIn : class
            where TResult : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse<TResult>>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse> UploadStream(string url, UploadStreamRequest parameter, string clientname = ClientName)
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse<TResult>> UploadStream<TResult>(string url, UploadStreamRequest parameter, string clientname = ClientName) where TResult : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse<TResult>>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse> UploadStream<TIn>(string url, UploadStreamRequest<TIn> parameter, string clientname = ClientName) where TIn : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse<TResult>> UploadStream<TIn, TResult>(string url, UploadStreamRequest<TIn> parameter, string clientname = ClientName)
            where TIn : class
            where TResult : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await InternalCallApi<OutputHttpResponse<TResult>>(url, content, clientname: clientname);
        }

        /// <summary>
        /// 下载文件到指定目录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="destFolder"></param>
        /// <param name="clientname"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DownloadFile(string url, string destFolder, string clientname = ClientName, HttpMethodType method = HttpMethodType.GET)
        {
            var hrm = await DownloadFileContent(url, clientname, method) ?? throw new Exception("文件下载失败,接口无返回数据");
            if (!hrm.IsSuccessStatusCode)
                throw new Exception($"文件下载失败,接口返回:[{hrm.StatusCode}]");
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            var filename = (hrm.Content.Headers.ContentDisposition?.FileNameStar ?? hrm.Content.Headers.ContentDisposition?.FileName) ?? throw new Exception($"文件下载失败,未找到文件");

            var dest = Path.Combine(destFolder, filename);

            using var fs = new FileStream(dest, FileMode.Create);
            using var sr = await hrm.Content.ReadAsStreamAsync();
            await sr.CopyToAsync(fs);
        }

        public async Task DownloadFile<TIn>(string url, TIn parameter, string destFolder, string clientname = ClientName)
            where TIn : class
        {
            var hrm = await DownloadFileContent(url, parameter, clientname) ?? throw new Exception("文件下载失败,接口无返回数据");
            if (!hrm.IsSuccessStatusCode)
                throw new Exception($"文件下载失败,接口返回:[{hrm.StatusCode}]");

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            var filename = (hrm.Content.Headers.ContentDisposition?.FileNameStar ?? hrm.Content.Headers.ContentDisposition?.FileName) ?? throw new Exception($"文件下载失败,未找到文件");
            var dest = Path.Combine(destFolder, filename);

            using var fs = new FileStream(dest, FileMode.Create);
            using var sr = await hrm.Content.ReadAsStreamAsync();
            await sr.CopyToAsync(fs);
        }

        /// <summary>
        /// 下载文件流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="clientname"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Stream?> DownloadStream(string url,string clientname=ClientName, HttpMethodType method = HttpMethodType.GET)
        {
            var hrm = await DownloadFileContent(url, clientname,method);
            var stream = await hrm!.Content!.ReadAsStreamAsync();
            return stream ?? throw new Exception("HttpResponseMessage is empty!");
        }

        public async Task<HttpResponseMessage?> DownloadFileContent(string url, string clientname = ClientName, HttpMethodType method = HttpMethodType.GET)
        {
            var hrm = await CallHttpResponseMessage(url, null, method,clientname);
            return hrm;
        }

        public async Task<HttpResponseMessage?> DownloadFileContent<TIn>(string url, TIn parameter, string clientname = ClientName)
            where TIn : class
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json");
            var hrm = await CallHttpResponseMessage(url, content, HttpMethodType.GET,clientname);
            return hrm;
        }

    }
}
