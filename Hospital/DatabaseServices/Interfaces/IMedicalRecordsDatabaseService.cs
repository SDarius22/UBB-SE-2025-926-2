using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IMedicalRecordsDatabaseService
    {
        Task<int> AddMedicalRecord(MedicalRecordModel medicalRecord);

        Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatient(int patientId);

        Task<MedicalRecordJointModel> GetMedicalRecordById(int medicalRecordId);

        Task<List<MedicalRecordJointModel>> GetMedicalRecordsForDoctor(int doctorId);
    }
}
