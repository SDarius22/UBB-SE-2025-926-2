using Backend.Configs;
using Backend.DatabaseServices.Interfaces;
using Backend.DbContext;
using Backend.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DoctorInformationModel = Backend.Models.DoctorInformationModel;

namespace Backend.DatabaseServices
{
    public class DoctorInformationDatabaseService : IDoctorInformationDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;
        private readonly AppDbContext _context;

        public DoctorInformationDatabaseService(AppDbContext context)
        {
            this._configuration = ApplicationConfiguration.GetInstance();
            _context = context;
        }

        public async Task<DoctorInformationModel> GetDoctorInformation(int doctorId)
        {
            try
            {
                var query = FormattableStringFactory.Create(@"
                    SELECT 
                        UserID, Username, Mail, Role, Name, Birthdate, Cnp, Address, 
                        PhoneNumber, RegistrationDate, DoctorID, LicenseNumber, 
                        Experience, Rating, DepartmentID, Name as DepartmentName
                    FROM UserDoctorDepartmentView
                    WHERE DoctorID = {0}",
                    doctorId);

                var doctorInfo = await Task.Run(() =>
                    _context.Database.SqlQuery<DoctorInformationModel>(query).FirstOrDefault());

                return doctorInfo ?? throw new DatabaseOperationException("Doctor not found");
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException($"General Error: {exception.Message}");
            }
        }
    }
}