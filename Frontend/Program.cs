//using Frontend.ApiClients.Interface;
//using Frontend.Configs;
//using Frontend.DbContext;
//using Hospital.ApiClients;
//using Microsoft.EntityFrameworkCore;
////using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

////builder.Services.AddDbContext<AppDbContext>(options =>
////    options.UseSqlServer(ApplicationConfiguration.GetInstance().DatabaseConnection));


//var connectionString = "Data Source=DIANA\\SQLEXPRESS;Initial Catalog=HospitalManagement;Integrated Security=True;TrustServerCertificate=True";

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString));


//builder.Services.AddScoped<IAppointmentsApiService, AppointmentsApiService>();
//builder.Services.AddScoped<IDepartmentsApiService, DepartmentsApiService>();
//builder.Services.AddScoped<IDoctorApiService, DoctorApiService>();
//builder.Services.AddScoped<IDocumentsApiService, DocumentsApiService>();
//builder.Services.AddScoped<IDrugsApiService, DrugsApiService>();
//builder.Services.AddScoped<IEquipmentApiService, EquipmentApiService>();
//builder.Services.AddScoped<IMedicalProceduresApiService, MedicalProceduresApiService>();
//builder.Services.AddScoped<IMedicalRecordsApiService, MedicalRecordsApiService>();
//builder.Services.AddScoped<IRatingApiService, RatingApiService>();
//builder.Services.AddScoped<IRoomApiService, RoomApiService>();
//builder.Services.AddScoped<IScheduleApiService, ScheduleApiService>();
//builder.Services.AddScoped<IShiftsApiService, ShiftsApiService>();
//builder.Services.AddScoped<IUserApiService, UserApiService>();


//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddSession();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddHttpClient();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseSession();
//app.UseAuthorization();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{id?}");

//app.Run();

//using Frontend.ApiClients.Interface;
//using Frontend.Configs;
//using Frontend.DbContext;
//using Hospital.ApiClients;
//using Microsoft.EntityFrameworkCore;

////using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

////builder.Services.AddDbContext<AppDbContext>(options =>
////    options.UseSqlServer(ApplicationConfiguration.GetInstance().DatabaseConnection));

//builder.Services.AddScoped<IAppointmentsApiService, AppointmentsApiService>();
//builder.Services.AddScoped<IDepartmentsApiService, DepartmentsApiService>();
//builder.Services.AddScoped<IDoctorApiService, DoctorApiService>();
//builder.Services.AddScoped<IDocumentsApiService, DocumentsApiService>();
//builder.Services.AddScoped<IDrugsApiService, DrugsApiService>();
//builder.Services.AddScoped<IEquipmentApiService, EquipmentApiService>();
//builder.Services.AddScoped<IMedicalProceduresApiService, MedicalProceduresApiService>();
//builder.Services.AddScoped<IMedicalRecordsApiService, MedicalRecordsApiService>();
//builder.Services.AddScoped<IRatingApiService, RatingApiService>();
//builder.Services.AddScoped<IRoomApiService, RoomApiService>();
//builder.Services.AddScoped<IScheduleApiService, ScheduleApiService>();
//builder.Services.AddScoped<IShiftsApiService, ShiftsApiService>();
//builder.Services.AddScoped<IUserApiService, UserApiService>();

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddSession();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddHttpClient();
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseSession();

//app.Use(async (context, next) =>
//{
//    if (context.Request.Query.ContainsKey("op"))
//    {
//        var operation = context.Request.Query["op"];
//        context.Session.SetString("CurrentCrudOp", operation);
//    }

//    await next();
//});

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{id?}");

//app.Run();



using Frontend.ApiClients.Interface;
using Frontend.Configs;
using Frontend.DbContext;
using Hospital.ApiClients;
using Microsoft.EntityFrameworkCore;

//using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(ApplicationConfiguration.GetInstance().DatabaseConnection));

builder.Services.AddScoped<IAppointmentsApiService, AppointmentsApiService>();
builder.Services.AddScoped<IDepartmentsApiService, DepartmentsApiService>();
builder.Services.AddScoped<IDoctorApiService, DoctorApiService>();
builder.Services.AddScoped<IDocumentsApiService, DocumentsApiService>();
builder.Services.AddScoped<IDrugsApiService, DrugsApiService>();
builder.Services.AddScoped<IEquipmentApiService, EquipmentApiService>();
builder.Services.AddScoped<IMedicalProceduresApiService, MedicalProceduresApiService>();
builder.Services.AddScoped<IMedicalRecordsApiService, MedicalRecordsApiService>();
builder.Services.AddScoped<IRatingApiService, RatingApiService>();
builder.Services.AddScoped<IRoomApiService, RoomApiService>();
builder.Services.AddScoped<IScheduleApiService, ScheduleApiService>();
builder.Services.AddScoped<IShiftsApiService, ShiftsApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.Use(async (context, next) =>
{
    if (context.Request.Query.ContainsKey("op"))
    {
        var operation = context.Request.Query["op"];
        context.Session.SetString("CurrentCrudOp", operation);
    }

    await next();
});



app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
