using Backend.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DatabaseServices.Interfaces
{
    public interface IDoctorInformationDatabaseService
    {
        public Task<DoctorInformationModel> GetDoctorInformation(int doctorId);
    }
}