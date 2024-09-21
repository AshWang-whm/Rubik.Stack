//using System.Text;
//using System.Text.Json;

//namespace Rubik.Infrastructure.RequestClient
//{
//    public partial class RequestClient
//    {

//        internal async Task<IOutputHttpResponse<TResult>> CallApiCamelOutput<TResult>(string url, HttpContent? content, string method = "POST", string? clientname = null)
//            where TResult : class
//        {
//            if (clientname == null)
//            {
//                return HttpResponseCamelOutput.NotOk(default(TResult), $"Client Option:[{clientname}] Not Found!");
//            }
//            using var client = httpClientFactory.CreateClient(clientname!);
//            if (client == null)
//            {
//                return HttpResponseCamelOutput.NotOk(default(TResult), $"Client Option:[{clientname}] Not Found!");
//            }

//            try
//            {
//                var hrm = await CallHttpResponseMessage(url, client, content, method);
//                if (hrm == null)
//                {
//                    return HttpResponseCamelOutput.NotOk(data: default(TResult), msg: "HttpResponseMessage Is Null!");
//                }
//                else if (!hrm.IsSuccessStatusCode)
//                {
//                    return HttpResponseCamelOutput.NotOk(data: default(TResult), msg: hrm.ReasonPhrase);
//                }
//                var json = await hrm.Content.ReadAsStringAsync();
//                var result = JsonSerializer.Deserialize<ResponseCamelOutput<TResult>>(json, JsonSerializerOption);
//                return result ?? HttpResponseCamelOutput.NotOk(data: default(TResult), msg: "Response Json Deserialize Error!");
//            }
//            catch (Exception ex)
//            {
//                return HttpResponseCamelOutput.NotOk(data: default(TResult), msg: $"Url:{url}, Err:{ex.Message}");
//            }
//            finally
//            {
//                content?.Dispose();
//            }
//        }

//        internal async Task<IResponseCamelOutput> CallApiCamelOutput(string url, HttpContent? content, string method = "POST", string? clientname = null)
//        {
//            if (clientname == null)
//            {
//                return HttpResponseCamelOutput.NotOk($"Client Option:[{clientname}] Not Found!");
//            }
//            var client = httpClientFactory.CreateClient(clientname);
//            if (client == null)
//            {
//                return HttpResponseCamelOutput.NotOk($"Client Option:[{clientname}] Not Found!");
//            }

//            try
//            {
//                var hrm = await CallHttpResponseMessage(url, client, content, method);
//                if (hrm == null)
//                {
//                    return HttpResponseCamelOutput.NotOk("HttpResponseMessage Is Null!");
//                }
//                var response = await hrm.Content.ReadAsStringAsync();
//                var result = JsonSerializer.Deserialize<ResponseCamelOutput>(response, JsonSerializerOption);
//                return result ?? HttpResponseCamelOutput.NotOk(msg: "Response Json Deserialize Error!");
//            }
//            catch (Exception ex)
//            {
//                return HttpResponseCamelOutput.NotOk(ex.Message);
//            }
//            finally
//            {
//                content?.Dispose();
//            }

//        }

//        internal async Task<IResponseCamelPageOutput<TResult>> CallPageApiCamelOutput<TResult>(string url, HttpContent? content, string method = "POST", string? clientname = null)
//        where TResult : class
//        {
//            if (clientname == null)
//            {
//                return HttpResponseCamelOutput.PageNotOk(default(IEnumerable<TResult>),total:0, $"Client Option:[{clientname}] Not Found!");
//            }
//            using var client = httpClientFactory.CreateClient(clientname);
//            if (client == null)
//            {
//                return HttpResponseCamelOutput.PageNotOk<TResult>(msg: $"Client Option:[{clientname}] Not Found!");
//            }

//            try
//            {
//                var hrm = await CallHttpResponseMessage(url, client, content, method);
//                if (hrm == null)
//                {
//                    return HttpResponseCamelOutput.PageNotOk<TResult>(msg: "HttpResponseMessage Is Null!");
//                }
//                else if (!hrm.IsSuccessStatusCode)
//                {
//                    return HttpResponseCamelOutput.PageNotOk<TResult>(msg: hrm.ReasonPhrase ?? hrm.StatusCode.ToString());
//                }
//                var json = await hrm.Content.ReadAsStringAsync();
//                var result = JsonSerializer.Deserialize<ResponseCamelPageOutput<TResult>>(json, JsonSerializerOption);
//                return result ?? HttpResponseCamelOutput.PageNotOk<TResult>(msg: "Response Json Deserialize Error!");
//            }
//            catch (Exception ex)
//            {
//                return HttpResponseCamelOutput.PageNotOk<TResult>(msg: ex.Message);
//            }
//            finally
//            {
//                content?.Dispose();
//            }
//        }

//        public async Task<IResponseCamelOutput<TResult>> CallApiCamelOutput<TResult>(string url, object? parameter = null, string? clientname = null)
//        where TResult : class
//        {
//            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json");
//            return await CallApiCamelOutput<TResult>(url, content, clientname: clientname);
//        }

//        public async Task<IResponseCamelOutput> CallApiCamelOutput(string url, object? parameter = null, string clientname = ClientName)
//        {
//            var content = new StringContent(JsonSerializer.Serialize(parameter), Encoding.UTF8, "application/json");
//            return await CallApiCamelOutput(url, content, clientname);
//        }

//        public async Task<IResponseCamelOutput<TResult>> CallApiCamelOutput<TResult>(string url, string clientname = ClientName) where TResult : class
//        {
//            return await CallApiCamelOutput<TResult>(url, null, clientname);
//        }

