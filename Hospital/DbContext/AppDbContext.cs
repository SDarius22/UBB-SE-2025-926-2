using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<AdminModel> Admins { get; set; }
    public DbSet<AppointmentJointModel> AppointmentJoints { get; set; }
    public DbSet<AppointmentModel> Appointments { get; set; }
    public DbSet<DepartmentModel> Departments { get; set; }
    public DbSet<DoctorInformationModel> DoctorInformations { get; set; }
    public DbSet<DoctorJointModel> DoctorJoints { get; set; }
    public DbSet<DocumentModel> Documents { get; set; }
    public DbSet<DrugModel> Drugs { get; set; }
    public DbSet<EquipmentModel> Equipments { get; set; }
    public DbSet<MedicalRecordJointModel> MedicalRecordJoints{ get; set; }
    public DbSet<MedicalRecordModel> MedicalRecords { get; set; }
    public DbSet<PatientJointModel> PatientJoints { get; set; }
    public DbSet<ProcedureModel> Procedures { get; set; }
    public DbSet<RatingModel> Ratings { get; set; }
    public DbSet<RoomModel> Rooms { get; set; }
    public DbSet<ScheduleModel> Schedules { get; set; }
    public DbSet<ShiftModel> Shifts { get; set; }
    public DbSet<TimeSlotModel> TimeSlots { get; set; }
    public DbSet<UserModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure your UserModel entity if needed
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.Role).IsRequired();
        });
    }

}