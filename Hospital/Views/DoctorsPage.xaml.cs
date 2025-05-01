// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Doctors.xaml.cs" company="YourCompany">
//   Copyright (c) YourCompany. All rights reserved.
// </copyright>
// <summary>
//   This file contains the code-behind for the DoctorsPage in the GUI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Hospital.Managers;
using Hospital.Services;

namespace Hospital.Views
{
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using Hospital.Models;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Hospital.DatabaseServices;
    using Hospital.DbContext;

    /// <summary>
    /// A page that displays a list of doctors with sorting and search functionality.
    /// </summary>
    public sealed partial class DoctorsPage : Page
    {
        /// <summary>
        /// Gets or sets the list of doctors.
        /// </summary>
        public ObservableCollection<DoctorJointModel> Doctors { get; set; } = new ();

        private readonly AppDbContext _context;

        private DoctorsDatabaseService doctorModel;

        private Dictionary<string, ListSortDirection> sortingStates = new ()
        {
            { "DoctorID", ListSortDirection.Ascending },
            { "Experience", ListSortDirection.Ascending },
            { "Rating", ListSortDirection.Ascending },
        };

        /// <summary>
        /// Gets or sets the doctor ID.
        /// </summary>
        public int DoctorID { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsPage"/> class.
        /// </summary>
        public DoctorsPage()
        {
            this.InitializeComponent();
            this.LoadDoctors();
            this.DataContext = this;
        }

        /// <summary>
        /// Loads and sorts the list of doctors.
        /// </summary>
        private void LoadDoctors()
        {
            this.Doctors.Clear();
            List<DoctorJointModel> doctorsList = this.doctorModel.GetDoctors();

            foreach (DoctorJointModel doctor in doctorsList)
            {
                this.Doctors.Add(doctor);
            }

            var sorted = this.SortDoctors(this.Doctors, "DoctorID", ListSortDirection.Ascending);
            this.Doctors.Clear();

            foreach (var doctor in sorted)
            {
                this.Doctors.Add(doctor);
            }
        }

        /// <summary>
        /// Handles the More Info button click event.
        /// </summary>
        private void MoreInfoClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DoctorJointModel doctor)
            {
                // Create instances of the required services
                var appointmentsDatabaseService = new AppointmentsDatabaseService(_context); // Replace with actual implementation
                var shiftsDatabaseService = new ShiftsDatabaseService(_context); // Replace with actual implementation
                var medicalRecordsDatabaseService = new MedicalRecordsDatabaseService(_context); // Replace with actual implementation
                var documentDatabaseService = new DocumentDatabaseService(_context); // Replace with actual implementation
                var fileService = new FileService(); // Replace with actual implementation

                // Create instances of the required managers
                IAppointmentManager appointmentManager = new AppointmentManager(appointmentsDatabaseService);
                IShiftManager shiftManager = new ShiftManager(shiftsDatabaseService);
                IMedicalRecordManager medicalRecordManager = new MedicalRecordManager(medicalRecordsDatabaseService);
                IDocumentManager documentManager = new DocumentManager(documentDatabaseService, fileService);

                // Create and show the DoctorScheduleView window
                var scheduleWindow = new DoctorScheduleView(appointmentManager, shiftManager, medicalRecordManager, documentManager);
                scheduleWindow.Activate();
            }
        }

        /// <summary>
        /// Handles sorting by doctor ID.
        /// </summary>
        private void SortByDoctorID(object sender, RoutedEventArgs e)
        {
            this.ToggleSort("DoctorID");
        }

        /// <summary>
        /// Handles sorting by rating.
        /// </summary>
        private void SortByRating(object sender, RoutedEventArgs e)
        {
            this.ToggleSort("Rating");
        }

        /// <summary>
        /// Toggles the sort direction and reorders the list.
        /// </summary>
        private void ToggleSort(string field)
        {
            if (this.sortingStates[field] == ListSortDirection.Ascending)
            {
                this.sortingStates[field] = ListSortDirection.Descending;
            }
            else
            {
                this.sortingStates[field] = ListSortDirection.Ascending;
            }

            var sortedDoctors = this.SortDoctors(this.Doctors, field, this.sortingStates[field]);
            this.Doctors.Clear();
            foreach (var doctor in sortedDoctors)
            {
                this.Doctors.Add(doctor);
            }
        }

        /// <summary>
        /// Sorts the doctor list based on a given field and direction.
        /// </summary>
        private ObservableCollection<DoctorJointModel> SortDoctors(ObservableCollection<DoctorJointModel> doctors, string field, ListSortDirection direction)
        {
            List<DoctorJointModel> sortedDoctors = doctors.ToList();

            if (field == "DoctorID")
            {
                sortedDoctors = direction == ListSortDirection.Ascending
                    ? sortedDoctors.OrderBy(x => x.DoctorId).ToList()
                    : sortedDoctors.OrderByDescending(x => x.DoctorId).ToList();
            }
            else if (field == "Rating")
            {
                sortedDoctors = direction == ListSortDirection.Ascending
                    ? sortedDoctors.OrderBy(x => x.DoctorRating).ToList()
                    : sortedDoctors.OrderByDescending(x => x.DoctorRating).ToList();
            }

            return new ObservableCollection<DoctorJointModel>(sortedDoctors);
        }

        /// <summary>
        /// Handles the search box text change event.
        /// </summary>
        private async void SearchBox_TextChange(object sender, RoutedEventArgs e)
        {
            string search = this.SearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(search))
            {
                this.LoadDoctors();
                return;
            }

            var filteredDoctors = this.Doctors.Where(doctor => doctor.UserId.ToString().Contains(search)).ToList();

            if (filteredDoctors.Count == 0)
            {
                var dialog = new ContentDialog
                {
                    Title = "No Results",
                    Content = "No doctors found with this id.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot,
                };

                await dialog.ShowAsync();
                return;
            }

            this.Doctors.Clear();
            foreach (var doctor in filteredDoctors)
            {
                this.Doctors.Add(doctor);
            }
        }
    }
}