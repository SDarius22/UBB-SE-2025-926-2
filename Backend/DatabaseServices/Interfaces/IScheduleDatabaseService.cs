using Backend.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DatabaseServices.Interfaces
{
    public interface IScheduleDatabaseService
    {
        public Task<bool> AddSchedule(ScheduleModel schedule);
        public Task<bool> UpdateSchedule(ScheduleModel schedule);
        public Task<bool> DeleteSchedule(int scheduleID);
        public Task<bool> DoesScheduleExist(int scheduleID);
        public Task<bool> DoesDoctorExist(int doctorID);
        public Task<bool> DoesShiftExist(int shiftID);
        public Task<List<ScheduleModel>> GetSchedules();
    }
}