using System;
using System.Diagnostics;

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

using Hospital.Views;
using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.ViewModels;
using Windows.ApplicationModel.Appointments;
using Hospital.Exceptions;
using Hospital.Models;
using Hospital.Services;

namespace Hospital
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        //DB Services
        private DepartmentsDatabaseService? departmentService;
        private MedicalProceduresDatabaseService? procedureService;
        private DoctorsDatabaseService? doctorService;
        private ShiftsDatabaseService? shiftService;
        private AppointmentsDatabaseService? appointmentService;
        private MedicalRecordsDatabaseService? medicalRecordsDatabaseService;
        private DocumentDatabaseService? documentService;

        //Services
        private IFileService? fileService;

        //ManagerModels 
        private DepartmentManager? DepartmentManager;
        private MedicalProcedureManager? ProcedureManager;
        private DoctorManager? DoctorManager;
        private ShiftManager? ShiftManager;
        private Managers.AppointmentManager? AppointmentManager;
        private MedicalRecordManager? MedicalRecordManager;
        private DocumentManager? DocumentManager;


        public MainWindow()
        {
            this.InitializeComponent();
            this.SetupDatabaseServices();
        }

        private async void Patient1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // First create the ViewModel
                var viewModel = await AppointmentCreationFormViewModel.CreateViewModel(
                    DepartmentManager, 
                    ProcedureManager, 
                    DoctorManager, 
                    ShiftManager, 
                    AppointmentManager);

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
                    CloseButtonText = "Ok"
                };
                contentDialog.XamlRoot = this.Content.XamlRoot;
                await contentDialog.ShowAsync();
            }
        }


        private void Patient3_Click(object sender, RoutedEventArgs e)
        {
            int mockPatientId = 1;
            MedicalRecordsHistoryView medicalRecordsHistoryView = new MedicalRecordsHistoryView(mockPatientId, MedicalRecordManager, DocumentManager);
            medicalRecordsHistoryView.Activate();
        }


        private void DoctorScheduleButton(object sender, RoutedEventArgs e)
        {
            DoctorScheduleView doctorScheduleView = new DoctorScheduleView(AppointmentManager, ShiftManager, MedicalRecordManager, DocumentManager);
            doctorScheduleView.Activate();
        }

        private void Doctor2_Click(object sender, RoutedEventArgs e)
        {
            if (MedicalRecordManager == null || DocumentManager == null)
                return;

            // Create ViewModel with required dependencies
            MedicalRecordCreationFormViewModel viewModel = new MedicalRecordCreationFormViewModel(MedicalRecordManager, DocumentManager);

            // Create a mock appointment for testing
            AppointmentJointModel mockAppointment = new AppointmentJointModel(
                appointmentId: 1,
                finished: false,
                dateAndTime: DateTime.Now,
                departmentId: 1,
                departmentName: "Cardiology",
                doctorId: 1,
                doctorName: "Dr. Jane Doe",
                patientId: 1,
                patientName: "John Doe",
                procedureId: 1,
                procedureName: "Heart Checkup",
                procedureDuration: TimeSpan.FromMinutes(30)
            );

            // Create and show the MedicalRecordCreateView window
            CreateMedicalRecordForm medicalRecordCreateView = new CreateMedicalRecordForm(viewModel, mockAppointment);
            medicalRecordCreateView.Activate();
        }


        private void SetupDatabaseServices()
        {
            //setup database services here
            departmentService = new DepartmentsDatabaseService();
            procedureService = new MedicalProceduresDatabaseService();
            doctorService = new DoctorsDatabaseService();
            shiftService = new ShiftsDatabaseService();
            appointmentService = new AppointmentsDatabaseService();
            medicalRecordsDatabaseService = new MedicalRecordsDatabaseService();
            documentService = new DocumentDatabaseService();

            //setup services here
            fileService = new FileService();

            //setup manager models here
            DepartmentManager = new DepartmentManager(departmentService);
            ProcedureManager = new MedicalProcedureManager(procedureService);
            DoctorManager = new DoctorManager(doctorService);
            ShiftManager = new ShiftManager(shiftService);
            AppointmentManager = new Managers.AppointmentManager(appointmentService);
            MedicalRecordManager = new MedicalRecordManager(medicalRecordsDatabaseService);
            DocumentManager = new DocumentManager(documentService, fileService);

        }

        private void PatientScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            PatientScheduleView patientScheduleView = new PatientScheduleView();
            patientScheduleView.Activate();
        }




    }
}