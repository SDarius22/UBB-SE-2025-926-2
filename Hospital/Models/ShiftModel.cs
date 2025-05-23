﻿using System;
using System.ComponentModel;

namespace Hospital.Models
{
    public class ShiftModel
    {
        public int ShiftID { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public ShiftModel()
        {
        }

        public ShiftModel(int shiftId, DateOnly date, TimeSpan startTime, TimeSpan endTime)
        {
            ShiftID = shiftId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
