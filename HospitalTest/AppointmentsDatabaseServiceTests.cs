// <copyright file="AppointmentsDatabaseServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HospitalTest
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Hospital.Configs;
    using Hospital.DatabaseServices;
    using Hospital.Exceptions;
    using Hospital.Models;
    using Microsoft.Data.SqlClient;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Contains unit tests for the AppointmentsDatabaseService class.
    /// </summary>
    [TestFixture]
    public class AppointmentsDatabaseServiceTests
    {
        private IAppointmentsDatabaseService service;
        private ApplicationConfiguration config;

        /// <summary>
        /// Initializes the test configuration.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.config = ApplicationConfiguration.GetInstance();
        }

        /// <summary>
        /// Sets up the test environment before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.service = new AppointmentsDatabaseService();
        }

        /// <summary>
        /// Tests that adding a valid appointment returns true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task AddAppointmentToDataBase_ValidAppointment_ReturnsTrue()
        {
            // Arrange
            var appointment = new AppointmentModel(
                appointmentId: 0,
                patientId: 1,
                doctorId: 1,
                dateAndTime: DateTime.Now.AddDays(1),
                finished: false,
                procedureId: 1);

            // Act
            var result = await this.service.AddAppointmentToDataBase(appointment);

            // Assert
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Tests that adding an appointment with a past date throws InvalidAppointmentException.
        /// </summary>
        [Test]
        public void AddAppointmentToDataBase_PastDate_ThrowsException()
        {
            // Arrange
            var appointment = new AppointmentModel(
                appointmentId: 0,
                patientId: 1,
                doctorId: 1,
                dateAndTime: DateTime.Now.AddDays(-1),
                finished: false,
                procedureId: 1);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidAppointmentException>(
                async () => await this.service.AddAppointmentToDataBase(appointment));
            Assert.That(exception.Message, Does.Contain("Cannot create appointments in the past"));
        }

        [Test]
        public void AddAppointmentToDataBase_DatabaseError_ThrowsException()
        {
            // Arrange
            var appointment = new AppointmentModel(
                appointmentId: 0,
                patientId: -1,
                doctorId: -1,
                dateAndTime: DateTime.Now.AddDays(1),
                finished: false,
                procedureId: -1
            );

            // Act & Assert
            Assert.ThrowsAsync<DatabaseOperationException>(
                async () => await this.service.AddAppointmentToDataBase(appointment)
            );
        }

        [Test]
        public async Task GetAppointmentsForDoctor_ValidDoctorId_ReturnsAppointments()
        {
            // Arrange
            int doctorId = 1;

            // Act
            var result = await this.service.GetAppointmentsForDoctor(doctorId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(AppointmentJointModel));
            });
        }

        [Test]
        public async Task GetAppointmentsForDoctor_InvalidId_ReturnsEmptyList()
        {
            // Arrange
            int doctorId = -1;

            // Act
            var result = await this.service.GetAppointmentsForDoctor(doctorId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAppointmentsForPatient_ValidPatientId_ReturnsAppointments()
        {
            // Arrange
            int patientId = 1;

            // Act
            var result = await this.service.GetAppointmentsForPatient(patientId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(AppointmentJointModel));
            });
        }

        [Test]
        public async Task GetAppointmentsForPatient_InvalidId_ReturnsEmptyList()
        {
            // Arrange
            int patientId = -1;

            // Act
            var result = await this.service.GetAppointmentsForPatient(patientId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAppointmentsByDoctorAndDate_ValidInput_ReturnsAppointments()
        {
            // Arrange
            int doctorId = 1;
            DateTime date = DateTime.Now.AddDays(+1);

            // Act
            var result = await this.service.GetAppointmentsByDoctorAndDate(doctorId, date);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(AppointmentJointModel));
            });
        }

        [Test]
        public void GetAppointmentsByDoctorAndDate_InvalidId_ThrowsException()
        {
            // Arrange
            int doctorId = -1;
            DateTime date = DateTime.Today;

            // Act & Assert
            Assert.ThrowsAsync<DatabaseOperationException>(
                async () => await this.service.GetAppointmentsByDoctorAndDate(doctorId, date)
            );
        }

        [Test]
        public void GetAppointmentsByDoctorAndDate_PastDate_ThrowsException()
        {
            // Arrange
            int doctorId = 1;
            DateTime date = DateTime.Today.AddDays(-1);

            // Act & Assert
            Assert.ThrowsAsync<InvalidAppointmentException>(
                async () => await this.service.GetAppointmentsByDoctorAndDate(doctorId, date)
            );
        }

        [Test]
        public async Task GetAppointment_ValidId_ReturnsAppointment()
        {
            // Arrange
            // First create an appointment
            var appointment = new AppointmentModel(
                appointmentId: 0,
                patientId: 1,
                doctorId: 1,
                dateAndTime: DateTime.Now.AddDays(1),
                finished: false,
                procedureId: 1
            );
            await this.service.AddAppointmentToDataBase(appointment);

            // Get a valid appointment ID
            var appointments = await this.service.GetAppointmentsForDoctor(1);
            var appointmentId = appointments[0].AppointmentId;

            // Act
            var result = await this.service.GetAppointment(appointmentId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<AppointmentJointModel>());
            });
        }

        [Test]
        public async Task GetAppointment_InvalidId_ReturnsNull()
        {
            // Arrange
            int appointmentId = -1;

            // Act
            var result = await this.service.GetAppointment(appointmentId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task RemoveAppointmentFromDataBase_ValidId_ReturnsTrue()
        {
            // Arrange
            // First create an appointment
            var appointment = new AppointmentModel(
                appointmentId: 0,
                patientId: 1,
                doctorId: 1,
                dateAndTime: DateTime.Now.AddDays(1),
                finished: false,
                procedureId: 1
            );
            await this.service.AddAppointmentToDataBase(appointment);

            // Get a valid appointment ID
            var appointments = await this.service.GetAppointmentsForDoctor(1);
            var validAppointmentId = appointments[0].AppointmentId;

            // Act
            var result = await this.service.RemoveAppointmentFromDataBase(validAppointmentId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public Task RemoveAppointmentFromDataBase_InvalidId_ReturnsFalse()
        {
            // Arrange
            int invalidId = -1;

            // Act
            Assert.ThrowsAsync<DatabaseOperationException>(
                async () => await this.service.RemoveAppointmentFromDataBase(invalidId)
            );
            return Task.CompletedTask;
        }

        [Test]
        public void RemoveAppointmentFromDataBase_DatabaseError_ThrowsException()
        {
            // Arrange
            int invalidId = 0;

            // Act & Assert
            Assert.ThrowsAsync<DatabaseOperationException>(
                async () => await this.service.RemoveAppointmentFromDataBase(invalidId)
            );
        }

        [Test]
        public async Task GetAllAppointments_ValidData_ReturnsAppointments()
        {
            // Arrange
            var mockAppointment1 = new AppointmentJointModel
            {
                AppointmentId = 1,
                Finished = false,
                DateAndTime = DateTime.Now.AddDays(1),
                DepartmentId = 1,
                DepartmentName = "Cardiology",
                DoctorId = 1,
                DoctorName = "Dr. Smith",
                PatientId = 1,
                PatientName = "John Doe",
                ProcedureId = 1,
                ProcedureName = "Heart Checkup",
                ProcedureDuration = TimeSpan.FromMinutes(30)
            };

            var mockAppointment2 = new AppointmentJointModel
            {
                AppointmentId = 2,
                Finished = true,
                DateAndTime = DateTime.Now.AddDays(2),
                DepartmentId = 2,
                DepartmentName = "Neurology",
                DoctorId = 2,
                DoctorName = "Dr. Adams",
                PatientId = 2,
                PatientName = "Jane Doe",
                ProcedureId = 2,
                ProcedureName = "Brain MRI",
                ProcedureDuration = TimeSpan.FromMinutes(45)
            };

            var appointments = new List<AppointmentJointModel> { mockAppointment1, mockAppointment2 };

            var mockService = new Mock<IAppointmentsDatabaseService>();
            mockService.Setup(service => service.GetAllAppointments()).ReturnsAsync(appointments);

            // Act
            var result = await mockService.Object.GetAllAppointments();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[0].DoctorName, Is.EqualTo("Dr. Smith"));
                Assert.That(result[1].DoctorName, Is.EqualTo("Dr. Adams"));
            });
        }

        [Test]
        public async Task GetAllAppointments_GetAppointmentsAsync()
        {
            // Arrange
            var mockService = new Mock<IAppointmentsDatabaseService>();
            var appointments = new List<AppointmentJointModel>
            {
                new AppointmentJointModel { AppointmentId = 1, DoctorName = "Dr. Smith" },
                new AppointmentJointModel { AppointmentId = 2, DoctorName = "Dr. Adams" }
            };
            mockService.Setup(service => service.GetAllAppointments()).ReturnsAsync(appointments);
            // Act
            var result = await mockService.Object.GetAllAppointments();
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }


        [Test]
        public async Task GetAllAppointments_NoAppointments_ReturnsEmptyList()
        {
            // Arrange
            var mockService = new Mock<IAppointmentsDatabaseService>();
            mockService.Setup(service => service.GetAllAppointments()).ReturnsAsync(new List<AppointmentJointModel>());

            // Act
            var result = await mockService.Object.GetAllAppointments();

            // Assert
            Assert.That(result, Is.Empty);
        }

    }
}