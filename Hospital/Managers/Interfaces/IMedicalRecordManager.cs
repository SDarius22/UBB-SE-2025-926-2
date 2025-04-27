using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public interface IMedicalRecordManager
    {
        Task LoadMedicalRecordsForPatient(int patientId);
        Task<MedicalRecordJointModel> GetMedicalRecordById(int medicalRecordId);
        Task<int> CreateMedicalRecord(AppointmentJointModel detailedAppointment, string conclusion);
        Task LoadMedicalRecordsForDoctor(int doctorId);
        Task<List<MedicalRecordJointModel>> GetMedicalRecords();
        Task<int> CreateMedicalRecordWithAppointment(AppointmentJointModel appointment, string conclusion);
    }
}
