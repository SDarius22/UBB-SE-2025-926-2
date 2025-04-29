using System;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.ViewModels;
using Hospital.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.UI.Xaml.Navigation;
using Hospital.Managers;
using Hospital.DatabaseServices;

namespace Hospital.Views
{
    public sealed partial class CreateMedicalRecordForm : Page
    {

        private MedicalRecordCreationFormViewModel _viewModel;
        private AppointmentJointModel _appointment;
        private MedicalRecordManager _medicalRecordManager;

        public CreateMedicalRecordForm()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewModel = new MedicalRecordCreationFormViewModel();
            _medicalRecordManager = new MedicalRecordManager(new MedicalRecordsDatabaseService());
            this.rootGrid.DataContext = _viewModel;
        }

        private async void UploadFiles_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".jpg", ".png", ".pdf", ".docx" },
            };

            // Get the window's HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Initialize the picker with the window handle
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var files = await picker.PickMultipleFilesAsync();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {

                    _viewModel.AddDocument(file.Path);
                }
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newMedicalRecord = new MedicalRecordModel(
                    medicalRecordId: 0,
                    patientId: 1,
                    doctorId: 1,
                    procedureId: 1,
                    conclusion: _viewModel.Conclusion,
                    dateAndTime: _viewModel.AppointmentDate
                );

                int createdRecordId = await _medicalRecordManager.CreateMedicalRecord(newMedicalRecord);

                if (createdRecordId > 0)
                {
                    await ShowSuccessDialog("Medical record created successfully!");
                }
                else
                {
                    await ShowErrorDialog("Failed to create medical record.");
                }
            }
            catch (ValidationException ex)
            {
                await ShowErrorDialog(ex.Message);
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("An unexpected error occurred: " + ex.Message);
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
            };
            await errorDialog.ShowAsync();
        }
    }
}
