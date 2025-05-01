// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoctorUpdateViewModel.cs" company="YourCompany">
//   Copyright (c) YourCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel responsible for updating doctors, including validation and persistence.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hospital.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Hospital.DatabaseServices;
    using Hospital.DatabaseServices.Interfaces;
    using Hospital.Models;
    using Hospital.Utils;

    /// <summary>
    /// ViewModel for updating doctors in the system.
    /// </summary>
    public class DoctorUpdateViewModel : INotifyPropertyChanged
    {
        private readonly IDoctorsDatabaseService doctorModel;
        private readonly IUserDatabaseService userModel;
        private string errorMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorUpdateViewModel"/> class.
        /// </summary>
        public DoctorUpdateViewModel(IDoctorsDatabaseService doctorModel, IUserDatabaseService userModel)
        {
            this.doctorModel = doctorModel;
            this.userModel = userModel;

            this.SaveChangesCommand = new RelayCommand(this.SaveChanges);
            this.LoadDoctors();
        }

        /// <summary>
        /// Gets or sets the error message for display.
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
        /// Gets the command for saving changes to doctors.
        /// </summary>
        public ICommand SaveChangesCommand { get; }

        /// <summary>
        /// Gets the list of doctors to be displayed and updated.
        /// </summary>
        public ObservableCollection<DoctorJointModel> Doctors { get; set; } = new ObservableCollection<DoctorJointModel>();

        /// <summary>
        /// Loads the doctors from the model into the view model.
        /// </summary>
        private void LoadDoctors()
        {
            this.Doctors.Clear();

            foreach (DoctorJointModel doctor in this.doctorModel.GetDoctors())
            {
                this.Doctors.Add(doctor);
            }
        }

        /// <summary>
        /// Saves the changes made to each doctor after validation.
        /// </summary>
        private async void SaveChanges()
        {
            bool hasErrors = false;
            StringBuilder errorMessages = new StringBuilder();

            foreach (DoctorJointModel doctor in this.Doctors)
            {
                if (!await this.ValidateDoctor(doctor))
                {
                    hasErrors = true;
                    errorMessages.AppendLine($"Doctor {doctor.DoctorId}: {this.ErrorMessage}");
                }
                else
                {
                    bool success = await this.doctorModel.UpdateDoctor(doctor);
                    if (!success)
                    {
                        errorMessages.AppendLine($"Failed to save changes for doctor: {doctor.DoctorId}");
                        hasErrors = true;
                    }
                }
            }

            this.ErrorMessage = hasErrors ? errorMessages.ToString() : "Changes saved successfully";
        }

        /// <summary>
        /// Validates the information of a single doctor.
        /// </summary>
        /// <param name="doctor">The doctor to validate.</param>
        /// <returns><c>true</c> if valid; otherwise, <c>false</c>.</returns>
        private async Task<bool> ValidateDoctor(DoctorJointModel doctor)
        {
            if (!await this.userModel.UserExistsWithRole(doctor.UserId, "Doctor") ||
                await this.doctorModel.UserExistsInDoctors(doctor.UserId, doctor.DoctorId))
            {
                this.ErrorMessage = "UserID doesn’t exist or has already been approved";
                return false;
            }

            if (!await this.doctorModel.DoesDepartmentExist(doctor.DepartmentId))
            {
                this.ErrorMessage = "DepartmentID doesn’t exist in the Departments Records";
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(doctor.LicenseNumber, @"^[a-zA-Z0-9 ]*$"))
            {
                this.ErrorMessage = "License Number should contain only alphanumeric characters";
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