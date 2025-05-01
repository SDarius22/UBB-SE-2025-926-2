using Backend.Configs;
using Backend.DbContext;
using Backend.Exceptions;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DatabaseServices
{
    public class DepartmentsDatabaseService : IDepartmentsDatabaseService
    {
        private readonly AppDbContext _context;

        public DepartmentsDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        // This method will be used to get the departments from the database
        public virtual async Task<List<DepartmentModel>> GetDepartmentsFromDataBase()
        {
            try
            {
                return await _context.Departments
                    .Select(d => new DepartmentModel(
                            d.DepartmentID,
                            d.Name
                        )).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new DepartmentNotFoundException($"Error loading departments: {exception.Message}");
            }
        }

        public async Task<bool> AddDepartment(DepartmentModel department)
        {
            try
            {
                var entity = new DepartmentModel(department.DepartmentID, department.Name);

                await _context.Departments.AddAsync(entity);
                await _context.SaveChangesAsync();

                department.DepartmentID = entity.DepartmentID;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates an existing department in the database.
        /// </summary>
        /// <param name="department">The department to update.</param>
        /// <returns>True if the department was updated successfully, otherwise false.</returns>
        public async Task<bool> UpdateDepartment(DepartmentModel department)
        {
            try
            {
                var existingDepartment = await _context.Departments.FindAsync(department.DepartmentID);
                if (existingDepartment == null)
                {
                    return false;
                }

                existingDepartment.DepartmentID = department.DepartmentID;
                existingDepartment.Name = department.Name;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a department from the database.
        /// </summary>
        /// <param name="departmentID">The ID of the department to delete.</param>
        /// <returns>True if the department was deleted successfully, otherwise false.</returns>
        public async Task<bool> DeleteDepartment(int departmentID)
        {
            try
            {
                var department = await _context.Departments.FindAsync(departmentID);
                if (department == null) return false;

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a department exists in the database.
        /// </summary>
        /// <param name="departmentID">The ID of the department to check.</param>
        /// <returns>True if the department exists, otherwise false.</returns>
        public async Task<bool> DoesDepartmentExist(int departmentID)
        {
            return await _context.Departments.AnyAsync(d => d.DepartmentID == departmentID);
        }

        /// <summary>
        /// Retrieves all departments from the database.
        /// </summary>
        /// <returns>A list of departments.</returns>
        public async Task<List<DepartmentModel>> GetDepartments()
        {
            try
            {
                //var list = await _context.Procedures
                //    .Select(p => new ProcedureModel(
                //        p.ProcedureId,
                //        p.DepartmentId,
                //        p.ProcedureName,
                //        p.ProcedureDuration)).ToListAsync();
                //for (int i = 0; i < list.Count; i++)
                //{
                //    Debug.WriteLine($"Procedure ID: {list[i].ProcedureId}, Department ID: {list[i].DepartmentId}, Procedure Name: {list[i].ProcedureName}, Procedure Duration: {list[i].ProcedureDuration}");
                //}
                return await _context.Departments
                    .Select(d => new DepartmentModel(
                        d.DepartmentID,
                        d.Name
                    )).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new DepartmentNotFoundException($"Error loading departments: {exception.Message}");
            }
        }
    }
}