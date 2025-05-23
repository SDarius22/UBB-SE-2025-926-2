﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrugDeleteViewModel.cs" company="YourCompanyName">
//   Copyright (c) YourCompanyName. All rights reserved.
// </copyright>
// <summary>
//   ViewModel for deleting drugs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Hospital.Models;
using Hospital.Utils;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hospital.ApiClients;

namespace Hospital.ViewModels.DeleteViewModels
{
    /// <summary>
    /// ViewModel for managing the deletion of drugs.
    /// </summary>
    public class DrugDeleteViewModel : INotifyPropertyChanged
    {
        private readonly DrugsApiService drugModel;
        private ObservableCollection<DrugModel> drugs;
        private int drugID;
        private string errorMessage;
        private string messageColor = "Red";

        /// <summary>
        /// Gets or sets the collection of drugs displayed in the DataGrid.
        /// </summary>
        public ObservableCollection<DrugModel> Drugs
        {
            get { return this.drugs; }
            set { this.SetProperty(ref this.drugs, value); }
        }

        /// <summary>
        /// Gets or sets the ID of the drug to be deleted.
        /// </summary>
        public int DrugID
        {
            get => this.drugID;
            set
            {
                this.drugID = value;
                this.OnPropertyChanged(nameof(this.DrugID));
                this.OnPropertyChanged(nameof(this.CanDeleteDrug));
            }
        }

        /// <summary>
        /// Gets or sets the error message to display in the UI.
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
        /// Gets or sets the color of the message displayed in the UI.
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
        /// Gets the command to delete a drug.
        /// </summary>
        public ICommand DeleteDrugCommand { get; }

        /// <summary>
        /// Gets a value indicating whether a drug can be deleted.
        /// </summary>
        public bool CanDeleteDrug => this.DrugID > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrugDeleteViewModel"/> class.
        /// </summary>
        public DrugDeleteViewModel()
        {
            // Initialize non-nullable fields
            this.drugModel = App.Services.GetRequiredService<DrugsApiService>();
            ;
            this.errorMessage = string.Empty;

            // Load drugs for the DataGrid
            LoadDrugs();

            this.DeleteDrugCommand = new RelayCommand(this.RemoveDrug);
        }

        private async void LoadDrugs()
        {
            this.drugs = new ObservableCollection<DrugModel>(await this.drugModel.GetDrugsAsync());
        }

        /// <summary>
        /// Determines whether the delete drug command can execute.
        /// </summary>
        /// <returns>True if a drug can be deleted; otherwise, false.</returns>
        private bool CanExecuteDeleteDrug()
        {
            return this.DrugID > 0;
        }

        /// <summary>
        /// Removes the selected drug from the database.
        /// </summary>
        private async void RemoveDrug()
        {
            if (this.DrugID == 0)
            {
                this.ErrorMessage = "No drug was selected";
                return;
            }

            if (!await this.drugModel.DoesDrugExistAsync(this.DrugID))
            {
                this.ErrorMessage = "DrugID doesn't exist in the records";
                return;
            }

            bool success = await this.drugModel.DeleteDrugAsync(this.DrugID);
            this.ErrorMessage = success ? "Drug deleted successfully" : "Failed to delete drug";

            if (success)
            {
                this.Drugs = new ObservableCollection<DrugModel>(await this.drugModel.GetDrugsAsync());
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the specified field to the given value and raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="field">The field to set.</param>
        /// <param name="value">The value to set the field to.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                this.OnPropertyChanged(propertyName);
            }
        }
    }
}