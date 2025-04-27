using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class AppointmentModel
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime DateAndTime { get; set; }
        public bool Finished { get; set; }
        public int ProcedureId { get; set; }

        public AppointmentModel(int appointmentId, int doctorId, int patientId, DateTime dateAndTime, bool finished, int procedureId)
        {
            AppointmentId = appointmentId;
            DoctorId = doctorId;
            PatientId = patientId;
            DateAndTime = dateAndTime;
            Finished = finished;
            ProcedureId = procedureId;
        }

        public AppointmentModel() { }

        public AppointmentModel(int appointmentId, int patientId, int doctorId, DateTime dateAndTime, bool finished)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            DoctorId = doctorId;
            DateAndTime = dateAndTime;
            Finished = finished;
        }
    }
}
