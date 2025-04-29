using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IScheduleDatabaseService
    {
        public bool AddSchedule(ScheduleModel schedule);
        public bool UpdateSchedule(ScheduleModel schedule);
        public bool DeleteSchedule(int scheduleID);
        public bool DoesScheduleExist(int scheduleID);
        public bool DoesDoctorExist(int doctorID);
        public bool DoesShiftExist(int shiftID);
        public List<ScheduleModel> GetSchedules();
    }
}
