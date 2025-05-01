using Backend.Configs;
using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Backend.DbContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(ApplicationConfiguration.GetInstance().DatabaseConnection));
// Add services to the container.


builder.Services.AddScoped<IAppointmentsDatabaseService, AppointmentsDatabaseService>();
builder.Services.AddScoped<IDepartmentsDatabaseService, DepartmentsDatabaseService>();
builder.Services.AddScoped<IDoctorInformationDatabaseService, DoctorInformationDatabaseService>();
builder.Services.AddScoped<IDoctorsDatabaseService, DoctorsDatabaseService>();
builder.Services.AddScoped<IDocumentDatabaseService, DocumentDatabaseService>();
builder.Services.AddScoped<IDrugsDatabaseService, DrugsDatabaseService>();
builder.Services.AddScoped<IEquipmentDatabaseService, EquipmentDatabaseService>();
builder.Services.AddScoped<IMedicalProceduresDatabaseService, MedicalProceduresDatabaseService>();
builder.Services.AddScoped<IMedicalRecordsDatabaseService, MedicalRecordsDatabaseService>();
builder.Services.AddScoped<IRatingDatabaseService, RatingDatabaseService>();
builder.Services.AddScoped<IRoomDatabaseService, RoomDatabaseService>();
builder.Services.AddScoped<IScheduleDatabaseService, ScheduleDatabaseService>();
builder.Services.AddScoped<IShiftsDatabaseService, ShiftsDatabaseService>();
builder.Services.AddScoped<IUserDatabaseService, UserDatabaseService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
