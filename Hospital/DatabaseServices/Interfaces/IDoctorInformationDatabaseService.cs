using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IDoctorInformationDatabaseService
    {
        public Task<DoctorInformationModel> GetDoctorInformation(int doctorId);
    }
}