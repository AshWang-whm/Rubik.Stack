using SharpSevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.Utils.ZipCompressor
{
    public class ZipExtension
    {
        public static async Task<MemoryStream?> ZipFilesAsync(IEnumerable<string> files, OutArchiveFormat format = OutArchiveFormat.SevenZip, CompressionLevel level = CompressionLevel.Ultra)
        {
            if(!files.Any())
            {
                return null;
            }
            //// 抓取文件并压缩
            var ms = new MemoryStream();

            var compressor = new SharpSevenZipCompressor
            {
                ArchiveFormat = format,
                PreserveDirectoryRoot = true,
                CompressionLevel = level
            };

            await compressor.CompressFilesAsync(ms, [.. files]);

            return ms;
        }

        public static async Task<MemoryStream?> ZipFilesAsync(IEnumerable<FileInfo> files, OutArchiveFormat format= OutArchiveFormat.SevenZip, CompressionLevel level= CompressionLevel.Ultra)
        {
            return await ZipFilesAsync(files.Select(a=>a.FullName), format, level);
        }

        /// <summary>
        /// 文件压缩到某个路径
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static async Task<bool> ZipFilesAsync(string dest, IEnumerable<FileInfo> files, OutArchiveFormat format = OutArchiveFormat.SevenZip, CompressionLevel level = CompressionLevel.Ultra)
        {
            try
            {
                if (File.Exists(dest))
                    File.Delete(dest);

                var sevenzip = new SharpSevenZipCompressor()
                {
                    ArchiveFormat = format,
                    DirectoryStructure = false,
                    CompressionLevel = level,
                };

                await sevenzip.CompressFilesAsync(dest, files.Select(a => a.FullName).ToArray());
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> ZipFoldersAsync(string dest, IEnumerable<string> folders)
        {
            var files = folders.SelectMany(a => new DirectoryInfo(a).GetFiles("*", SearchOption.AllDirectories))
                .GroupBy(a => a.Name)
                .Select(a => a.First())
                .ToList();

            return await ZipFilesAsync(dest, files);
        }
    }
}
