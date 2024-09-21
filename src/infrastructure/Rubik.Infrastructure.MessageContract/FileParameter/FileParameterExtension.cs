using System.Text.Json;

namespace Rubik.Infrastructure.Contract.HttpRequest
{
    public static class FileParameterExtension
    {
        static readonly System.Net.Http.Headers.MediaTypeHeaderValue SteamHeader = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");
        static readonly System.Net.Http.Headers.MediaTypeHeaderValue JsonHeader = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        public static MultipartFormDataContent ToMultipartFormDataContent(this UploadFileRequest request)
        {
            return  CreateMultipartFormDataContent(request.Files!);
        }

        public static MultipartFormDataContent ToMultipartFormDataContent<T>(this UploadFileRequest<T> request)where T : class
        {
            var content = CreateMultipartFormDataContent(request.Files!);
            return content.AddJsonContent(request.Data);
        }

        public static MultipartFormDataContent ToMultipartFormDataContent(this UploadStreamRequest request)
        {
            var content = CreateMultipartFormDataContent(new UploadStreamRequest[] { request }); ;
            return content;
        }

        public static MultipartFormDataContent ToMultipartFormDataContent(this IEnumerable<UploadStreamRequest> request) 
        {
            var content = CreateMultipartFormDataContent(request!);
            return content;
        }

        public static MultipartFormDataContent ToMultipartFormDataContent<T>(this UploadStreamRequest<T> request) where T: class
        {
            var content = CreateMultipartFormDataContent(request.Files!);
            return content.AddJsonContent(request.Data);
        }

        static MultipartFormDataContent CreateMultipartFormDataContent(IEnumerable<FileInfo> files)
        {
            var multi = new MultipartFormDataContent();
            foreach (var file in files)
            {
                var fs = file.Open(FileMode.Open,FileAccess.Read,FileShare.Read);
                multi.Add(CreateStreamContent(fs), file.Name, file.Name);
            }
            return multi;
        }

        static MultipartFormDataContent CreateMultipartFormDataContent(IEnumerable<UploadStreamRequest> files)
        {
            var multi = new MultipartFormDataContent();
            foreach (var file in files)
            {
                multi.Add(CreateStreamContent(file.Stream!),file.FileName!,file.FileName!);
            }
            return multi;
        }

        static StreamContent CreateStreamContent(Stream stream)
        {
            var content = new StreamContent(stream);
            content.Headers.ContentType = SteamHeader;
            content.Headers.ContentLength= stream.Length;
            return content;
        }

        static StringContent CreateJsonContent<T>(T? data)
            where T : class
        {
            var content = new StringContent(JsonSerializer.Serialize(data));
            content.Headers.ContentType = JsonHeader;
            return content;
        }

        static MultipartFormDataContent AddJsonContent<T>(this MultipartFormDataContent content,T? data) where T : class
        {
            var jsonContent = CreateJsonContent(data);
            content.Add(jsonContent, "Data");
            return content;
        }
    }
}
