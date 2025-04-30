using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    private DbSet<AdminModel> Admins { get; set; }
    private DbSet<AppointmentJointModel> AppointmentJoints { get; set; }
    private DbSet<AppointmentModel> Appointments { get; set; }
    private DbSet<DepartmentModel> Departments { get; set; }
    private DbSet<DoctorInformationModel> DoctorInformations { get; set; }
    private DbSet<DoctorJointModel> DoctorJoints { get; set; }
    private DbSet<DocumentModel> Documents { get; set; }
    private DbSet<DrugModel> Drugs { get; set; }
    private DbSet<EquipmentModel> Equipments { get; set; }
    private DbSet<MedicalRecordJointModel> MedicalRecordJoints{ get; set; }
    private DbSet<MedicalRecordModel> MedicalRecords { get; set; }
    private DbSet<PatientJointModel> PatientJoints { get; set; }
    private DbSet<ProcedureModel> Procedures { get; set; }
    private DbSet<RatingModel> Ratings { get; set; }
    private DbSet<RoomModel> Rooms { get; set; }
    private DbSet<ScheduleModel> Schedules { get; set; }
    private DbSet<ShiftModel> Shifts { get; set; }
    private DbSet<TimeSlotModel> TimeSlots { get; set; }
    private DbSet<UserModel> Users { get; set; }

}