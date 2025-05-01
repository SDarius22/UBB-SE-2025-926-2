//using Hospital.Configs;
using Hospital.DbContext;
using Hospital.Exceptions;
using Microsoft.EntityFrameworkCore;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Hospital.DatabaseServices.Interfaces;
using Microsoft.VisualBasic;
using System.Linq;

namespace Hospital.DatabaseServices
{
    public class MedicalRecordsDatabaseService : IMedicalRecordsDatabaseService
    {
        private readonly AppDbContext _context;

        public MedicalRecordsDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddMedicalRecord(MedicalRecordModel medicalRecord)
        {
            try
            {
                medicalRecord.DateAndTime = DateTime.Now;

                var entity = new MedicalRecordModel(medicalRecord.MedicalRecordId, medicalRecord.PatientId, medicalRecord.DoctorId, medicalRecord.ProcedureId, medicalRecord.Conclusion, medicalRecord.DateAndTime);

                await _context.MedicalRecords.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Update the model with generated ID if needed
                medicalRecord.MedicalRecordId = entity.MedicalRecordId;
                return entity.MedicalRecordId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding medical record: {ex.Message}");
                return -1;
            }
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatient(int patientId)
        {
            try
            {
                var records = await _context.MedicalRecords
                    .Where(mr => mr.PatientId == patientId)
                    .Join(_context.Users, mr => mr.PatientId, p => p.UserId, (mr, p) => new { mr, PatientName = p.Name })
                    .Join(_context.Users, temp => temp.mr.DoctorId, d => d.UserId, (temp, d) => new { temp.mr, temp.PatientName, DoctorName = d.Name })
                    .Join(_context.Procedures, temp => temp.mr.ProcedureId, pr => pr.ProcedureId, (temp, pr) => new { temp, pr, pr.DepartmentId, ProcedureName = pr.ProcedureName })
                    .Join(_context.Departments, temp => temp.DepartmentId, dept => dept.DepartmentId, (temp, dept) => new MedicalRecordJointModel(
                        temp.temp.mr.MedicalRecordId,
                        temp.temp.mr.PatientId,
                        temp.temp.PatientName,
                        temp.temp.mr.DoctorId,
                        temp.temp.DoctorName,
                        dept.DepartmentId,
                        dept.DepartmentName,
                        temp.pr.ProcedureId,
                        temp.ProcedureName,
                        temp.temp.mr.DateAndTime,
                        temp.temp.mr.Conclusion))
                    .ToListAsync();

                if (!records.Any())
                {
                    throw new MedicalRecordNotFoundException("No medical records found for the given patient.");
                }

                return records;
            }
            catch (Exception ex)
            {
                throw new MedicalRecordNotFoundException($"Error fetching records: {ex.Message}");
            }

        }

        public async Task<MedicalRecordJointModel> GetMedicalRecordById(int medicalRecordId)
        {
            try
            {
                var record = await _context.MedicalRecords
                    .Where(mr => mr.MedicalRecordId == medicalRecordId)
                    .Join(_context.Users, mr => mr.PatientId, p => p.UserId, (mr, p) => new { mr, PatientName = p.Name })
                    .Join(_context.Users, temp => temp.mr.DoctorId, d => d.UserId, (temp, d) => new { temp.mr, temp.PatientName, DoctorName = d.Name })
                    .Join(_context.Procedures, temp => temp.mr.ProcedureId, pr => pr.ProcedureId, (temp, pr) => new { temp, pr, pr.DepartmentId, ProcedureName = pr.ProcedureName })
                    .Join(_context.Departments, temp => temp.DepartmentId, dept => dept.DepartmentId, (temp, dept) => new MedicalRecordJointModel(
                        temp.temp.mr.MedicalRecordId,
                        temp.temp.mr.PatientId,
                        temp.temp.PatientName,
                        temp.temp.mr.DoctorId,
                        temp.temp.DoctorName,
                        dept.DepartmentId,
                        dept.DepartmentName,
                        temp.pr.ProcedureId,
                        temp.ProcedureName,
                        temp.temp.mr.DateAndTime,
                        temp.temp.mr.Conclusion))
                    .FirstOrDefaultAsync();

                if (record == null)
                {
                    throw new MedicalRecordNotFoundException("No medical record found for the given ID.");
                }

                return record;
            }
            catch (Exception ex)
            {
                throw new MedicalRecordNotFoundException($"Error fetching record: {ex.Message}");
            }
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForDoctor(int doctorId)
        {
            try
            {
                var records = await _context.MedicalRecords
                    .Where(mr => mr.DoctorId == doctorId)
                    .Join(_context.Users, mr => mr.PatientId, p => p.UserId, (mr, p) => new { mr, PatientName = p.Name })
                    .Join(_context.Users, temp => temp.mr.DoctorId, d => d.UserId, (temp, d) => new { temp.mr, temp.PatientName, DoctorName = d.Name })
                    .Join(_context.Procedures, temp => temp.mr.ProcedureId, pr => pr.ProcedureId, (temp, pr) => new { temp, pr, pr.DepartmentId, ProcedureName = pr.ProcedureName })
                    .Join(_context.Departments, temp => temp.DepartmentId, dept => dept.DepartmentId, (temp, dept) => new MedicalRecordJointModel(
                        temp.temp.mr.MedicalRecordId,
                        temp.temp.mr.PatientId,
                        temp.temp.PatientName,
                        temp.temp.mr.DoctorId,
                        temp.temp.DoctorName,
                        dept.DepartmentId,
                        dept.DepartmentName,
                        temp.pr.ProcedureId,
                        temp.ProcedureName,
                        temp.temp.mr.DateAndTime,
                        temp.temp.mr.Conclusion))
                    .ToListAsync();

                if (!records.Any())
                {
                    throw new MedicalRecordNotFoundException("No medical records found for the given doctor.");
                }

                return records;
            }
            catch (Exception ex)
            {
                throw new MedicalRecordNotFoundException($"Error fetching records: {ex.Message}");
            }
        }
    }
}