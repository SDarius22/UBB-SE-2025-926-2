using Hospital.Configs;
using Hospital.Helpers;
using NUnit.Framework;
using System;

namespace Hospital.Tests.Helpers
{
    [TestFixture]
    public class TimeRounderTests
    {
        [SetUp]
        public void SetUp()
        {
            ApplicationConfiguration.GetInstance().SlotDuration = 30;
        }

        [TestCase(25, 30)]
        [TestCase(44, 30)]
        [TestCase(46, 60)]
        [TestCase(74, 60)]
        [TestCase(76, 90)]
        [TestCase(0, 0)]
        public void RoundProcedureDuration_RoundsToNearestSlot(int inputMinutes, int expectedMinutes)
        {
            var input = TimeSpan.FromMinutes(inputMinutes);
            var expected = TimeSpan.FromMinutes(expectedMinutes);

            var result = TimeRounder.RoundProcedureDuration(input);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
