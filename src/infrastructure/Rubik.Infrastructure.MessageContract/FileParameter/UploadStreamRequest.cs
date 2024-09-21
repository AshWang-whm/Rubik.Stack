
using System.Collections.Generic;
using System.IO;

namespace Rubik.Infrastructure.Contract.HttpRequest
{
    public class UploadFileRequest
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public required IEnumerable<FileInfo> Files { get; set; }
    }

    public class UploadFileRequest<T>:UploadFileRequest where T : class
    {
        public required T Data { get; set; }
    }

    public class UploadStreamRequest
    {
        public required Stream Stream { get; set; }

        public required string FileName { get; set; }
    }

    public class UploadStreamRequest<T> where T: class
    {
        public required T Data { get; set; }

        public required IEnumerable<UploadStreamRequest> Files { get; set; }
    }
}
