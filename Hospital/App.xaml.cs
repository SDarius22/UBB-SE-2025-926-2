namespace Hospital
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Hospital.ApiClients;
    using Hospital.ViewModel;
    using Hospital.ViewModels;
    using Hospital.ViewModels.AddViewModels;
    using Hospital.ViewModels.DeleteViewModels;
    using Hospital.ViewModels.UpdateViewModels;
    using Hospital.Views;
    using Hospital.Views.AddViews;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using Microsoft.UI.Xaml.Shapes;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        public static Window MainWindow { get; private set; }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AppointmentsApiService>();
            services.AddSingleton<DepartmentsApiService>();
            services.AddSingleton<DoctorApiService>();
            services.AddSingleton<DocumentsApiService>();
            services.AddSingleton<DrugsApiService>();
            services.AddSingleton<EquipmentApiService>();
            services.AddSingleton<MedicalProceduresApiService>();
            services.AddSingleton<MedicalRecordsApiService>();
            services.AddSingleton<RatingApiService>();
            services.AddSingleton<RoomApiService>();
            services.AddSingleton<ScheduleApiService>();
            services.AddSingleton<ShiftsApiService>();
            services.AddSingleton<UserApiService>();


            services.AddTransient<DepartmentAddViewModel>();
            services.AddTransient<DoctorAddViewModel>();
            services.AddTransient<DrugAddViewModel>();
            services.AddTransient<EquipmentAddViewModel>();
            services.AddTransient<RoomAddViewModel>();
            services.AddTransient<ScheduleAddViewModel>();
            services.AddTransient<ShiftAddViewModel>();

            services.AddTransient<DepartmentDeleteViewModel>();
            services.AddTransient<DoctorDeleteViewModel>();
            services.AddTransient<DrugDeleteViewModel>();
            services.AddTransient<EquipmentDeleteViewModel>();
            services.AddTransient<RoomDeleteViewModel>();
            services.AddTransient<ScheduleDeleteViewModel>();
            services.AddTransient<ShiftDeleteViewModel>();

            services.AddTransient<DepartmentUpdateViewModel>();
            services.AddTransient<DoctorUpdateViewModel>();
            services.AddTransient<DrugUpdateViewModel>();
            services.AddTransient<EquipmentUpdateViewModel>();
            services.AddTransient<RoomUpdateViewModel>();
            services.AddTransient<ScheduleUpdateViewModel>();
            services.AddTransient<ShiftUpdateViewModel>();

            services.AddTransient<AppointmentCreationFormViewModel>();
            services.AddTransient<AppointmentDetailsViewModel>();
            services.AddTransient<DoctorInformationViewModel>();
            services.AddTransient<DoctorScheduleViewModel>();
            services.AddTransient<MedicalRecordCreationFormViewModel>();
            services.AddTransient<MedicalRecordDetailsViewModel>();
            services.AddTransient<MedicalRecordsHistoryViewModel>();
            services.AddTransient<PatientScheduleViewModel>();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            MainWindow = new LoginPage();
            MainWindow.Activate();
        }
    }
}