using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.Models;
using Hospital.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Tests.Managers
{
    [TestFixture]
    public class AppointmentManagerTests
    {
        private Mock<IAppointmentsDatabaseService> _mockDatabaseService;
        private AppointmentManager _appointmentManager;

        [SetUp]
        public void SetUp()
        {
            _mockDatabaseService = new Mock<IAppointmentsDatabaseService>();
            _appointmentManager = new AppointmentManager(_mockDatabaseService.Object);
        }

        #region LoadDoctorAppointmentsOnDate Tests

        [Test]
        public void GetAppointments_ReturnsCurrentAppointments()
        {
            var result = _appointmentManager.GetAppointments();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task LoadDoctorAppointmentsOnDate_ValidData_AppointmentsLoaded()
        {
            var doctorId = 1;
            var date = DateTime.Now;
            var appointments = new List<AppointmentJointModel>
            {
                new AppointmentJointModel { DoctorId = doctorId, DateAndTime = date }
            };

            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(doctorId, date))
                                .ReturnsAsync(appointments);

            await _appointmentManager.LoadDoctorAppointmentsOnDate(doctorId, date);

            Assert.That(_appointmentManager.Appointments.Count, Is.EqualTo(appointments.Count));
        }

        [Test]
        public Task LoadDoctorAppointmentsOnDate_ThrowsException_WhenDatabaseCallFails()
        {
            var doctorId = 1;
            var date = DateTime.Now;
            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(doctorId, date))
                                .Throws(new Exception("Mocked database exception"));

            Assert.ThrowsAsync<Exception>(() => _appointmentManager.LoadDoctorAppointmentsOnDate(doctorId, date));
            return Task.CompletedTask;
        }

        #endregion

        [Test]
        public async Task LoadAppointmentsForDoctor_ValidData_AppointmentsLoaded()
        {
            var doctorId = 1;
            var appointments = new List<AppointmentJointModel>
            {
                new AppointmentJointModel { DoctorId = doctorId, DateAndTime = DateTime.Now }
            };

            _mockDatabaseService.Setup(s => s.GetAppointmentsForDoctor(doctorId))
                                .ReturnsAsync(appointments);

            await _appointmentManager.LoadAppointmentsForDoctor(doctorId);

            Assert.That(_appointmentManager.Appointments.Count, Is.EqualTo(appointments.Count));
        }

        [Test]
        public void LoadAppointmentsForDoctor_ThrowsException_ThrowsWrapped()
        {
            var doctorId = 1;

            _mockDatabaseService.Setup(s => s.GetAppointmentsForDoctor(doctorId))
                                .ThrowsAsync(new Exception("Failure"));

            var ex = Assert.ThrowsAsync<Exception>(() => _appointmentManager.LoadAppointmentsForDoctor(doctorId));
            Assert.That(ex.Message, Does.Contain("Error loading appointments for doctor"));
        }


        [Test]
        public async Task LoadAppointmentsByDoctorAndDate_ValidData_AppointmentsLoaded()
        {
            var doctorId = 1;
            var date = DateTime.Today;
            var appointments = new List<AppointmentJointModel>
            {
                new AppointmentJointModel { DoctorId = doctorId, DateAndTime = date }
            };

            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(doctorId, date))
                                .ReturnsAsync(appointments);

            await _appointmentManager.LoadAppointmentsByDoctorAndDate(doctorId, date);

            Assert.That(_appointmentManager.Appointments.Count, Is.EqualTo(appointments.Count));
        }

        [Test]
        public void LoadAppointmentsByDoctorAndDate_ThrowsException_ThrowsWrapped()
        {
            var doctorId = 1;
            var date = DateTime.Today;

            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(doctorId, date))
                                .ThrowsAsync(new Exception("Database error"));

            var ex = Assert.ThrowsAsync<Exception>(() => _appointmentManager.LoadAppointmentsByDoctorAndDate(doctorId, date));

            Assert.That(ex.Message, Does.Contain($"Error loading appointments for doctor {doctorId}"));
        }


        [Test]
        public void CreateAppointment_DbInsertFails_ThrowsDatabaseOperationException()
        {
            var appointment = new AppointmentModel
            {
                DoctorId = 1,
                PatientId = 1,
                DateAndTime = DateTime.Now.AddDays(1),
                ProcedureId = 1
            };

            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(appointment.DoctorId, appointment.DateAndTime))
                                .ReturnsAsync(new List<AppointmentJointModel>());
            _mockDatabaseService.Setup(s => s.GetAppointmentsForPatient(appointment.PatientId))
                                .ReturnsAsync(new List<AppointmentJointModel>());
            _mockDatabaseService.Setup(s => s.AddAppointmentToDataBase(appointment))
                                .ReturnsAsync(false);

            Assert.ThrowsAsync<DatabaseOperationException>(() => _appointmentManager.CreateAppointment(appointment));
        }

        [Test]
        public void CreateAppointment_InsertFails_ThrowsDatabaseOperationException()
        {
            var appointment = new AppointmentModel
            {
                DoctorId = 1,
                PatientId = 2,
                DateAndTime = DateTime.Now.AddHours(1),
                ProcedureId = 3,
                Finished = false
            };

            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(appointment.DoctorId, appointment.DateAndTime))
                                .ReturnsAsync(new List<AppointmentJointModel>());

            _mockDatabaseService.Setup(s => s.GetAppointmentsForPatient(appointment.PatientId))
                                .ReturnsAsync(new List<AppointmentJointModel>());

            _mockDatabaseService.Setup(s => s.AddAppointmentToDataBase(appointment))
                                .ReturnsAsync(false);

            Assert.ThrowsAsync<DatabaseOperationException>(() => _appointmentManager.CreateAppointment(appointment));
        }

        [Test]
        public void CreateAppointment_PatientAlreadyBookedAtSameTime_ThrowsAppointmentConflictException()
        {
            var newAppointment = new AppointmentModel
            {
                DoctorId = 1,
                PatientId = 2,
                DateAndTime = DateTime.Now.AddHours(1),
                ProcedureId = 3,
                Finished = false
            };

            var existingPatientAppointment = new AppointmentJointModel
            {
                PatientId = newAppointment.PatientId,
                DateAndTime = newAppointment.DateAndTime
            };

            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(newAppointment.DoctorId, newAppointment.DateAndTime))
                                .ReturnsAsync(new List<AppointmentJointModel>()); // doctor is free

            _mockDatabaseService.Setup(s => s.GetAppointmentsForPatient(newAppointment.PatientId))
                                .ReturnsAsync(new List<AppointmentJointModel> { existingPatientAppointment }); // patient busy

            Assert.ThrowsAsync<AppointmentConflictException>(() => _appointmentManager.CreateAppointment(newAppointment));
        }



        [Test]
        public void GetAppointments_ReturnsCurrentList()
        {
            var result = _appointmentManager.GetAppointments();
            Assert.IsNotNull(result);
        }

        #region LoadAppointmentsForPatient Tests

        [Test]
        public async Task LoadAppointmentsForPatient_ValidData_AppointmentsLoaded()
        {
            var patientId = 1;
            var appointments = new List<AppointmentJointModel>
            {
                new AppointmentJointModel { PatientId = patientId, DateAndTime = DateTime.Now.AddHours(1), Finished = false },
                new AppointmentJointModel { PatientId = patientId, DateAndTime = DateTime.Now.AddHours(-1), Finished = false }
            };

            _mockDatabaseService.Setup(s => s.GetAppointmentsForPatient(patientId))
                                .ReturnsAsync(appointments);

            await _appointmentManager.LoadAppointmentsForPatient(patientId);

            Assert.That(_appointmentManager.Appointments.Count, Is.EqualTo(1));
        }

        [Test]
        public Task LoadAppointmentsForPatient_ThrowsException_WhenDatabaseCallFails()
        {
            var patientId = 1;
            _mockDatabaseService.Setup(s => s.GetAppointmentsForPatient(patientId))
                                .Throws(new Exception("Mocked database exception"));

            Assert.ThrowsAsync<Exception>(() => _appointmentManager.LoadAppointmentsForPatient(patientId));
            return Task.CompletedTask;
        }

        #endregion

        #region RemoveAppointment Tests

        [Test]
        public async Task RemoveAppointment_ValidAppointment_Removed()
        {
            var appointmentId = 1;
            var appointment = new AppointmentJointModel { DateAndTime = DateTime.Now.AddHours(24.1) };
            _mockDatabaseService.Setup(s => s.GetAppointment(appointmentId))
                                .ReturnsAsync(appointment);
            _mockDatabaseService.Setup(s => s.RemoveAppointmentFromDataBase(appointmentId))
                                .ReturnsAsync(true);

            var result = await _appointmentManager.RemoveAppointment(appointmentId);

            Assert.IsTrue(result);
        }

        [Test]
        public Task RemoveAppointment_AppointmentNotFound_ThrowsException()
        {
            var appointmentId = 1;
            _mockDatabaseService.Setup(s => s.GetAppointment(appointmentId))!
                                .ReturnsAsync((AppointmentJointModel?)null);

            Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentManager.RemoveAppointment(appointmentId));
            return Task.CompletedTask;
        }

        [Test]
        public Task RemoveAppointment_AppointmentWithin24Hours_ThrowsException()
        {
            var appointmentId = 1;
            var appointment = new AppointmentJointModel { DateAndTime = DateTime.Now.AddHours(12) };
            _mockDatabaseService.Setup(s => s.GetAppointment(appointmentId))
                                .ReturnsAsync(appointment);

            Assert.ThrowsAsync<CancellationNotAllowedException>(() => _appointmentManager.RemoveAppointment(appointmentId));
            return Task.CompletedTask;
        }

        #endregion

        #region CreateAppointment Tests

        [Test]
        public Task CreateAppointment_ValidData_AppointmentCreated()
        {
            var newAppointment = new AppointmentModel { DoctorId = 1, PatientId = 1, DateAndTime = DateTime.Now.AddHours(1) };
            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(newAppointment.DoctorId, newAppointment.DateAndTime))
                                .ReturnsAsync(new List<AppointmentJointModel>());
            _mockDatabaseService.Setup(s => s.GetAppointmentsForPatient(newAppointment.PatientId))
                                .ReturnsAsync(new List<AppointmentJointModel>());
            _mockDatabaseService.Setup(s => s.AddAppointmentToDataBase(newAppointment))
                                .ReturnsAsync(true);

            Assert.DoesNotThrowAsync(() => _appointmentManager.CreateAppointment(newAppointment));
            return Task.CompletedTask;
        }

        [Test]
        public Task CreateAppointment_TimeSlotTaken_ThrowsException()
        {
            var newAppointment = new AppointmentModel { DoctorId = 1, PatientId = 1, DateAndTime = DateTime.Now.AddHours(1) };
            var existingAppointment = new AppointmentJointModel { DoctorId = newAppointment.DoctorId, DateAndTime = newAppointment.DateAndTime };
            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(newAppointment.DoctorId, newAppointment.DateAndTime))
                                .ReturnsAsync(new List<AppointmentJointModel> { existingAppointment });

            Assert.ThrowsAsync<AppointmentConflictException>(() => _appointmentManager.CreateAppointment(newAppointment));
            return Task.CompletedTask;
        }

        #endregion

        #region CanCancelAppointment Tests

        [Test]
        public void CanCancelAppointment_AppointmentIsCancelable_ReturnsTrue()
        {
            var appointment = new AppointmentJointModel { DateAndTime = DateTime.Now.AddHours(24.01) };

            var result = _appointmentManager.CanCancelAppointment(appointment);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanCancelAppointment_AppointmentIsNotCancelable_ReturnsFalse()
        {
            var appointment = new AppointmentJointModel { DateAndTime = DateTime.Now.AddHours(12) };

            var result = _appointmentManager.CanCancelAppointment(appointment);

            Assert.IsFalse(result);
        }

        #endregion

        [Test]
        public void RemoveAppointment_DeleteFails_ThrowsDatabaseOperationException()
        {
            var appointmentId = 1;
            var appointment = new AppointmentJointModel { DateAndTime = DateTime.Now.AddHours(25) };

            _mockDatabaseService.Setup(s => s.GetAppointment(appointmentId))
                                .ReturnsAsync(appointment);
            _mockDatabaseService.Setup(s => s.RemoveAppointmentFromDataBase(appointmentId))
                                .ReturnsAsync(false);

            Assert.ThrowsAsync<DatabaseOperationException>(() => _appointmentManager.RemoveAppointment(appointmentId));
        }

        [Test]
        public void RemoveAppointment_UnexpectedException_ThrowsWrappedException()
        {
            var appointmentId = 1;
            _mockDatabaseService.Setup(s => s.GetAppointment(appointmentId))
                                .ThrowsAsync(new Exception("Unexpected"));

            var ex = Assert.ThrowsAsync<Exception>(() => _appointmentManager.RemoveAppointment(appointmentId));
            Assert.That(ex.Message, Does.Contain("Unexpected error removing appointment"));
        }

        [Test]
        public void MarkAppointmentAsCompletedInDatabase_NotImplemented_ThrowsException()
        {
            Assert.ThrowsAsync<NotImplementedException>(() => AppointmentManager.MarkAppointmentAsCompletedInDatabase(1));
        }

        [Test]
        public void CanCancelAppointment_NullAppointment_ReturnsFalse()
        {
            var result = _appointmentManager.CanCancelAppointment(null!);
            Assert.IsFalse(result);
        }
        
        [Test]
        public Task RemoveAppointment_InvalidAppointmentId_ThrowsCorrectMessage()
        {
            var id = 99;
            _mockDatabaseService.Setup(s => s.GetAppointment(id))!.ReturnsAsync((AppointmentJointModel?)null);

            var ex = Assert.ThrowsAsync<AppointmentNotFoundException>(() => _appointmentManager.RemoveAppointment(id));
            Assert.That(ex.Message, Does.Contain($"Appointment with ID {id} not found"));
            return Task.CompletedTask;
        }

        [Test]
        public void LoadAppointmentsByDoctorAndDate_ThrowsException_WhenDatabaseFails()
        {
            var doctorId = 1;
            var date = DateTime.Today;

            _mockDatabaseService.Setup(s => s.GetAppointmentsByDoctorAndDate(doctorId, date))
                                .ThrowsAsync(new Exception("Mock error"));

            var ex = Assert.ThrowsAsync<Exception>(() => _appointmentManager.LoadAppointmentsByDoctorAndDate(doctorId, date));
            Assert.That(ex.Message, Does.Contain("Error loading appointments for doctor"));
        }

        [Test]
        public void LoadAppointmentsForDoctor_ThrowsException_WhenDatabaseFails()
        {
            var doctorId = 1;

            _mockDatabaseService.Setup(s => s.GetAppointmentsForDoctor(doctorId))
                                .ThrowsAsync(new Exception("Mock error"));

            var ex = Assert.ThrowsAsync<Exception>(() => _appointmentManager.LoadAppointmentsForDoctor(doctorId));
            Assert.That(ex.Message, Does.Contain("Error loading appointments for doctor"));
        }

    }
}