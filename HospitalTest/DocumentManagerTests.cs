using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Managers;
using Hospital.Models;
using Hospital.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace Hospital.Tests.Managers
{
    [TestFixture]
    public class DocumentManagerTests
    {
        private Mock<IDocumentDatabaseService> _mockDbService;
        private Mock<IFileService> _mockFileService;
        private DocumentManager _manager;

        [SetUp]
        public void SetUp()
        {
            _mockDbService = new Mock<IDocumentDatabaseService>();
            _mockFileService = new Mock<IFileService>();
            _manager = new DocumentManager(_mockDbService.Object, _mockFileService.Object);
        }

        [Test]
        public async Task LoadDocuments_FillsInternalList()
        {
            var docs = new List<DocumentModel>
            {
                new DocumentModel(1, 1, @"C:\Docs\doc1.pdf")
            };

            _mockDbService.Setup(s => s.GetDocumentsByMedicalRecordId(1))
                          .ReturnsAsync(docs);

            await _manager.LoadDocuments(1);

            Assert.That(_manager.GetDocuments().Count, Is.EqualTo(1));
        }

        [Test]
        public void HasDocuments_WhenMatchingRecord_ReturnsTrue()
        {
            var doc = new DocumentModel(1, 99, @"C:\test.pdf");
            _manager.GetDocuments().Add(doc);

            var result = _manager.HasDocuments(99);

            Assert.IsTrue(result);
        }

        [Test]
        public void HasDocuments_WhenNoMatch_ReturnsFalse()
        {
            var doc = new DocumentModel(1, 55, @"C:\test.pdf");
            _manager.GetDocuments().Add(doc);

            var result = _manager.HasDocuments(99);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddDocumentToMedicalRecord_Success_AddsToList()
        {
            var doc = new DocumentModel(2, 1, @"C:\doc2.pdf");
            _mockDbService.Setup(s => s.UploadDocumentToDataBase(doc)).ReturnsAsync(true);

            await _manager.AddDocumentToMedicalRecord(doc);

            Assert.That(_manager.GetDocuments().Count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddDocumentToMedicalRecord_Failure_DoesNotAdd()
        {
            var doc = new DocumentModel(3, 1, @"C:\doc3.pdf");
            _mockDbService.Setup(s => s.UploadDocumentToDataBase(doc)).ReturnsAsync(false);

            await _manager.AddDocumentToMedicalRecord(doc);

            Assert.That(_manager.GetDocuments().Count, Is.EqualTo(0));
        }

        [Test]
        public async Task AddDocumentToMedicalRecord_SuccessfulUpload_AddsDocument()
        {
            var doc = new DocumentModel(1, 1, @"C:\doc1.pdf");

            _mockDbService.Setup(s => s.UploadDocumentToDataBase(doc))
                                .ReturnsAsync(true);

            await _manager.AddDocumentToMedicalRecord(doc);

            var result = _manager.GetDocuments();
            Assert.That(result, Contains.Item(doc));
        }

        [Test]
        public async Task AddDocumentToMedicalRecord_UploadThrowsException_HandledGracefully()
        {
            var doc = new DocumentModel(10, 10, @"C:\doc10.pdf");

            _mockDbService.Setup(s => s.UploadDocumentToDataBase(doc))
                                .ThrowsAsync(new Exception("Upload failed"));

            await _manager.AddDocumentToMedicalRecord(doc);

            var result = _manager.GetDocuments();
            Assert.That(result, Is.Empty);
        }


        [Test]
        public void DownloadDocuments_NoDocuments_ThrowsException()
        {
            Assert.ThrowsAsync<DocumentNotFoundException>(() => _manager.DownloadDocuments(1));
        }

        [Test]
        public async Task DownloadDocuments_WithFiles_CreatesZip()
        {
            var doc = new DocumentModel(4, 1, @"C:\doc4.pdf");
            _manager.GetDocuments().Add(doc);
            _mockFileService.Setup(f => f.CreateAndSaveZipFile(It.IsAny<List<string>>()))
                            .ReturnsAsync(@"C:\output\files.zip");

            await _manager.DownloadDocuments(1);

            _mockFileService.Verify(f => f.CreateAndSaveZipFile(It.Is<List<string>>(l => l.Contains(@"C:\doc4.pdf"))), Times.Once);
        }
    }
}