using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
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
            try
            {
                Doctors.Clear();
                List<DoctorJointModel> doctorsList = await _doctorDatabaseService.GetDoctorsByDepartment(departmentId).ConfigureAwait(false);
                foreach (DoctorJointModel doctor in doctorsList)
                {
                    Doctors.Add(doctor);
                }
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
