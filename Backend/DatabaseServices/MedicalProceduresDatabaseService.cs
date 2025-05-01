//using Backend.Configs;
using Backend.DbContext;
using Backend.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DatabaseServices.Interfaces;
using Backend.Exceptions;

namespace Backend.DatabaseServices
{
    public class MedicalProceduresDatabaseService : IMedicalProceduresDatabaseService
    {
        private readonly AppDbContext _context;

        public MedicalProceduresDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        // This method will be used to get the procedures from the database
        public async Task<List<ProcedureModel>> GetProceduresByDepartmentId(int departmentId)
        {
            if (departmentId <= 0)
            {
                throw new ShiftNotFoundException("Invalid department ID");
            }

            try
            {
                return await _context.Procedures
                    .Where(procedure => procedure.DepartmentId == departmentId)
                    .ToListAsync();
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading procedures for departmentID {departmentId}: {exception.Message}");
            }
        }
    }
}
