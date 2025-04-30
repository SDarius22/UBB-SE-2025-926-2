using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class ScheduleModel
    {
        public int DoctorId { get; set; }

        public int ShiftId { get; set; }

        public int ScheduleId { get; set; }

        public ScheduleModel(int doctorId, int shiftId, int scheduleId)
        {
            DoctorId = doctorId;
            ShiftId = shiftId;
            ScheduleId = scheduleId;
        }
    }
}