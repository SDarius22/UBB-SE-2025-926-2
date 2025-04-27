using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class AppointmentJointModel
    {
        public int AppointmentId { get; set; }

        public bool Finished { get; set; }

        public DateTime DateAndTime { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int DoctorId { get; set; }

        public string DoctorName { get; set; }

        public int PatientId { get; set; }

        public string PatientName { get; set; }

        public int ProcedureId { get; set; }

        public string ProcedureName { get; set; }

        public TimeSpan ProcedureDuration { get; set; }


        public AppointmentJointModel(int appointmentId, bool finished, DateTime dateAndTime, int departmentId, string departmentName, int doctorId, string doctorName, int patientId, string patientName, int procedureId, string procedureName, TimeSpan procedureDuration)
        {
            AppointmentId = appointmentId;
            Finished = finished;
            DateAndTime = dateAndTime;
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            DoctorId = doctorId;
            DoctorName = doctorName;
            PatientId = patientId;
            PatientName = patientName;
            ProcedureId = procedureId;
            ProcedureName = procedureName;
            ProcedureDuration = procedureDuration;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public AppointmentJointModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}
