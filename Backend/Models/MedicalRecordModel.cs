﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class MedicalRecordModel
    {
        public int MedicalRecordId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public DateTime DateAndTime { get; set; }

        public int ProcedureId { get; set; }

        public string Conclusion { get; set; }

        public virtual PatientJointModel? Patient { get; set; }
        public virtual DoctorJointModel? Doctor { get; set; }
        public virtual ProcedureModel? Procedure { get; set; }

        public MedicalRecordModel()
        {
        }


        public MedicalRecordModel(int medicalRecordId, int patientId, int doctorId, int procedureId, string conclusion, DateTime dateAndTime)
        {
            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            DoctorId = doctorId;
            ProcedureId = procedureId;
            Conclusion = conclusion;
            DateAndTime = dateAndTime;
        }
    }
}
