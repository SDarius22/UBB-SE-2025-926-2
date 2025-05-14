using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IMedicalRecordsApiService
{
    public Task<int> AddMedicalRecordAsync(MedicalRecordModel medicalRecord);

    public Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatientAsync(int patientId);

    public Task<MedicalRecordJointModel> GetMedicalRecordByIdAsync(int medicalRecordId);

    public Task<List<MedicalRecordJointModel>> GetMedicalRecordsForDoctorAsync(int doctorId);
}