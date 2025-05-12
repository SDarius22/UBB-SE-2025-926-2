using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class ScheduleModel
    {
        public int DoctorId { get; set; }

        public int ShiftId { get; set; }

        public int ScheduleId { get; set; }

        public virtual DoctorJointModel? Doctor { get; set; }

        public virtual ShiftModel? Shift { get; set; }

        public ScheduleModel()
        {
        }

        public ScheduleModel(int doctorId, int shiftId, int scheduleId)
        {
            DoctorId = doctorId;
            ShiftId = shiftId;
            ScheduleId = scheduleId;
        }
    }
}