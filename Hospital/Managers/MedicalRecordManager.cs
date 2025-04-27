using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public class MedicalRecordManager : IMedicalRecordManager
    {
        public List<MedicalRecordJointModel> MedicalRecords { get; private set; }

        private readonly IMedicalRecordsDatabaseService _medicalRecordsDatabaseService;

        public MedicalRecordManager(IMedicalRecordsDatabaseService medicalRecordsDatabaseService)
        {
            _medicalRecordsDatabaseService = medicalRecordsDatabaseService;
            MedicalRecords = new List<MedicalRecordJointModel>();
        }

        public async Task LoadMedicalRecordsForPatient(int patientId)
        {
            try
            {
                List<MedicalRecordJointModel> medicalRecords = await _medicalRecordsDatabaseService
                    .GetMedicalRecordsForPatient(patientId)
                    .ConfigureAwait(false);
                MedicalRecords.Clear();
                if (medicalRecords == null)
                {
                    medicalRecords = new List<MedicalRecordJointModel>();
                }
                foreach (MedicalRecordJointModel medicalRecord in medicalRecords)
                {
                    MedicalRecords.Add(medicalRecord);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading medical records: {exception.Message}");
                return;
            }
        }


        public async Task<MedicalRecordJointModel> GetMedicalRecordById(int medicalRecordId) 
            {
            try 
            {
                return await _medicalRecordsDatabaseService.GetMedicalRecordById(medicalRecordId);
            } catch (MedicalRecordNotFoundException) 
            {
                throw new MedicalRecordNotFoundException("No medical record found for the given id.");
            } catch (Exception exception) 
            {
                Console.WriteLine($"Error loading medical record: {exception.Message}");
                return null;
            }
        }

        public async Task<int> CreateMedicalRecord(AppointmentJointModel detailedAppointment, string conclusion)
        {
            try
            {
                // Placeholder MedicalRecordId, will be replaced by DB-generated ID
                int placeholderMedicalRecordId = 0;

                // Create a new MedicalRecord instance with the provided conclusion.
                MedicalRecordModel medicalRecord = new MedicalRecordModel(
                    placeholderMedicalRecordId,
                    detailedAppointment.PatientId,
                    detailedAppointment.DoctorId,
                    detailedAppointment.ProcedureId,
                    conclusion,
                    DateTime.Now
                );

                // Insert the new record into the database and get the generated ID.
                int newMedicalRecordId = await _medicalRecordsDatabaseService.AddMedicalRecord(medicalRecord)
                                                          .ConfigureAwait(false);

                // If the record was successfully added, update the in-memory list.
                if (newMedicalRecordId > 0)
                {
                    medicalRecord.MedicalRecordId = newMedicalRecordId;

                    // Optionally, retrieve the full record from the database (with join data) and add it.
                    MedicalRecords.Add(await GetMedicalRecordById(newMedicalRecordId));
                }

                return newMedicalRecordId;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error creating medical record: {exception.Message}");
                return -1;
            }
        }


        public async Task LoadMedicalRecordsForDoctor(int doctorId)
        {
            try
            {
                List<MedicalRecordJointModel> medicalRecords = await _medicalRecordsDatabaseService
                    .GetMedicalRecordsForDoctor(doctorId)
                    .ConfigureAwait(false);
                MedicalRecords.Clear();
                foreach (MedicalRecordJointModel medicalRecord in medicalRecords)
                {
                    MedicalRecords.Add(medicalRecord);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading medical records: {exception.Message}");
                return;
            }
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecords()
        {
            return MedicalRecords;
        }

        public bool ValidateConclusion(string conclusion)
        {
            return !string.IsNullOrWhiteSpace(conclusion) && conclusion.Length <= 255;
        }

        public async Task<int> CreateMedicalRecordWithAppointment(AppointmentJointModel appointment, string conclusion)
        {
            if (!ValidateConclusion(conclusion))
            {
                throw new ValidationException("Conclusion must not be empty and cannot exceed 255 characters.");
            }

            // Update appointment status
            appointment.Finished = true;
            appointment.DateAndTime = DateTime.Now;

            // Create medical record
            return await CreateMedicalRecord(appointment, conclusion);
        }
    }
}
