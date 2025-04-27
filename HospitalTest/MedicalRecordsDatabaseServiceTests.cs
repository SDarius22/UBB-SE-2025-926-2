using Hospital.Configs;
using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Models;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Hospital.Tests.DatabaseServices
{
    [TestFixture]
    public class MedicalRecordsDatabaseServiceTests
    {
        private IMedicalRecordsDatabaseService _service;
        private ApplicationConfiguration _config;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _config = ApplicationConfiguration.GetInstance();
        }

        [SetUp]
        public void SetUp()
        {
            _service = new MedicalRecordsDatabaseService();
        }

        [Test]
        public async Task AddMedicalRecord_ValidRecord_ReturnsId()
        {
            // Arrange
            var record = new MedicalRecordModel(
                medicalRecordId: 0,
                patientId: 1,
                doctorId: 2,
                procedureId: 3,
                conclusion: "Test conclusion",
                dateAndTime: DateTime.Now
            );

            // Act
            var result = await _service.AddMedicalRecord(record);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void AddMedicalRecord_DatabaseError_ThrowsException()
        {
            // Arrange
            var record = new MedicalRecordModel(
                medicalRecordId: 0,
                patientId: -1,
                doctorId: -1,
                procedureId: -1,
                conclusion: "Test conclusion",
                dateAndTime: DateTime.Now
            );

            // Act & Assert
            Assert.ThrowsAsync<DatabaseOperationException>(
                async () => await _service.AddMedicalRecord(record)
            );
        }

        [Test]
        public async Task GetMedicalRecordsForPatient_ValidPatientId_ReturnsRecords()
        {
            // Arrange
            int patientId = 1;

            // Act
            var result = await _service.GetMedicalRecordsForPatient(patientId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(MedicalRecordJointModel));
            });
        }

        [Test]
        public void GetMedicalRecordsForPatient_InvalidId_ThrowsException()
        {
            // Arrange
            int patientId = -1;

            // Act & Assert
            Assert.ThrowsAsync<MedicalRecordNotFoundException>(
                async () => await _service.GetMedicalRecordsForPatient(patientId)
            );
        }

        [Test]
        public async Task GetMedicalRecordById_ValidId_ReturnsRecord()
        {
            // Arrange
            // First create a medical record
            var record = new MedicalRecordModel(
                medicalRecordId: 0,
                patientId: 1,
                doctorId: 2,
                procedureId: 3,
                conclusion: "Test conclusion",
                dateAndTime: DateTime.Now
            );
            var recordId = await _service.AddMedicalRecord(record);

            // Act
            var result = await _service.GetMedicalRecordById(recordId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<MedicalRecordJointModel>());
            });
        }

        [Test]
        public void GetMedicalRecordById_InvalidId_ThrowsException()
        {
            // Arrange
            int recordId = -1;

            // Act & Assert
            Assert.ThrowsAsync<MedicalRecordNotFoundException>(
                async () => await _service.GetMedicalRecordById(recordId)
            );
        }

        [Test]
        public async Task GetMedicalRecordsForDoctor_ValidDoctorId_ReturnsRecords()
        {
            // Arrange
            int doctorId = 1;

            // Act
            var result = await _service.GetMedicalRecordsForDoctor(doctorId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(MedicalRecordJointModel));
            });
        }

        [Test]
        public void GetMedicalRecordsForDoctor_InvalidId_ThrowsException()
        {
            // Arrange
            int doctorId = -1;

            // Act & Assert
            Assert.ThrowsAsync<MedicalRecordNotFoundException>(
                async () => await _service.GetMedicalRecordsForDoctor(doctorId)
            );
        }
    }
}
