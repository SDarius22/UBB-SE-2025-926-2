namespace Hospital.ViewModels.UpdateViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text;
    using System.Windows.Input;
    using Hospital.DatabaseServices;
    using Hospital.Models;
    using Hospital.Utils;
    using Microsoft.VisualBasic;

    /// <summary>
    /// ViewModel for updating departments.
    /// </summary>
    public class DepartmentUpdateViewModel : INotifyPropertyChanged
    {
        private readonly IDepartmentsDatabaseService departmentModel;
        private string errorMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentUpdateViewModel"/> class.
        /// </summary>
        public DepartmentUpdateViewModel(IDepartmentsDatabaseService departmentModel)
        {
            this.departmentModel = departmentModel;
            this.SaveChangesCommand = new RelayCommand(this.SaveChanges);
            this.LoadDepartments();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the collection of departments.
        /// </summary>
        public ObservableCollection<DepartmentModel> Departments { get; set; } = new ObservableCollection<DepartmentModel>();

        /// <summary>
        /// Gets or sets the error message.
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
        /// Gets the command to save changes to the departments.
        /// </summary>
        public ICommand SaveChangesCommand { get; }

        /// <summary>
        /// Loads the departments from the database.
        /// </summary>
        private async void LoadDepartments()
        {
            this.Departments.Clear();
            var list = await this.departmentModel.GetDepartments();
            foreach (DepartmentModel department in list)
            {
                this.Departments.Add(department);
            }
        }

        /// <summary>
        /// Saves the changes to the departments in the database.
        /// </summary>
        private async void SaveChanges()
        {
            bool hasErrors = false;
            StringBuilder errorMessages = new StringBuilder();

            foreach (DepartmentModel department in this.Departments)
            {
                if (!this.ValidateDepartment(department))
                {
                    hasErrors = true;
                    errorMessages.AppendLine("Department " + department.DepartmentId + ": " + this.ErrorMessage);
                }
                else
                {
                    bool success = await this.departmentModel.UpdateDepartment(department);
                    if (!success)
                    {
                        errorMessages.AppendLine("Failed to save changes for department: " + department.DepartmentId);
                        hasErrors = true;
                    }
                }
            }

            this.ErrorMessage = hasErrors ? errorMessages.ToString() : "Changes saved successfully";
        }

        /// <summary>
        /// Validates the department.
        /// </summary>
        /// <param name="department">The department to validate.</param>
        /// <returns>True if the department is valid, otherwise false.</returns>
        private bool ValidateDepartment(DepartmentModel department)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(department.DepartmentName, @"^[a-zA-Z0-9 ]*$"))
            {
                this.ErrorMessage = "Department Name should contain only alphanumeric characters";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}