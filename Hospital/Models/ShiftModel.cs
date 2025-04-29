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
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public ShiftModel(int shiftId, DateTime dateTime, TimeSpan startTime, TimeSpan endtime)
        {
            ShiftId = shiftId;
            Date = dateTime;
            StartTime = startTime;
            EndTime = endtime;
        }
    }
}