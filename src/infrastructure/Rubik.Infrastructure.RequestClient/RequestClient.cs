using Rubik.Infrastructure.Contract.Extensions;
using Rubik.Infrastructure.Contract.HttpRequest;
using Rubik.Infrastructure.Contract.HttpResponse;
using System.Text;
using System.Text.Json;

namespace Rubik.Infrastructure.RequestClient
{
    public partial class RequestClient(IHttpClientFactory httpClientFactory)
    {
        public const string ClientName = "__REQUESTCLIENT__";
        private readonly JsonSerializerOptions JsonSerializerOption = new() { PropertyNameCaseInsensitive = true };
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

        internal static async Task<HttpResponseMessage?> CallHttpResponseMessage(string url, HttpClient client, HttpContent? content, string method = "POST")
        {
            HttpResponseMessage? hrm = null;
            switch (method)
            {
                case "GET":
                    hrm = await client.GetAsync(url);
                    break;
                case "POST":
                    hrm = await client.PostAsync(url, content);
                    break;
                case "PUT":
                    hrm = await client.PutAsync(url, content);
                    break;
                case "DELETE":
                    hrm = await client.DeleteAsync(url);
                    break;
            }
            return hrm;
        }

        internal async Task<IOutputHttpResponse<TResult>> CallApi<TResult>(string url, HttpContent? content,string method="POST", string? clientname = null)
        where TResult : class
        {

            using var client = httpClientFactory.CreateClient(clientname!);
            if (client == null)
            {
                return HttpOutput.NotOk(default(TResult), $"Client Option:[{clientname}] Not Found!");
            }

            try
            {
                var hrm = await CallHttpResponseMessage(url, client, content, method);
                if (hrm == null)
                {
                    return HttpOutput.NotOk(data: default(TResult), msg: "HttpResponseMessage Is Null!");
                }
                else if (!hrm.IsSuccessStatusCode)
                {
                    return HttpOutput.NotOk(data: default(TResult), msg: hrm.ReasonPhrase);
                }
                var json = await hrm.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OutputHttpResponse<TResult>>(json, JsonSerializerOption);
                return result ?? HttpOutput.NotOk(data: default(TResult), msg: "Response Json Deserialize Error!");
            }
            catch (Exception ex)
            {
                return HttpOutput.NotOk(data: default(TResult), msg: $"Url:{url}, Err:{ex.Message}");
            }
            finally
            {
                content?.Dispose();
            }
        }

        internal async Task<IOutputHttpResponse> CallApi(string url, HttpContent? content, string method = "POST", string? clientname = null)
        {
            if (clientname == null)
            {
                return HttpOutput.NotOk($"Client Option:[{clientname}] Not Found!");
            }
            var client = httpClientFactory.CreateClient(clientname);
            if (client == null)
            {
                return HttpOutput.NotOk($"Client Option:[{clientname}] Not Found!");
            }

            try
            {
                var hrm = await CallHttpResponseMessage(url, client, content, method);
                if (hrm == null)
                {
                    return HttpOutput.NotOk("HttpResponseMessage Is Null!");
                }
                var response = await hrm.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OutputHttpResponse>(response, JsonSerializerOption);
                return result ?? HttpOutput.NotOk(msg: "Response Json Deserialize Error!");
            }
            catch (Exception ex)
            {
                return HttpOutput.NotOk(ex.Message);
            }
            finally
            {
                content?.Dispose();
            }

        }

        internal async Task<IOutputHttpPageResponse<TResult>> CallPageApi<TResult>(string url, HttpContent? content, string method = "POST", string? clientname = null)
        where TResult : class
        {
            if (clientname == null)
            {
                return HttpOutput.PageNotOk<TResult>(msg:$"Client Option:[{clientname}] Not Found!");
            }
            using var client = httpClientFactory.CreateClient(clientname);
            if (client == null)
            {
                return HttpOutput.PageNotOk<TResult>(msg: $"Client Option:[{clientname}] Not Found!");
            }

            try
            {
                var hrm = await CallHttpResponseMessage(url, client, content, method);
                if (hrm == null)
                {
                    return HttpOutput.PageNotOk<TResult>(msg: "HttpResponseMessage Is Null!");
                }
                else if (!hrm.IsSuccessStatusCode)
                {
                    return HttpOutput.PageNotOk<TResult>(msg: hrm.ReasonPhrase??hrm.StatusCode.ToString());
                }
                var json = await hrm.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OutputHttpPageResponse<TResult>>(json, JsonSerializerOption);
                return result ?? HttpOutput.PageNotOk<TResult>(msg: "Response Json Deserialize Error!");
            }
            catch (Exception ex)
            {
                return HttpOutput.PageNotOk<TResult>(msg: ex.Message);
            }
            finally
            {
                content?.Dispose();
            }
        }

