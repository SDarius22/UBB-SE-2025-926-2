using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Managers;
using Hospital.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Tests.Managers
{
    [TestFixture]
    public class ShiftManagerTests
    {
        private Mock<IShiftsDatabaseService> _mockShiftService;
        private ShiftManager _manager;

        [SetUp]
        public void SetUp()
        {
            _mockShiftService = new Mock<IShiftsDatabaseService>();
            _manager = new ShiftManager(_mockShiftService.Object);
        }

        [Test]
        public async Task LoadShifts_LoadsFromService()
        {
            var shifts = new List<ShiftModel>
            {
                new ShiftModel(1, DateTime.Today, TimeSpan.FromHours(8), TimeSpan.FromHours(16))
            };

            _mockShiftService.Setup(s => s.GetShiftsByDoctorId(1)).ReturnsAsync(shifts);

            await _manager.LoadShifts(1);

            Assert.That(_manager.GetShifts().Count, Is.EqualTo(1));
        }

        [Test]
        public async Task LoadUpcomingDoctorDayshifts_LoadsFromService()
        {
            var shifts = new List<ShiftModel>
            {
                new ShiftModel(2, DateTime.Today.AddDays(1), TimeSpan.FromHours(8), TimeSpan.FromHours(16))
            };

            _mockShiftService.Setup(s => s.GetDoctorDaytimeShifts(1)).ReturnsAsync(shifts);

            await _manager.LoadUpcomingDoctorDayshifts(1);

            Assert.That(_manager.GetShifts().Count, Is.EqualTo(1));
        }

        [Test]
        public void GetShifts_ReturnsCurrentList()
        {
            var shift = new ShiftModel(1, DateTime.Today, TimeSpan.FromHours(8), TimeSpan.FromHours(16));
            _manager.GetShifts().Add(shift);

            var result = _manager.GetShifts();

            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetShiftByDay_ShiftExists_ReturnsShift()
        {
            var today = DateTime.Today;
            var shift = new ShiftModel(1, today, TimeSpan.FromHours(8), TimeSpan.FromHours(16));
            _manager.GetShifts().Add(shift);

            var result = _manager.GetShiftByDay(today);

            Assert.That(result.DateTime.Date, Is.EqualTo(today.Date));
        }

        [Test]
        public void GetShiftByDay_ShiftNotFound_ThrowsException()
        {
            Assert.Throws<ShiftNotFoundException>(() => _manager.GetShiftByDay(DateTime.Today));
        }

        [Test]
        public void GetMonthlyCalendarRange_ReturnsFirstAndLastDayOfMonth()
        {
            var (start, end) = _manager.GetMonthlyCalendarRange();
            var expectedStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var expectedEnd = expectedStart.AddMonths(1).AddDays(-1);

            Assert.That(start.Date, Is.EqualTo(expectedStart));
            Assert.That(end.Date, Is.EqualTo(expectedEnd));
        }

        [Test]
        public void GenerateTimeSlots_CreatesCorrectSlots()
        {
            var date = DateTime.Today;
            var shift = new ShiftModel(1, date, TimeSpan.FromHours(8), TimeSpan.FromHours(10));
            var appointment = new AppointmentJointModel(
                1, false, date.AddHours(9), 1, "Cardio", 1, "Dr. A", 1, "Patient A", 1, "MRI", TimeSpan.FromMinutes(30));

            var result = _manager.GenerateTimeSlots(date, new List<ShiftModel> { shift }, new List<AppointmentJointModel> { appointment });

            Assert.That(result.Count, Is.EqualTo(48));

            var slotAt9 = result.FirstOrDefault(s => s.TimeSlot.Hour == 9 && s.TimeSlot.Minute == 0);
            Assert.NotNull(slotAt9);
            Assert.That(slotAt9.Appointment, Is.EqualTo("MRI"));
        }

        [Test]
        public void GenerateTimeSlots_HandlesOvernightShiftCorrectly()
        {
            var date = new DateTime(2025, 4, 15);
            var shift = new ShiftModel(1, date, new TimeSpan(22, 0, 0), new TimeSpan(2, 0, 0)); // 10PM to 2AM
            var shifts = new List<ShiftModel> { shift };

            var appointmentTime = date.AddHours(23);
            var appointments = new List<AppointmentJointModel>
    {
        new AppointmentJointModel
        {
            DateAndTime = appointmentTime,
            ProcedureName = "X-Ray"
        }
    };

            var manager = new ShiftManager(null);

            var result = manager.GenerateTimeSlots(date, shifts, appointments);

            var slot = result.FirstOrDefault(s => s.TimeSlot == appointmentTime);
            Assert.That(slot, Is.Not.Null);
            Assert.That(slot.HighlightStatus, Is.EqualTo("Booked"));
            Assert.That(slot.Appointment, Is.EqualTo("X-Ray"));
        }

        [Test]
        public void GenerateTimeSlots_OutsideShift_HighlightIsNone()
        {
            var date = new DateTime(2025, 4, 15);
            var shift = new ShiftModel(1, date, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)); // 10–12
            var shifts = new List<ShiftModel> { shift };
            var appointments = new List<AppointmentJointModel>();

            var manager = new ShiftManager(null);
            var slots = manager.GenerateTimeSlots(date, shifts, appointments);

            var slotBefore = slots.FirstOrDefault(s => s.TimeSlot == date.AddHours(9));
            Assert.That(slotBefore?.HighlightStatus, Is.EqualTo("None"));
        }

    }
}