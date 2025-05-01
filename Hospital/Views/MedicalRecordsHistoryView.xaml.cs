


using Hospital.ApiClients;
using Hospital.Managers;
using Hospital.Models;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Appointments;

namespace Hospital.Views
{
    /// <summary>
    /// A window where past medical records are listed for the logged in user.
    /// </summary>
    public sealed partial class MedicalRecordsHistoryView : Page
    {
        private MedicalRecordsHistoryViewModel _viewModel;
        public MedicalRecordsHistoryView()
        {
            this.InitializeComponent();
            var _medicalRecordDatabaseService = App.Services.GetRequiredService<MedicalRecordsApiService>();
            var _documentDatabaseService = App.Services.GetRequiredService<DocumentsApiService>();

            _viewModel = new MedicalRecordsHistoryViewModel(
                1, // This should be replaced with the actual logged-in patient ID
                new MedicalRecordManager(_medicalRecordDatabaseService),
                new DocumentManager(_documentDatabaseService, new FileService())
            );
            this.MedicalRecordsPanel.DataContext = _viewModel;
        }


        private async void ShowMedicalRecordDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRecord = this.MedicalRecordsListView.SelectedItem;
                if (selectedRecord is MedicalRecordJointModel medicalRecord)
                {
                    this._viewModel.ShowMedicalRecordDetails(medicalRecord);
                }
                else if (selectedRecord == null)
                {
                    var validationDialog = new ContentDialog
                    {
                        Title = "No element selected",
                        Content = "Please select a medical record to view its details.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot // Ensure the dialog is tied to the current view's XamlRoot
                    };
                    await validationDialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var validationDialog = new ContentDialog
                {
                    Title = "No element selected",
                    Content = "Please select a medical record to view its details.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot // Ensure the dialog is tied to the current view's XamlRoot
                };
                await validationDialog.ShowAsync();
                return;
            }
        }
    }
}