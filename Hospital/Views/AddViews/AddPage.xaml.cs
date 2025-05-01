using System;
using Microsoft.UI.Xaml.Controls;
using Hospital.Managers;
using Hospital.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Hospital.ApiClients;

// To learn more about WinUI, the WinUI Hospital structure,
// and more about our Hospital templates, see: http://aka.ms/winui-Hospital-info.

namespace Hospital.Views.AddViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {

        private readonly DepartmentManager _departmentManager;
        private readonly MedicalProcedureManager _procedureManager;
        private readonly DoctorManager _doctorManager;
        private readonly ShiftManager _shiftManager;
        private readonly AppointmentManager _appointmentManager;


        public AddPage()
        {
            this.InitializeComponent();

            var departmentsDatabaseService = App.Services.GetRequiredService<DepartmentsApiService>();
            var proceduresDatabaseService = App.Services.GetRequiredService<MedicalProceduresApiService>();
            var doctorsDatabaseService = App.Services.GetRequiredService<DoctorApiService>();
            var shiftsDatabaseService = App.Services.GetRequiredService<ShiftsApiService>();
            var appointmentsDatabaseService = App.Services.GetRequiredService<AppointmentsApiService>();

            _departmentManager = new DepartmentManager(departmentsDatabaseService);
            _procedureManager = new MedicalProcedureManager(proceduresDatabaseService);
            _doctorManager = new DoctorManager(doctorsDatabaseService);
            _shiftManager = new ShiftManager(shiftsDatabaseService);
            _appointmentManager = new AppointmentManager(appointmentsDatabaseService);
            ContentFrame.Navigate(typeof(AddDoctorView));
        }

        private async void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                string invokedItemName = args.SelectedItemContainer.Tag.ToString();
                switch (invokedItemName)
                {
                    case "Doctors":
                        ContentFrame.Navigate(typeof(AddDoctorView));
                        break;
                    case "Drugs":
                        ContentFrame.Navigate(typeof(AddDrugView));
                        break;
                    case "Equipments":
                        ContentFrame.Navigate(typeof(AddEquipmentView));
                        break;
                    case "Rooms":
                        ContentFrame.Navigate(typeof(AddRoomAndDepartments));
                        break;
                    case "Schedules":
                        ContentFrame.Navigate(typeof(AddScheduleAndShifts));
                        break;
                    case "Shifts":
                        ContentFrame.Navigate(typeof(AddShiftView));
                        break;
                    case "Patients":
                        {
                            try
                            {
                                var viewModel = await AppointmentCreationFormViewModel.CreateViewModel(
                                    _departmentManager,
                                    _procedureManager,
                                    _doctorManager,
                                    _shiftManager,
                                    _appointmentManager);

                                ContentFrame.Navigate(typeof(AppointmentCreationForm), viewModel);
                            }
                            catch (Exception ex)
                            {
                                ContentDialog contentDialog = new ContentDialog
                                {
                                    Title = "Error",
                                    Content = ex.Message,
                                    CloseButtonText = "Ok",
                                    XamlRoot = this.Content.XamlRoot
                                };
                                await contentDialog.ShowAsync();
                            }

                            break;
                        }

                    case "MedicalRecords":
                        ContentFrame.Navigate(typeof(CreateMedicalRecordForm));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}