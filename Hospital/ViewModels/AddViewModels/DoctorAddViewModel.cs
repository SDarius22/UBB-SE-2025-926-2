﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoctorAddViewModel.cs" company="YourCompany">
//   Copyright (c) YourCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel responsible for handling doctor addition logic, validation, and data binding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hospital.ViewModels.AddViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Hospital.ApiClients;
    using Hospital.Models;
    using Hospital.Utils;
    using Microsoft.Extensions.DependencyInjection;
    using Windows.System;

    /// <summary>
    /// ViewModel for adding a new doctor.
    /// </summary>
    public class DoctorAddViewModel : INotifyPropertyChanged
    {
        // Private fields
        private readonly DoctorApiService doctorModel;
        private int userID;
        private int departmentID;
        private int rating;
        private string licenseNumber = string.Empty;
        private string errorMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorAddViewModel"/> class.
        /// </summary>
        public DoctorAddViewModel()
        {
            this.doctorModel = App.Services.GetRequiredService<DoctorApiService>();
            this.SaveDoctorCommand = new RelayCommand(this.SaveDoctor);
            this.LoadDoctors();
        }

        /// <summary>
        /// Gets the list of all doctors.
        /// </summary>
        public ObservableCollection<DoctorJointModel> Doctors { get; set; } = new ObservableCollection<DoctorJointModel>();

        /// <summary>
        /// Gets or sets the UserID of the doctor.
        /// </summary>
        public int UserID
        {
            get => this.userID;
            set
            {
                this.userID = value;
                this.OnPropertyChanged(nameof(this.UserID));
            }
        }

        /// <summary>
        /// Gets or sets the DepartmentID of the doctor.
        /// </summary>
        public int DepartmentID
        {
            get => this.departmentID;
            set
            {
                this.departmentID = value;
                this.OnPropertyChanged(nameof(this.DepartmentID));
            }
        }

        /// <summary>
        /// Gets or sets the Rating of the doctor.
        /// </summary>
        public int Rating
        {
            get => this.rating;
            set
            {
                this.rating = value;
                this.OnPropertyChanged(nameof(this.Rating));
            }
        }

        /// <summary>
        /// Gets or sets the license number of the doctor.
        /// </summary>
        public string LicenseNumber
        {
            get => this.licenseNumber;
            set
            {
                this.licenseNumber = value;
                this.OnPropertyChanged(nameof(this.LicenseNumber));
            }
        }

        /// <summary>
        /// Gets or sets the error message to display in the UI.
        /// </summary>
        public string ErrorMessage
        {
            get => this.errorMessage;
            set
            {
                this.errorMessage = value;
                this.OnPropertyChanged(nameof(this.ErrorMessage));
            }
        }

        /// <summary>
        /// Gets the command that triggers the SaveDoctor method.
        /// </summary>
        public ICommand SaveDoctorCommand { get; }

        /// <summary>
        /// Loads all doctors into the ObservableCollection.
        /// </summary>
        private async void LoadDoctors()
        {
            this.Doctors.Clear();
            var list = await this.doctorModel.GetDoctorsAsync();
            foreach (DoctorJointModel doctor in list)
            {
                this.Doctors.Add(doctor);
            }
        }

        /// <summary>
        /// Creates and saves a new doctor if validation passes.
        /// </summary>
        private async void SaveDoctor()
        {
            var doctor = new DoctorJointModel
            {
                UserId = this.UserID,
                DepartmentId = this.DepartmentID,
                LicenseNumber = this.LicenseNumber,
                Rating = this.Rating,
            };

            bool success = await this.doctorModel.AddDoctorAsync(doctor);
            this.ErrorMessage = success ? "Doctor added successfully" : "Failed to add doctor";

            if (success)
            {
                this.LoadDoctors();
            }
        }

        /// <summary>
        /// Validates the doctor information before saving.
        /// </summary>
        /// <param name="doctor">The doctor to validate.</param>
        /// <returns><c>true</c> if valid; otherwise, <c>false</c>.</returns>
        private async Task<bool> ValidateDoctor(DoctorJointModel doctor)
        {
            if (!await this.doctorModel.DoesUserExistAsync(doctor.UserId))
            {
                this.ErrorMessage = "UserID doesn’t exist in the Users Records.";
                return false;
            }

            if (!await this.doctorModel.IsUserDoctorAsync(doctor.UserId))
            {
                this.ErrorMessage = "The user with this UserID is not a Doctor.";
                return false;
            }

            if (await this.doctorModel.IsUserAlreadyDoctorAsync(doctor.UserId))
            {
                this.ErrorMessage = "The user already exists in the Doctors Records.";
                return false;
            }
            Debug.WriteLine($"Department ID: {doctor.DepartmentId}");
            if (!await this.doctorModel.DoesDepartmentExistAsync(doctor.DepartmentId))
            {
                this.ErrorMessage = "DepartmentID doesn’t exist in the Departments Records.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(doctor.LicenseNumber))
            {
                this.ErrorMessage = "Please enter the License Number.";
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Triggers the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}