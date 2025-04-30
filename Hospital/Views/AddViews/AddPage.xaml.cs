using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Hospital.Views.AddViews;
using Hospital.Views;
using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.ViewModels;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI Hospital structure,
// and more about our Hospital templates, see: http://aka.ms/winui-Hospital-info.

namespace Hospital.Views.AddViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        private DepartmentManager _departmentManager;
        private MedicalProcedureManager _procedureManager;
        private DoctorManager _doctorManager;
        private ShiftManager _shiftManager;
        private Managers.AppointmentManager _appointmentManager;

        public AddPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(AddDoctorView));
            SetupManagers();
        }

        private void SetupManagers()
        {
            var departmentService = new DepartmentsDatabaseService();
            var procedureService = new MedicalProceduresDatabaseService();
            var doctorService = new DoctorsDatabaseService();
            var shiftService = new ShiftsDatabaseService();
            var appointmentService = new AppointmentsDatabaseService();

            _departmentManager = new DepartmentManager(departmentService);
            _procedureManager = new MedicalProcedureManager(procedureService);
            _doctorManager = new DoctorManager(doctorService);
            _shiftManager = new ShiftManager(shiftService);
            _appointmentManager = new Managers.AppointmentManager(appointmentService);
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
                    case "Patient":
                        try
                        {
                            // First create the ViewModel
                            var viewModel = await AppointmentCreationFormViewModel.CreateViewModel(
                                _departmentManager,
                                _procedureManager,
                                _doctorManager,
                                _shiftManager,
                                _appointmentManager);

                            // Then create the form with the ViewModel
                            AppointmentCreationForm appointmentCreationForm = await AppointmentCreationForm.CreateAppointmentCreationForm(viewModel);
                            appointmentCreationForm.Activate();
                        }
                        catch (Exception ex)
                        {
                            ContentDialog contentDialog = new ContentDialog
                            {
                                Title = "Error",
                                Content = ex.Message,
                                CloseButtonText = "Ok",
                            };
                            contentDialog.XamlRoot = this.Content.XamlRoot;
                            await contentDialog.ShowAsync();
                        }
                        break;
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
