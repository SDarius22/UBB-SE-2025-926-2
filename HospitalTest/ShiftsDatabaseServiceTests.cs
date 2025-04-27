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
    public class ShiftsDatabaseServiceTests
    {
        private IShiftsDatabaseService _service;
        private ApplicationConfiguration _config;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _config = ApplicationConfiguration.GetInstance();
        }

        [SetUp]
        public void SetUp()
        {
            _service = new ShiftsDatabaseService();
        }

        [Test]
        public async Task GetShifts_ReturnsShiftsList()
        {
            // Act
            var result = await _service.GetShifts();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ShiftModel));
            });
        }


        [Test]
        public async Task GetSchedules_ReturnsSchedulesList()
        {
            // Act
            var result = await _service.GetSchedules();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ScheduleModel));
            });
        }

   

        [Test]
        public async Task GetShiftsByDoctorId_ValidId_ReturnsShifts()
        {
            // Arrange
            int doctorId = 1;

            // Act
            var result = await _service.GetShiftsByDoctorId(doctorId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ShiftModel));
            });
        }

        [Test]
        public void GetShiftsByDoctorId_InvalidId_ThrowsException()
        {
            // Arrange
            int doctorId = -1;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ShiftNotFoundException>(
                async () => await _service.GetShiftsByDoctorId(doctorId)
            );
            Assert.That(exception.Message, Does.Contain("Error loading shifts for doctor"));
        }

        [Test]
        public async Task GetDoctorDaytimeShifts_ValidId_ReturnsShifts()
        {
            // Arrange
            int doctorId = 1;

            // Act
            var result = await _service.GetDoctorDaytimeShifts(doctorId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ShiftModel));
                foreach (var shift in result)
                {
                    Assert.That(shift.StartTime.Hours, Is.LessThan(20), "Daytime shifts should start before 20:00");
                }
            });
        }

        [Test]
        public void GetDoctorDaytimeShifts_InvalidId_ThrowsException()
        {
            // Arrange
            int doctorId = -1;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ShiftNotFoundException>(
                async () => await _service.GetDoctorDaytimeShifts(doctorId)
            );
            Assert.That(exception.Message, Does.Contain("Error loading upcoming shifts for doctor"));
        }

        [Test]
        public async Task GetDoctorDaytimeShifts_NoShifts_ReturnsEmptyList()
        {
            // Arrange
            int doctorId = 999; // Use a doctor ID that shouldn't have any shifts

            // Act
            var result = await _service.GetDoctorDaytimeShifts(doctorId);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
