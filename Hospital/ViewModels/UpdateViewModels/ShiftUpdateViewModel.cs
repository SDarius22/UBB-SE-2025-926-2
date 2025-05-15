// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShiftUpdateViewModel.cs" company="YourCompany">
//   Copyright (c) YourCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel for updating shifts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hospital.ViewModels.UpdateViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Hospital.ApiClients;
    using Hospital.Models;
    using Hospital.Utils;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// ViewModel for updating shifts.
    /// </summary>
    public class ShiftUpdateViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The model for managing shifts.
        /// </summary>
        private readonly ShiftsApiService shiftModel;

        /// <summary>
        /// The collection of shifts displayed in the view.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftUpdateViewModel"/> class.
        /// </summary>
        public ShiftUpdateViewModel()
        {
            this.shiftModel = App.Services.GetRequiredService<ShiftsApiService>();
            this.errorMessage = string.Empty;
            this.SaveChangesCommand = new RelayCommand(this.SaveChanges);
            this.LoadShifts();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the ID of the shifts to be updated.
        /// </summary>
        public ObservableCollection<ShiftModel> Shifts { get; set; } = new();

        /// <summary>
        /// Gets or sets the error message to be displayed.
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
        /// Gets the command for saving changes.
        /// </summary>
        public ICommand SaveChangesCommand { get; }

        /// <summary>
        /// Loads the shifts from the model.
        /// </summary>
        private async void LoadShifts()
        {
            this.Shifts.Clear();
            var result = await this.shiftModel.GetShiftsAsync();
            foreach (ShiftModel shift in result)
            {
                this.Shifts.Add(shift);
            }
        }

        /// <summary>
        /// Saves the changes made to the shifts.
        /// </summary>
        private async void SaveChanges()
        {
            bool hasErrors = false;
            StringBuilder errorMessages = new StringBuilder();

            foreach (ShiftModel shift in this.Shifts)
            {
                string validationError = ValidateShift(shift);
                if (!string.IsNullOrEmpty(validationError))
                {
                    hasErrors = true;
                    errorMessages.AppendLine("Shift " + shift.ShiftID + ": " + this.ErrorMessage);
                }
                else
                {
                    bool success = await this.shiftModel.UpdateShiftAsync(shift.ShiftID, shift);
                    if (!success)
                    {
                        errorMessages.AppendLine("Failed to save changes for shift: " + shift.ShiftID);
                        hasErrors = true;
                    }
                }
            }

            this.ErrorMessage = hasErrors ? errorMessages.ToString() : "Changes saved successfully";
        }

        private string ValidateShift(ShiftModel shift)
        {
            if (shift.StartTime != new TimeSpan(8, 0, 0) && shift.StartTime != new TimeSpan(20, 0, 0))
            {
                return "Start time should be either 8:00 AM or 8:00 PM";
            }

            if (shift.EndTime != new TimeSpan(8, 0, 0) && shift.EndTime != new TimeSpan(20, 0, 0))
            {
                return "End time should be either 8:00 AM or 8:00 PM";
            }

            return string.Empty;
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}