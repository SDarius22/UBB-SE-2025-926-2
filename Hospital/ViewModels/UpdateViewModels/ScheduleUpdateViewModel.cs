// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleUpdateViewModel.cs" company="YourCompany">
//   Copyright (c) YourCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel for updating schedules.
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
    using Hospital.ViewModel;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// ViewModel for updating schedules.
    /// </summary>
    public class ScheduleUpdateViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The model for managing schedules.
        /// </summary>
        private readonly ScheduleApiService scheduleModel;

        /// <summary>
        /// The collection of schedules displayed in the view.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleUpdateViewModel"/> class.
        /// </summary>
        public ScheduleUpdateViewModel()
        {
            this.scheduleModel = App.Services.GetRequiredService<ScheduleApiService>();
            this.errorMessage = string.Empty;
            this.SaveChangesCommand = new RelayCommand(this.SaveChanges);
            this.LoadSchedules();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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
        /// Gets the command to save changes.
        /// </summary>
        public ICommand SaveChangesCommand { get; }

        /// <summary>
        /// Gets or sets the collection of schedules displayed in the view.
        /// </summary>
        public ObservableCollection<ScheduleModel> Schedules { get; set; } = new();

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Loads the schedules from the database and populates the Schedules collection.
        /// </summary>
        private async void LoadSchedules()
        {
            this.Schedules.Clear();
            var list = await this.scheduleModel.GetSchedulesAsync();

            foreach (ScheduleModel schedule in list)
            {
                this.Schedules.Add(schedule);
            }
        }

        /// <summary>
        /// Saves the changes made to the schedules.
        /// </summary>
        private async void SaveChanges()
        {
            bool hasErrors = false;
            StringBuilder errorMessages = new StringBuilder();

            foreach (ScheduleModel schedule in this.Schedules)
            {
                bool isValid = await this.ValidateSchedule(schedule);
                if (!isValid)
                {
                    hasErrors = true;
                    errorMessages.AppendLine("Schedule " + schedule.ScheduleId + ": " + this.ErrorMessage);
                }
                else
                {
                    bool success = await this.scheduleModel.UpdateScheduleAsync(schedule.ScheduleId, schedule);

                    if (!success)
                    {
                        errorMessages.AppendLine("Failed to save changes for schedule: " + schedule.DoctorId);
                        hasErrors = true;
                    }
                }
            }

            if (hasErrors)
            {
                this.ErrorMessage = errorMessages.ToString();
            }
            else
            {
                this.ErrorMessage = "Changes saved successfully";
            }
        }

        /// <summary>
        /// Validates the schedule before saving it to the database.
        /// </summary>
        /// <param name="schedule">Schedule to be validated.</param>
        /// <returns>True if the schedule is valid, false otherwise.</returns>
        private async Task<bool> ValidateSchedule(ScheduleModel schedule)
        {
            bool doctorExists = await this.scheduleModel.DoesDoctorExistAsync(schedule.DoctorId);

            if (!doctorExists)
            {
                this.ErrorMessage = "DoctorID doesn’t exist in the Doctors Records";
                return false;
            }

            bool shiftExists = await this.scheduleModel.DoesShiftExistAsync(schedule.ShiftId);

            if (!shiftExists)
            {
                this.ErrorMessage = "ShiftID doesn’t exist in the Shifts Records";
                return false;
            }

            return true;
        }
    }
}