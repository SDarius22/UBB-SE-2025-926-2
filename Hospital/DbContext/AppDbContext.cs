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
    public DbSet<AppointmentModel> Appointments { get; set; }
    public DbSet<DepartmentModel> Departments { get; set; }
    public DbSet<DoctorJointModel> DoctorJoints { get; set; }
    public DbSet<DocumentModel> Documents { get; set; }
    public DbSet<DrugModel> Drugs { get; set; }
    public DbSet<EquipmentModel> Equipments { get; set; }
    public DbSet<MedicalRecordModel> MedicalRecords { get; set; }
    public DbSet<PatientJointModel> PatientJoints { get; set; }
    public DbSet<ProcedureModel> Procedures { get; set; }
    public DbSet<RatingModel> Ratings { get; set; }
    public DbSet<RoomModel> Rooms { get; set; }
    public DbSet<ScheduleModel> Schedules { get; set; }
    public DbSet<ShiftModel> Shifts { get; set; }
    public DbSet<UserModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User Model Configuration
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.UserID);
            entity.Property(u => u.Role).IsRequired();
        });

        // Admin Model Configuration
        modelBuilder.Entity<AdminModel>(entity =>
        {
            entity.ToTable("Admins");
            entity.HasKey(a => a.AdminId);
            entity.HasOne(a => a.User)
                  .WithMany()
                  .HasForeignKey(a => a.UserId);
        });

        // DoctorJoint Model Configuration
        modelBuilder.Entity<DoctorJointModel>(entity =>
        {
            entity.ToTable("Doctors");
            entity.HasKey(d => d.DoctorId);
            entity.HasOne(d => d.User)
                  .WithMany()
                  .HasForeignKey(d => d.UserId);
            entity.HasOne(d => d.Department)
                  .WithMany()
                  .HasForeignKey(d => d.DepartmentId);
        });

        // PatientJoint Model Configuration
        modelBuilder.Entity<PatientJointModel>(entity =>
        {
            entity.ToTable("Patients");
            entity.HasKey(p => p.PatientId);
            entity.HasOne(p => p.User)
                  .WithMany()
                  .HasForeignKey(p => p.UserId);
        });

        // Department Model Configuration
        modelBuilder.Entity<DepartmentModel>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(d => d.DepartmentId);
        });

        // Appointment Model Configuration
        modelBuilder.Entity<AppointmentModel>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(a => a.AppointmentId);
            entity.HasOne(a => a.Doctor)
                  .WithMany()
                  .HasForeignKey(a => a.DoctorId);
            entity.HasOne(a => a.Patient)
                  .WithMany()
                  .HasForeignKey(a => a.PatientId);
        });

        // Medical Record Configuration
        modelBuilder.Entity<MedicalRecordModel>(entity =>
        {
            entity.ToTable("MedicalRecords");
            entity.HasKey(m => m.MedicalRecordId);
            entity.HasOne(m => m.Patient)
                  .WithMany()
                  .HasForeignKey(m => m.PatientId);
            entity.HasOne(m => m.Doctor)
                  .WithMany()
                  .HasForeignKey(m => m.DoctorId);
        });

        // Room Model Configuration
        modelBuilder.Entity<RoomModel>(entity =>
        {
            entity.ToTable("Rooms");
            entity.HasKey(r => r.RoomID);
            entity.HasOne(r => r.Department)
                  .WithMany()
                  .HasForeignKey(r => r.DepartmentID);
        });

        // Schedule Model Configuration
        modelBuilder.Entity<ScheduleModel>(entity =>
        {
            entity.ToTable("Schedules");
            entity.HasKey(s => s.ScheduleId);
            entity.HasOne(s => s.Doctor)
                  .WithMany()
                  .HasForeignKey(s => s.DoctorId);
            entity.HasOne(s => s.Shift)
                  .WithMany()
                  .HasForeignKey(s => s.ShiftId);
        });

        // Shift Model Configuration
        modelBuilder.Entity<ShiftModel>(entity =>
        {
            entity.ToTable("Shifts");
            entity.HasKey(s => s.ShiftID);
        });

        // Other model configurations...
        modelBuilder.Entity<DocumentModel>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.DocumentId);
            entity.HasOne(d => d.MedicalRecord)
                  .WithMany()
                  .HasForeignKey(d => d.MedicalRecordId);
        });

        modelBuilder.Entity<DrugModel>(entity =>
        {
            entity.ToTable("Drugs");
            entity.HasKey(d => d.DrugID);
        });

        modelBuilder.Entity<EquipmentModel>(entity =>
        {
            entity.ToTable("Equipments");
            entity.HasKey(e => e.EquipmentID);
        });

        modelBuilder.Entity<ProcedureModel>(entity =>
        {
            entity.ToTable("Procedures");
            entity.HasKey(p => p.ProcedureId);
        });

        modelBuilder.Entity<RatingModel>(entity =>
        {
            entity.ToTable("Reviews");
            entity.HasKey(r => r.RatingId);
            entity.HasOne(r => r.MedicalRecord)
                  .WithMany()
                  .HasForeignKey(r => r.MedicalRecordId);
        });
    }

}