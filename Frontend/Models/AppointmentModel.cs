using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class AppointmentModel
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime DateAndTime { get; set; }
        public bool Finished { get; set; }
        public int ProcedureId { get; set; }

        public virtual DoctorJointModel? Doctor { get; set; }

        public virtual PatientJointModel? Patient { get; set; }

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

    public static class AppoinmentModelExtensions
    {
        public static AppointmentModel ToModel(this AppointmentJointModel appointment)
        {
            return new AppointmentModel
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                DateAndTime = appointment.DateAndTime,
                Finished = appointment.Finished
            };
        }
    }
}
