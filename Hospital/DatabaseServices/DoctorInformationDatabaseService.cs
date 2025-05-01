using Hospital.Configs;
using Hospital.DatabaseServices.Interfaces;
using Hospital.DbContext;
using Hospital.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DoctorInformationModel = Hospital.Models.DoctorInformationModel;

namespace Hospital.DatabaseServices
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

        // ... existing code ...
        public async Task<DoctorInformationModel> GetDoctorInformation(int doctorId)
        {
            try
            {
                var doctorInfo = await _context.DoctorJoints
                    .Join(
                        _context.Users,
                        d => d.UserId,
                        u => u.UserID,
                        (d, u) => new { Doctor = d, User = u }
                    )
                    .Join(
                        _context.Departments,
                        du => du.Doctor.DepartmentId,
                        dept => dept.DepartmentId,
                        (du, dept) => new DoctorInformationModel
                        {
                            UserID = du.User.UserID,
                            Username = du.User.Username,
                            Mail = du.User.Mail,
                            Role = du.User.Role,
                            Name = du.User.Name,
                            Birthdate = du.User.BirthDate.ToDateTime(TimeOnly.MinValue),
                            Cnp = du.User.Cnp,
                            Address = du.User.Address,
                            PhoneNumber = du.User.PhoneNumber,
                            RegistrationDate = du.User.RegistrationDate,
                            DoctorID = du.Doctor.DoctorId,
                            LicenseNumber = du.Doctor.LicenseNumber,
                            Rating = (float)du.Doctor.DoctorRating,
                            DepartmentID = dept.DepartmentId,
                            DepartmentName = dept.Name
                        }
                    )
                    .FirstOrDefaultAsync(d => d.DoctorID == doctorId);

                return doctorInfo ?? throw new DatabaseOperationException("Doctor not found");
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException(ex.Message);
            }
        }
        // ... existing code ...
    }
}