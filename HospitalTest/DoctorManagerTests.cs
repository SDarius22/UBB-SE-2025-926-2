using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Tests.Managers
{
    [TestFixture]
    public class DoctorManagerTests
    {
        private Mock<IDoctorsDatabaseService> _mockDatabaseService;
        private DoctorManager _doctorManager;

        [SetUp]
        public void SetUp()
        {
            _mockDatabaseService = new Mock<IDoctorsDatabaseService>();
            _doctorManager = new DoctorManager(_mockDatabaseService.Object);
        }

        [Test]
        public async Task LoadDoctors_ValidDepartment_PopulatesDoctorsList()
        {
            var departmentId = 1;
            var doctors = new List<DoctorJointModel>
            {
                new DoctorJointModel(1, 101, "Dr. House", 1, 4.8, "ABC123"),
                new DoctorJointModel(2, 102, "Dr. Grey", 1, 4.6, "DEF456")
            };


            _mockDatabaseService.Setup(s => s.GetDoctorsByDepartment(departmentId))
                .ReturnsAsync(doctors);

            await _doctorManager.LoadDoctors(departmentId);

            Assert.That(_doctorManager.Doctors.Count, Is.EqualTo(2));
            Assert.That(_doctorManager.Doctors[0].DoctorName, Is.EqualTo("Dr. House"));
        }

        [Test]
        public async Task LoadDoctors_WhenExceptionThrown_LogsErrorAndKeepsListEmpty()
        {
            var departmentId = 1;
            _mockDatabaseService.Setup(s => s.GetDoctorsByDepartment(departmentId))
                .ThrowsAsync(new Exception("DB error"));

            await _doctorManager.LoadDoctors(departmentId);

            Assert.That(_doctorManager.Doctors.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetDoctorsWithRatings_ReturnsCurrentDoctorsList()
        {
            _doctorManager.Doctors.Add(new DoctorJointModel(3, 103, "Dr. Wilson", 2, 4.2, "XYZ789"));
            var result = _doctorManager.GetDoctorsWithRatings();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].DoctorName, Is.EqualTo("Dr. Wilson"));
        }
    }
}
