using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Hospital.ViewModels;
using Hospital.Models;
using Hospital.Managers;
using Hospital.DatabaseServices;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Hospital.DbContext;
using Hospital.DatabaseServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Views
{
    public sealed partial class CreateMedicalRecordForm : Page
    {
        private MedicalRecordCreationFormViewModel _viewModel;
        private MedicalRecordManager _medicalRecordManager;

        public CreateMedicalRecordForm()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var idoctorsDatabaseService = App.Services.GetRequiredService<IDoctorsDatabaseService>();
            var imedicalProceduresDatabaseService = App.Services.GetRequiredService<IMedicalProceduresDatabaseService>();
            var idepartmentsDatabaseService = App.Services.GetRequiredService<IDepartmentsDatabaseService>();
            var imedicalRecordsDatabaseService = App.Services.GetRequiredService<IMedicalRecordsDatabaseService>();

            var doctorManager = new DoctorManager(idoctorsDatabaseService);
            var procedureManager = new MedicalProcedureManager(imedicalProceduresDatabaseService);
            var departmentManager = new DepartmentManager(idepartmentsDatabaseService);

            _medicalRecordManager = new MedicalRecordManager(imedicalRecordsDatabaseService);

            _viewModel = new MedicalRecordCreationFormViewModel(doctorManager, procedureManager, imedicalRecordsDatabaseService);

            await departmentManager.LoadDepartments();
            foreach (var d in departmentManager.GetDepartments())
                _viewModel.DepartmentsList.Add(d);

            this.rootGrid.DataContext = _viewModel;
        }

        //private async void UploadFiles_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new FileOpenPicker
        //    {
        //        ViewMode = PickerViewMode.Thumbnail,
        //        FileTypeFilter = { ".jpg", ".png", ".pdf", ".docx" },
        //    };

        //    var window = Microsoft.UI.Xaml.Window.GetWindowForXamlRoot(this.Content.XamlRoot);
        //    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        //    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        //    var files = await picker.PickMultipleFilesAsync();
        //    if (files is not null)
        //    {
        //        foreach (var file in files)
        //        {
        //            _viewModel.AddDocument(file.Path);
        //        }
        //    }
        //}

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.PatientId <= 0 ||
                    _viewModel.SelectedDoctor == null ||
                    _viewModel.SelectedProcedure == null ||
                    string.IsNullOrWhiteSpace(_viewModel.Conclusion) ||
                    _viewModel.AppointmentDate == null)
                {
                    await ShowErrorDialog("Please complete all fields before submitting.");
                    return;
                }

                if (_viewModel.SelectedProcedure == null)
                {
                    await ShowErrorDialog("Please select a procedure.");
                    return;
                }

                var newMedicalRecord = new MedicalRecordModel(
                    medicalRecordId: 0,
                    patientId: _viewModel.PatientId,
                    doctorId: _viewModel.SelectedDoctor.DoctorId,
                    procedureId: _viewModel.SelectedProcedure.ProcedureId,
                    conclusion: _viewModel.Conclusion,
                    dateAndTime: _viewModel.AppointmentDate
                );

                int createdId = await _medicalRecordManager.CreateMedicalRecord(newMedicalRecord);

                if (createdId > 0)
                {
                    await ShowSuccessDialog("Medical record created successfully!");
                    if (this.Frame.CanGoBack)
                        this.Frame.GoBack();
                }
                else
                {
                    await ShowErrorDialog("Failed to create medical record.");
                }
            }
            catch (ValidationException ex)
            {
                await ShowErrorDialog("Validation error: " + ex.Message);
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Unexpected error: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async Task ShowSuccessDialog(string message)
        {
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        private async Task ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await errorDialog.ShowAsync();
        }
    }
}