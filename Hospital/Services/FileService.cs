using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Hospital.Exceptions;

namespace Hospital.Services
{
    public interface IFileService
    {
        Task<string> CreateAndSaveZipFile(List<string> filePaths);
    }

    public class FileService : IFileService
    {
        public async Task<string> CreateAndSaveZipFile(List<string> filePaths)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                throw new ArgumentException("No files provided for zip creation");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in filePaths)
                    {
                        if (File.Exists(filePath))
                        {
                            var fileName = Path.GetFileName(filePath);
                            var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                        else
                        {
                            throw new DocumentNotFoundException($"Document not found at path: {filePath}");
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                var zipFile = memoryStream.ToArray();

                string zipFileName = GenerateZipFileName();
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", zipFileName);
                await File.WriteAllBytesAsync(path, zipFile);

                return path;
            }
        }

        private string GenerateZipFileName()
        {
            string timestampFormat = "yyyyMMddHHmmss";
            return $"Documents_{DateTime.Now.ToString(timestampFormat)}.zip";
        }
    }
} 