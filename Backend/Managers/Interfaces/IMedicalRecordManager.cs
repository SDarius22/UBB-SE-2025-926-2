using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Managers
{
    public interface IMedicalRecordManager
    {
        Task LoadMedicalRecordsForPatient(int patientId);
        Task<MedicalRecordJointModel> GetMedicalRecordById(int medicalRecordId);
        Task<int> CreateMedicalRecord(MedicalRecordModel medicalRecord);
        Task LoadMedicalRecordsForDoctor(int doctorId);
        Task<List<MedicalRecordJointModel>> GetMedicalRecords();
    }
}