//        public async Task<IResponseCamelPageOutput<TResult>> CallPageApiCamelOutput<TResult>(string url, object? parameter, string clientname = ClientName) where TResult : class
//        {
//            return await CallPageApiCamelOutput<TResult>(url, parameter, clientname);
//        }

//        public async Task<IResponseCamelOutput<TResult>> QueryApiCamelOutput<TResult>(string url, object? parameter = null, string clientname = ClientName)
//            where TResult : class
//        {
//            var query = parameter.ToQueryUrl(url);
//            return await CallApiCamelOutput<TResult>(query, null, clientname);
//        }

//        public async Task<IResponseCamelOutput> QueryApiCamelOutput(string url, object? parameter = null, string clientname = ClientName)
//        {
//            var query = parameter.ToQueryUrl(url);
//            return await CallApiCamelOutput(query, null, clientname);
//        }

//        public async Task<IResponseCamelOutput<TResult>> CallRestApiCamelOutput<TResult>(string url, object? parameter=null, string clientname = ClientName)
//        where TResult : class
//        {
//            var query = parameter.ToQueryUrl(url);
//            return await CallApiCamelOutput<TResult>(query, null, clientname);
//        }

//        public async Task<IResponseCamelOutput> CallRestApiCamelOutput(string url, object? parameter=null, string clientname = ClientName)
//        {
//            var query = parameter.ToQueryUrl(url);
//            return await CallApiCamelOutput(query, null, clientname);
//        }

//        public async Task<IResponseCamelOutput> UploadFileCamelOutput(string url, UploadFile file, string clientname = ClientName)
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput(url, content, clientname: clientname);
//        }

//        /// <summary>
//        /// Api端使用HttpRequest.Form.Files接收文件
//        /// </summary>
//        /// <typeparam name="TResult"></typeparam>
//        /// <param name="url"></param>
//        /// <param name="file"></param>
//        /// <param name="clientname"></param>
//        /// <returns></returns>
//        public async Task<IResponseCamelOutput> UploadFileCamelOutput<TResult>(string url, UploadFile file, string clientname = ClientName) where TResult : class
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput<TResult>(url, content, clientname: clientname);
//        }

//        /// <summary>
//        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
//        /// </summary>
//        /// <typeparam name="TIn"></typeparam>
//        /// <param name="url"></param>
//        /// <param name="file"></param>
//        /// <param name="clientname"></param>
//        /// <returns></returns>
//        public async Task<IResponseCamelOutput> UploadFileCamelOutput<TIn>(string url, UploadFile<TIn> file, string clientname = ClientName) where TIn : class
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput(url, content, clientname: clientname);
//        }

//        /// <summary>
//        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
//        /// </summary>
//        /// <typeparam name="TIn"></typeparam>
//        /// <typeparam name="TResult"></typeparam>
//        /// <param name="url"></param>
//        /// <param name="file"></param>
//        /// <param name="clientname"></param>
//        /// <returns></returns>
//        public async Task<IResponseCamelOutput<TResult>> UploadFileCamelOutput<TIn, TResult>(string url, UploadFile<TIn> file, string clientname = ClientName)
//            where TIn : class
//            where TResult : class
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput<TResult>(url, content, clientname: clientname);
//        }

//        /// <summary>
//        /// Api端使用HttpRequest.Form.Files接收文件
//        /// </summary>
//        /// <param name="url"></param>
//        /// <param name="file"></param>
//        /// <param name="clientname"></param>
//        /// <returns></returns>
//        public async Task<IResponseCamelOutput> UploadFileCamelOutput(string url, UploadStream file, string clientname = ClientName)
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput(url, content, clientname: clientname);
//        }

//        /// <summary>
//        /// Api端使用HttpRequest.Form.Files接收文件
//        /// </summary>
//        /// <typeparam name="TResult"></typeparam>
//        /// <param name="url"></param>
//        /// <param name="file"></param>
//        /// <param name="clientname"></param>
//        /// <returns></returns>
//        public async Task<IResponseCamelOutput> UploadFileCamelOutput<TResult>(string url, UploadStream file, string clientname = ClientName) where TResult : class
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput<TResult>(url, content, clientname: clientname);
//        }

//        /// <summary>
//        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
//        /// </summary>
//        /// <typeparam name="TIn"></typeparam>
//        /// <param name="url"></param>
//        /// <param name="file"></param>
//        /// <param name="clientname"></param>
//        /// <returns></returns>
//        public async Task<IResponseCamelOutput> UploadFileCamelOutput<TIn>(string url, UploadStream<TIn> file, string clientname = ClientName) where TIn : class
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput(url, content, clientname: clientname);
//        }

//        /// <summary>
//        /// Api端使用HttpRequest.Form.Files接收文件,Form["Data"]接收Json
//        /// </summary>
//        /// <typeparam name="TIn"></typeparam>
//        /// <typeparam name="TResult"></typeparam>
//        /// <param name="url"></param>
//        /// <param name="file"></param>
//        /// <param name="clientname"></param>
//        /// <returns></returns>
//        public async Task<IResponseCamelOutput<TResult>> UploadFileCamelOutput<TIn, TResult>(string url, UploadStream<TIn> file, string clientname = ClientName)
//            where TIn : class
//            where TResult : class
//        {
//            var content = file.ToMultipartFormDataContent();
//            return await CallApiCamelOutput<TResult>(url, content, clientname: clientname);
//        }
//    }
//}
