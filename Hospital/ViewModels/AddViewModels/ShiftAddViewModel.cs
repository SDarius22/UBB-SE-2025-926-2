// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShiftAddViewModel.cs" company="YourCompany">
//   Copyright (c) YourCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel for adding shifts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Hospital.DatabaseServices;
using Hospital.Models;
using Hospital.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.ViewModels.AddViewModels
{
    /// <summary>
    /// ViewModel for adding shifts.
    /// </summary>
    public class ShiftAddViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Gets or sets the model for managing shifts.
        /// </summary>
        private readonly IShiftsDatabaseService shiftModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftAddViewModel"/> class.
        /// </summary>
        public ShiftAddViewModel()
        {

            this.SaveShiftCommand = new RelayCommand(this.SaveShift);
            this.LoadShifts();
            this.shiftModel = App.Services.GetRequiredService<IShiftsDatabaseService>();
        }

        /// <summary>
        /// Gets or sets the collection of shifts displayed in the view.
        /// </summary>
        public ObservableCollection<ShiftModel> Shifts { get; set; } = new();

        /// <summary>
        /// Gets or sets the ID of the shift to be deleted.
        /// </summary>
        public DateOnly Date
        {
            get => this.date;
            set
            {
                this.date = value;
                this.OnPropertyChanged(nameof(this.Date));
            }
        }

        /// <summary>
        /// Gets or sets the start time of the shift.
        /// </summary>
        public TimeSpan StartTime
        {
            get => this.startTime;
            set
            {
                this.startTime = value;
                this.OnPropertyChanged(nameof(this.StartTime));
            }
        }

        /// <summary>
        /// Gets or sets the end time of the shift.
        /// </summary>
        public TimeSpan EndTime
        {
            get => this.endTime;
            set
            {
                this.endTime = value;
                this.OnPropertyChanged(nameof(this.EndTime));
            }
        }

        /// <summary>
        /// Gets or sets the error message to display in the view.
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
        /// Gets the command to save a shift.
        /// </summary>
        public ICommand SaveShiftCommand { get; }

        /// <summary>
        /// Gets or sets the model for managing doctors.
        /// </summary>
        private DateOnly date = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Gets or sets the start time of the shift.
        /// </summary>
        private TimeSpan startTime;

        /// <summary>
        /// Gets or sets the end time of the shift.
        /// </summary>
        private TimeSpan endTime;

        /// <summary>
        /// Gets or sets the error message to display in the view.
        /// </summary>
        private string errorMessage = string.Empty;

        /// <summary>
        /// Loads the shifts from the database and populates the Shifts collection.
        /// </summary>
        private async void LoadShifts()
        {
            this.Shifts.Clear();
            var list = await this.shiftModel.GetShifts();
            foreach (ShiftModel shift in list)
            {
                this.Shifts.Add(shift);
            }
        }

        /// <summary>
        /// Saves the shift to the database.
        /// </summary>
        private async void SaveShift()
        {
            var shift = new ShiftModel(
                0,
                this.Date,
                this.StartTime,
                this.EndTime);

            if (this.ValidateShift(shift))
            {
                bool success = await this.shiftModel.AddShift(shift);
                this.ErrorMessage = success ? "Shift added successfully" : "Failed to add shift";
                if (success)
                {
                    this.LoadShifts();
                }
            }
        }

        /// <summary>
        /// Validates the shift details.
        /// </summary>
        /// <param name="shift">The shift to validate.</param>
        /// <returns>True if the shift is valid, false otherwise.</returns>
        private bool ValidateShift(ShiftModel shift)
        {
            if (shift.StartTime != new TimeSpan(8, 0, 0) && shift.StartTime != new TimeSpan(20, 0, 0))
            {
                this.ErrorMessage = "Start time should be either 8:00 AM or 8:00 PM";
                return false;
            }

            if (shift.EndTime != new TimeSpan(8, 0, 0) && shift.EndTime != new TimeSpan(20, 0, 0))
            {
                this.ErrorMessage = "End time should be either 8:00 AM or 8:00 PM";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}