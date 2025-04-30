using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class ShiftModel
    {
        public int ShiftId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public ShiftModel(int shiftId, DateOnly date, TimeSpan startTime, TimeSpan endTime)
        {
            ShiftId = shiftId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}