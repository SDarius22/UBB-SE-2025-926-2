﻿using Hospital.ApiClients;
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

        private readonly MedicalRecordsApiService _medicalRecordsDatabaseService;

        public MedicalRecordManager(MedicalRecordsApiService medicalRecordsDatabaseService)
        {
            _medicalRecordsDatabaseService = medicalRecordsDatabaseService;
            MedicalRecords = new List<MedicalRecordJointModel>();
        }

        public async Task LoadMedicalRecordsForPatient(int patientId)
        {
            try
            {
                List<MedicalRecordJointModel> medicalRecords = await _medicalRecordsDatabaseService
                    .GetMedicalRecordsForPatientAsync(patientId)
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
                return await _medicalRecordsDatabaseService.GetMedicalRecordByIdAsync(medicalRecordId);
            }
            catch (MedicalRecordNotFoundException)
            {
                throw new MedicalRecordNotFoundException("No medical record found for the given id.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading medical record: {exception.Message}");
                return null;
            }
        }

        public async Task<int> CreateMedicalRecord(MedicalRecordModel medicalRecord)
        {
            try
            {
                // Insert the new record into the database and get the generated ID.
                int newMedicalRecordId = await _medicalRecordsDatabaseService.AddMedicalRecordAsync(medicalRecord)
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
                    .GetMedicalRecordsForDoctorAsync(doctorId)
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
    }
}