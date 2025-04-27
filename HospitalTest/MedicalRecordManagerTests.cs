using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Managers;
using Hospital.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Hospital.Tests.Managers
{
    [TestFixture]
    public class MedicalRecordManagerTests
    {
        private Mock<IMedicalRecordsDatabaseService> _mockDbService;
        private MedicalRecordManager _manager;

        [SetUp]
        public void SetUp()
        {
            _mockDbService = new Mock<IMedicalRecordsDatabaseService>();
            _manager = new MedicalRecordManager(_mockDbService.Object);
        }

        [Test]
        public async Task LoadMedicalRecordsForPatient_LoadsList()
        {
            var records = new List<MedicalRecordJointModel>
            {
                new MedicalRecordJointModel(1, 1, "John", 1, "Dr. A", 1, "Cardio", 1, "MRI", DateTime.Now, "Fine")
            };

            _mockDbService.Setup(s => s.GetMedicalRecordsForPatient(1)).ReturnsAsync(records);

            await _manager.LoadMedicalRecordsForPatient(1);

            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(1));
        }

        public MedicalRecordManager Get_manager()
        {
            return _manager;
        }

        [Test]
        public async Task GetMedicalRecordById_RecordExists_ReturnsRecord()
        {
            var record = new MedicalRecordJointModel(1, 1, "John", 1, "Dr. A", 1, "Cardio", 1, "MRI", DateTime.Now, "Fine");
            _mockDbService.Setup(s => s.GetMedicalRecordById(1)).ReturnsAsync(record);

            var result = await _manager.GetMedicalRecordById(1);

            Assert.That(result.PatientName, Is.EqualTo("John"));
        }

        [Test]
        public async Task CreateMedicalRecord_ValidData_AddsToList()
        {
            var appointment = new AppointmentJointModel(1, false, DateTime.Now, 1, "Cardio", 2, "Dr. A", 3, "John", 4, "MRI", TimeSpan.FromMinutes(30));
            var expectedId = 99;

            _mockDbService.Setup(s => s.AddMedicalRecord(It.IsAny<MedicalRecordModel>())).ReturnsAsync(expectedId);
            _mockDbService.Setup(s => s.GetMedicalRecordById(expectedId)).ReturnsAsync(
                new MedicalRecordJointModel(expectedId, 3, "John", 2, "Dr. A", 1, "Cardio", 4, "MRI", DateTime.Now, "Test OK"));

            var result = await _manager.CreateMedicalRecord(appointment, "Test OK");

            Assert.That(result, Is.EqualTo(expectedId));
            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CreateMedicalRecord_ValidAppointment_AddsRecordAndReturnsId()
        {
            var appointment = new AppointmentJointModel(
                1, false, DateTime.Now, 1, "Cardiology", 2, "Dr. Smith", 3, "John Doe", 4, "ECG", TimeSpan.FromMinutes(30));

            var insertedId = 100;
            var jointModel = new MedicalRecordJointModel(
                insertedId, appointment.PatientId, appointment.PatientName, appointment.DoctorId,
                appointment.DoctorName, appointment.DepartmentId, appointment.DepartmentName,
                appointment.ProcedureId, appointment.ProcedureName, DateTime.Now, "All good");

            _mockDbService.Setup(s => s.AddMedicalRecord(It.IsAny<MedicalRecordModel>()))
                                .ReturnsAsync(insertedId);

            _mockDbService.Setup(s => s.GetMedicalRecordById(insertedId))
                                .ReturnsAsync(jointModel);

            var resultId = await _manager.CreateMedicalRecord(appointment, "All good");

            Assert.That(resultId, Is.EqualTo(insertedId));
            Assert.That(_manager.MedicalRecords, Has.One.Items);
        }

        [Test]
        public async Task CreateMedicalRecord_DatabaseThrowsException_ReturnsMinusOne()
        {
            var appointment = new AppointmentJointModel(
                1, false, DateTime.Now, 1, "Cardiology", 2, "Dr. Smith", 3, "John Doe", 4, "ECG", TimeSpan.FromMinutes(30));

            _mockDbService.Setup(s => s.AddMedicalRecord(It.IsAny<MedicalRecordModel>()))
                                .ThrowsAsync(new Exception("DB failure"));

            var result = await _manager.CreateMedicalRecord(appointment, "Fail test");

            Assert.That(result, Is.EqualTo(-1));
            Assert.That(_manager.MedicalRecords, Is.Empty);
        }


        [Test]
        public void ValidateConclusion_Valid_ReturnsTrue()
        {
            Assert.IsTrue(_manager.ValidateConclusion("This is fine."));
        }

        [Test]
        public void ValidateConclusion_Empty_ReturnsFalse()
        {
            Assert.IsFalse(_manager.ValidateConclusion(""));
        }

        [Test]
        public void ValidateConclusion_TooLong_ReturnsFalse()
        {
            var longText = new string('a', 256);
            Assert.IsFalse(_manager.ValidateConclusion(longText));
        }

        [Test]
        public void CreateMedicalRecordWithAppointment_InvalidConclusion_ThrowsValidationException()
        {
            var appointment = new AppointmentJointModel(1, false, DateTime.Now, 1, "Cardio", 2, "Dr. A", 3, "John", 4, "MRI", TimeSpan.FromMinutes(30));

            Assert.ThrowsAsync<ValidationException>(() => _manager.CreateMedicalRecordWithAppointment(appointment, ""));
        }

        [Test]
        public async Task CreateMedicalRecordWithAppointment_ValidConclusion_UpdatesAndCreates()
        {
            var appointment = new AppointmentJointModel(1, false, DateTime.Now.AddDays(-1), 1, "Cardio", 2, "Dr. A", 3, "John", 4, "MRI", TimeSpan.FromMinutes(30));

            _mockDbService.Setup(s => s.AddMedicalRecord(It.IsAny<MedicalRecordModel>())).ReturnsAsync(99);
            _mockDbService.Setup(s => s.GetMedicalRecordById(99)).ReturnsAsync(
                new MedicalRecordJointModel(99, 3, "John", 2, "Dr. A", 1, "Cardio", 4, "MRI", DateTime.Now, "Healthy"));

            var result = await _manager.CreateMedicalRecordWithAppointment(appointment, "Healthy");

            Assert.That(result, Is.EqualTo(99));
            Assert.IsTrue(appointment.Finished);
        }

        [Test]
        public async Task LoadMedicalRecordsForDoctor_ValidDoctorId_LoadsRecords()
        {
            int doctorId = 42;
            var mockRecords = new List<MedicalRecordJointModel>
    {
        new MedicalRecordJointModel(1, 1, "John", doctorId, "Dr. Smith", 1, "Cardio", 1, "ECG", DateTime.Now, "All good")
    };

            _mockDbService.Setup(s => s.GetMedicalRecordsForDoctor(doctorId))
                          .ReturnsAsync(mockRecords);

            await _manager.LoadMedicalRecordsForDoctor(doctorId);

            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(1));
            Assert.That(_manager.MedicalRecords[0].DoctorId, Is.EqualTo(doctorId));
        }

        [Test]
        public async Task LoadMedicalRecordsForDoctor_ThrowsException_DoesNotCrash()
        {
            int doctorId = 99;
            _mockDbService.Setup(s => s.GetMedicalRecordsForDoctor(doctorId))
                          .ThrowsAsync(new Exception("Database error"));

            await _manager.LoadMedicalRecordsForDoctor(doctorId);

            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(0)); // Nothing added
        }

        [Test]
        public async Task LoadMedicalRecordsForDoctor_AddsRecords()
        {
            var records = new List<MedicalRecordJointModel>
            {
                new MedicalRecordJointModel(1, 1, "John", 2, "Dr. A", 1, "Cardio", 4, "MRI", DateTime.Now, "Good")
            };

            _mockDbService.Setup(s => s.GetMedicalRecordsForDoctor(2)).ReturnsAsync(records);

            await _manager.LoadMedicalRecordsForDoctor(2);

            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMedicalRecords_ReturnsList()
        {
            var result = await _manager.GetMedicalRecords();
            Assert.IsInstanceOf<List<MedicalRecordJointModel>>(result);
        }

        [Test]
        public async Task GetMedicalRecordById_ValidId_ReturnsRecord()
        {
            var recordId = 10;
            var expectedRecord = new MedicalRecordJointModel(
                recordId, 1, "Patient", 2, "Doctor", 3, "Cardiology",
                4, "Procedure", DateTime.Now, "Conclusion");

            _mockDbService.Setup(s => s.GetMedicalRecordById(recordId))
                          .ReturnsAsync(expectedRecord);

            var result = await _manager.GetMedicalRecordById(recordId);

            Assert.That(result, Is.EqualTo(expectedRecord));
        }

        [Test]
        public void GetMedicalRecordById_RecordNotFound_ThrowsMedicalRecordNotFoundException()
        {
            var recordId = 99;
            _mockDbService.Setup(s => s.GetMedicalRecordById(recordId))
                          .ThrowsAsync(new MedicalRecordNotFoundException("not found"));

            Assert.ThrowsAsync<MedicalRecordNotFoundException>(async () => await _manager.GetMedicalRecordById(recordId));
        }

        [Test]
        public async Task GetMedicalRecordById_OtherException_ReturnsNull()
        {
            var recordId = 5;

            _mockDbService.Setup(s => s.GetMedicalRecordById(recordId))
                          .ThrowsAsync(new Exception("some error"));

            var result = await _manager.GetMedicalRecordById(recordId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task LoadMedicalRecordsForPatient_ValidPatientId_LoadsRecords()
        {
            int patientId = 1;
            var mockRecords = new List<MedicalRecordJointModel>
            {
                new MedicalRecordJointModel(1, patientId, "John", 1, "Dr. Smith", 1, "Cardio", 1, "ECG", DateTime.Now, "All good")
            };

            _mockDbService.Setup(s => s.GetMedicalRecordsForPatient(patientId))
                          .ReturnsAsync(mockRecords);

            await _manager.LoadMedicalRecordsForPatient(patientId);

            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(1));
            Assert.That(_manager.MedicalRecords[0].PatientId, Is.EqualTo(patientId));
        }

        [Test]
        public async Task LoadMedicalRecordsForPatient_NullReturned_InitializesEmptyList()
        {
            int patientId = 1;

            _mockDbService.Setup(s => s.GetMedicalRecordsForPatient(patientId))!
                          .ReturnsAsync((List<MedicalRecordJointModel>?)null);

            await _manager.LoadMedicalRecordsForPatient(patientId);

            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task LoadMedicalRecordsForPatient_ThrowsException_DoesNotCrash()
        {
            int patientId = 1;

            _mockDbService.Setup(s => s.GetMedicalRecordsForPatient(patientId))
                          .ThrowsAsync(new Exception("Something failed"));

            await _manager.LoadMedicalRecordsForPatient(patientId);

            Assert.That(_manager.MedicalRecords.Count, Is.EqualTo(0));
        }


    }
}