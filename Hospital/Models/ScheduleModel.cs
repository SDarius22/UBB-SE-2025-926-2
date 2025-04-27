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

        public ScheduleModel(int doctorId, int shiftId)
        {
            DoctorId = doctorId;
            ShiftId = shiftId;
        }
    }
}
