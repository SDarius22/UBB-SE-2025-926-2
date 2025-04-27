using Hospital.Exceptions;
using Hospital.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Hospital.Tests.Services
{
    [TestFixture]
    public class FileServiceTests
    {
        private FileService _fileService;
        private List<string> _tempFiles;

        [SetUp]
        public void SetUp()
        {
            _fileService = new FileService();
            _tempFiles = new List<string>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var file in _tempFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        [Test]
        public async Task CreateAndSaveZipFile_ValidFiles_ReturnsZipPath()
        {
            string tempFile = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFile, "This is a test file.");
            _tempFiles.Add(tempFile);

            string zipPath = await _fileService.CreateAndSaveZipFile(new List<string> { tempFile });

            Assert.IsTrue(File.Exists(zipPath));
            _tempFiles.Add(zipPath);

            using var zip = ZipFile.OpenRead(zipPath);
            Assert.That(zip.Entries.Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateAndSaveZipFile_EmptyList_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _fileService.CreateAndSaveZipFile(new List<string>()));
        }

        [Test]
        public void CreateAndSaveZipFile_FileNotFound_ThrowsDocumentNotFoundException()
        {
            string fakePath = @"C:\this\path\does\not\exist.txt";
            var ex = Assert.ThrowsAsync<DocumentNotFoundException>(() =>
                _fileService.CreateAndSaveZipFile(new List<string> { fakePath }));

            Assert.IsTrue(ex.Message.Contains("Document not found"));
        }
    }
}