        public async Task<IOutputHttpResponse<TResult>> CallApi<TResult>(string url, object? parameter = null, string clientname = ClientName)
        where TResult : class
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json");
            return await CallApi<TResult>(url, content, clientname);
        }

        public async Task<IOutputHttpResponse> CallApi(string url, object? parameter = null, string clientname = ClientName)
        {
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json");
            return await CallApi(url, content, clientname);
        }

        public async Task<IOutputHttpResponse<TResult>> CallApi<TResult>(string url, string clientname = ClientName) where TResult : class
        {
            return await CallApi<TResult>(url, null, clientname);
        }

        public async Task<IOutputHttpPageResponse<TResult>> CallPageApi<TResult>(string url, object? parameter, string clientname = ClientName) where TResult : class
        {
            return await CallPageApi<TResult>(url, parameter, clientname);
        }

        public async Task<IOutputHttpResponse<TResult>> QueryApi<TResult>(string url, object parameter, string clientname = ClientName)
            where TResult : class
        {
            var query = parameter.ToQueryString(url);
            return await CallApi<TResult>(query, null, clientname);
        }

        public async Task<IOutputHttpResponse> QueryApi(string url, object parameter, string clientname = ClientName)
        {
            var query = parameter.ToQueryString(url);
            return await CallApi(query, null, clientname);
        }

        public async Task<IOutputHttpResponse<TResult>> CallRestApi<TResult>(string url, object parameter, string clientname = ClientName)
        where TResult : class
        {
            var query = parameter.ToRestQueryString(url);
            return await CallApi<TResult>(query, null, clientname);
        }

        public async Task<IOutputHttpResponse> CallRestApi(string url, object parameter, string clientname = ClientName)
        {
            var query = parameter.ToRestQueryString(url);
            return await CallApi(query, null, clientname);
        }

        public async Task<IOutputHttpResponse> UploadFile(string url, UploadFileRequest parameter, string clientname = ClientName)
        {
            var content = parameter.ToMultipartFormDataContent();
            return await CallApi(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse> UploadFile<TResult>(string url, UploadFileRequest parameter, string clientname = ClientName) where TResult : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await CallApi<TResult>(url, content, clientname: clientname);
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
            return await CallApi(url, content, clientname: clientname);
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
            return await CallApi<TResult>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse> UploadFile(string url, UploadStreamRequest parameter, string clientname = ClientName)
        {
            var content = parameter.ToMultipartFormDataContent();
            return await CallApi(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse> UploadFile<TResult>(string url, UploadStreamRequest parameter, string clientname = ClientName) where TResult : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await CallApi<TResult>(url, content, clientname: clientname);
        }

        /// <summary>
        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <param name="clientname"></param>
        /// <returns></returns>
        public async Task<IOutputHttpResponse> UploadFile<TIn>(string url, UploadStreamRequest<TIn> parameter, string clientname = ClientName) where TIn : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await CallApi(url, content, clientname: clientname);
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
        public async Task<IOutputHttpResponse<TResult>> UploadFile<TIn, TResult>(string url, UploadStreamRequest<TIn> parameter, string clientname = ClientName)
            where TIn : class
            where TResult : class
        {
            var content = parameter.ToMultipartFormDataContent();
            return await CallApi<TResult>(url, content, clientname: clientname);
        }

        public async Task DownloadFile(string url, string destFolder, string clientname = ClientName, string method = "GET")
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

        public async Task<Stream?> DownloadStream(string url,string clientname=ClientName,string method="GET")
        {
            var hrm = await DownloadFileContent(url, clientname,method);
            var stream = await hrm!.Content!.ReadAsStreamAsync();
            return stream ?? throw new Exception("HttpResponseMessage is empty!");
        }

        public async Task<HttpResponseMessage?> DownloadFileContent(string url, string clientname = ClientName, string method = "GET")
        {
            using var client = httpClientFactory.CreateClient(clientname) ?? throw new Exception($"Client Option:[{clientname}] Not Found!");
            var hrm = await CallHttpResponseMessage(url, client, null, method);
            return hrm;
        }

        public async Task<HttpResponseMessage?> DownloadFileContent<TIn>(string url, TIn parameter, string clientname = ClientName)
            where TIn : class
        {
            using var client = httpClientFactory.CreateClient(clientname) ?? throw new Exception($"Client Option:[{clientname}] Not Found!");
            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json");
            var hrm = await CallHttpResponseMessage(url, client, content, "POST");
            return hrm;
        }

    }
}
