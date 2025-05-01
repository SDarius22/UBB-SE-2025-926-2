using System;

namespace Hospital.Helpers
{

    public static class TimeRounder
    {
        private static int SlotDuration = 30;
        public static TimeSpan RoundProcedureDuration(TimeSpan procedureDuration)
        {
            // Double slotDuration = ApplicationConfiguration.GetInstance().SlotDuration;
            //    int totalMinutes = (int)initialDuration.TotalMinutes;
            //    int roundedMinutes = (int)Math.Round(totalMinutes / slotDuration) * (int)slotDuration;
            //    return TimeSpan.FromMinutes(roundedMinutes);
            double slotDuration = SlotDuration;

            int procedureDurationInMinutes = (int)procedureDuration.TotalMinutes;

            int slotDurationAllocatedForProcedure = (int)Math.Round(procedureDurationInMinutes / slotDuration) * (int)slotDuration; // Round to nearest multiple of 30

            return TimeSpan.FromMinutes(slotDurationAllocatedForProcedure); // Convert back to TimeSpan
        }

    }

}
