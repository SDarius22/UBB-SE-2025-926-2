﻿namespace Hospital.ViewModels.DeleteViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using Hospital.ApiClients;
    using Hospital.Models;
    using Hospital.Utils;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// ViewModel for deleting departments.
    /// </summary>
    public class DepartmentDeleteViewModel : INotifyPropertyChanged
    {
        private readonly DepartmentsApiService departmentModel;
        private ObservableCollection<DepartmentModel> departments;
        private int departmentID;
        private string errorMessage = string.Empty;
        private string messageColor = "Red";

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDeleteViewModel"/> class.
        /// </summary>
        public DepartmentDeleteViewModel()
        {
            // Load departments for the DataGrid
            this.departmentModel = App.Services.GetRequiredService<DepartmentsApiService>();
            LoadDepartments();
            this.DeleteDepartmentCommand = new RelayCommand(this.RemoveDepartment);
        }

        private async void LoadDepartments()
        {
            this.departments = new ObservableCollection<DepartmentModel>(await this.departmentModel.GetDepartmentsAsync());
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the collection of departments.
        /// </summary>
        public ObservableCollection<DepartmentModel> Departments
        {
            get => this.departments;
            set => this.SetProperty(ref this.departments, value);
        }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public int DepartmentID
        {
            get => this.departmentID;
            set
            {
                this.departmentID = value;
                this.OnPropertyChanged(nameof(this.DepartmentID));
                this.OnPropertyChanged(nameof(this.CanDeleteDepartment));
            }
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get => this.errorMessage;
            set
            {
                this.errorMessage = value;
                this.MessageColor = string.IsNullOrEmpty(value) ? "Red" : value.Contains("successfully") ? "Green" : "Red";
                this.OnPropertyChanged(nameof(this.ErrorMessage));
                this.OnPropertyChanged(nameof(this.MessageColor));
            }
        }

        /// <summary>
        /// Gets or sets the message color.
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
        /// Gets the command to delete the department.
        /// </summary>
        public ICommand DeleteDepartmentCommand { get; }

        /// <summary>
        /// Gets a value indicating whether the department can be deleted.
        /// </summary>
        public bool CanDeleteDepartment => this.DepartmentID > 0;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Removes the department from the database.
        /// </summary>
        private async void RemoveDepartment()
        {
            if (this.DepartmentID == 0)
            {
                this.ErrorMessage = "No department was selected";
                return;
            }

            if (!await this.departmentModel.DoesDepartmentExistAsync(this.DepartmentID))
            {
                this.ErrorMessage = "DepartmentID doesn't exist in the records";
                return;
            }

            bool success = await this.departmentModel.DeleteDepartmentAsync(this.DepartmentID);
            this.ErrorMessage = success ? "Department deleted successfully" : "Failed to delete department";

            if (success)
            {
                this.Departments = new ObservableCollection<DepartmentModel>(await this.departmentModel.GetDepartmentsAsync());
            }
        }

        /// <summary>
        /// Sets the property value and raises the <see cref="PropertyChanged"/> event if the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The field to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="propertyName">The name of the property. This is optional and will be automatically set by the compiler.</param>
        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null!)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                this.OnPropertyChanged(propertyName);
            }
        }
    }
}