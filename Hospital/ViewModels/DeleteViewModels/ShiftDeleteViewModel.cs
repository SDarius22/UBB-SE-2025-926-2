﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShiftDeleteViewModel.cs" company="YourCompany">
//   Copyright (c) YourCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel for deleting shifts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hospital.ViewModels.DeleteViewModels
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
    using Windows.ApplicationModel.VoiceCommands;

    /// <summary>
    /// ViewModel for deleting shifts.
    /// </summary>
    public class ShiftDeleteViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The model for managing shifts.
        /// </summary>
        private readonly ShiftsApiService shiftModel;

        /// <summary>
        /// The collection of shifts displayed in the view.
        /// </summary>
        private ObservableCollection<ShiftModel> shifts;

        /// <summary>
        /// The ID of the shift to be deleted.
        /// </summary>
        private int shiftID;

        /// <summary>
        /// The error message to be displayed.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// The color of the message to be displayed.
        /// </summary>
        private string messageColor = "Red";

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftDeleteViewModel"/> class.
        /// </summary>
        public ShiftDeleteViewModel()
        {
            this.shiftModel = App.Services.GetRequiredService<ShiftsApiService>();
            this.DeleteShiftCommand = new RelayCommand(this.RemoveShift);
            this.LoadShifts();
        }

        private async void LoadShifts()
        {
            var shifts = await this.shiftModel.GetShiftsAsync();
            this.Shifts = new ObservableCollection<ShiftModel>(shifts);
        }

        /// <summary>
        /// Gets or sets the collection of shifts displayed in the view.
        /// </summary>
        public ObservableCollection<ShiftModel> Shifts
        {
            get => this.shifts;
            set => this.SetProperty(ref this.shifts, value);
        }

        /// <summary>
        /// Gets or sets the ID of the shift to be deleted.
        /// </summary>
        public int ShiftID
        {
            get => this.shiftID;
            set
            {
                this.shiftID = value;
                this.OnPropertyChanged(nameof(this.ShiftID));
                this.OnPropertyChanged(nameof(this.CanDeleteShift));
            }
        }

        /// <summary>
        /// Gets or sets the error message to display in the view.
        /// </summary>
        public string ErrorMessage
        {
            get => this.errorMessage ?? string.Empty;
            set
            {
                this.errorMessage = value;
                this.MessageColor = string.IsNullOrEmpty(value) ? "Red" : value.Contains("successfully") ? "Green" : "Red";
                this.OnPropertyChanged(nameof(this.ErrorMessage));
                this.OnPropertyChanged(nameof(this.MessageColor));
            }
        }

        /// <summary>
        /// Gets or sets the color of the message displayed in the view.
        /// </summary>
        public string MessageColor
        {
            get => this.messageColor;
            set
            {
                this.messageColor = value;
                this.OnPropertyChanged(nameof(this.MessageColor));
            }
        }

        /// <summary>
        /// Gets the command to delete a shift.
        /// </summary>
        public ICommand DeleteShiftCommand { get; }

        /// <summary>
        /// Gets a value indicating whether the shift can be deleted.
        /// </summary>
        public bool CanDeleteShift => this.ShiftID > 0;

        /// <summary>
        /// Removes the selected shift.
        /// </summary>
        private async void RemoveShift()
        {
            if (this.ShiftID == 0)
            {
                this.ErrorMessage = "No shift was selected";
                return;
            }

            if (!await this.shiftModel.DoesShiftExistAsync(this.ShiftID))
            {
                this.ErrorMessage = "ShiftID doesn't exist in the records";
                return;
            }

            bool succes = await this.shiftModel.DeleteShiftAsync(this.ShiftID);
            this.ErrorMessage = succes ? "Shift was successfully deleted" : "Shift was not deleted";
            if (succes)
            {
                var list = await this.shiftModel.GetShiftsAsync();
                this.Shifts = new ObservableCollection<ShiftModel>(list);
            }
        }

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the view that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <param name="field">The field to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <typeparam name="T">The type of the field.</typeparam>
        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                this.OnPropertyChanged(propertyName);
            }
        }
    }
}