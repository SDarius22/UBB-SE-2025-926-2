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
using Hospital.Managers;
using Hospital.DatabaseServices;
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

        private readonly DepartmentManager _departmentManager;
        private readonly MedicalProcedureManager _procedureManager;
        private readonly DoctorManager _doctorManager;
        private readonly ShiftManager _shiftManager;
        private readonly AppointmentManager _appointmentManager;

        public AddPage()
        {
            this.InitializeComponent();

            var departmentsDatabaseService = new DepartmentsDatabaseService();
            var proceduresDatabaseService = new MedicalProceduresDatabaseService();
            var doctorsDatabaseService = new DoctorsDatabaseService();
            var shiftsDatabaseService = new ShiftsDatabaseService();
            var appointmentsDatabaseService = new AppointmentsDatabaseService();

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