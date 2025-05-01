using Backend.DatabaseServices;
using Backend.Exceptions;
using Backend.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Managers
{
    public class DoctorManager : IDoctorManager
    {
        public List<DoctorJointModel> Doctors { get; private set; }

        private IDoctorsDatabaseService _doctorDatabaseService;

        public DoctorManager(IDoctorsDatabaseService doctorDatabaseService)
        {
            _doctorDatabaseService = doctorDatabaseService;
            Doctors = new List<DoctorJointModel>();
        }

        public async Task LoadDoctors(int departmentId)
        {
            Debug.WriteLine("→ DoctorManager.LoadDoctors(" + departmentId + ")");
            try
            {
                Doctors.Clear();
                List<DoctorJointModel> doctorsList = await _doctorDatabaseService.GetDoctorsByDepartment(departmentId);
                foreach (DoctorJointModel doctor in doctorsList)
                {
                    Doctors.Add(doctor);
                }
                Debug.WriteLine("→ DoctorsDatabaseService.GetDoctorsByDepartment returned: " + doctorsList.Count);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading departments: {exception.Message}");
            }
        }

        public List<DoctorJointModel> GetDoctorsWithRatings()
        {
            return Doctors;
        }
    }
}