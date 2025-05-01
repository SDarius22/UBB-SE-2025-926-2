using Hospital.DatabaseServices.Interfaces;

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
    /// ViewModel for updating room information. Handles validation, updating rooms, and error message management.
    /// </summary>
    public class RoomUpdateViewModel : INotifyPropertyChanged
    {
        private readonly RoomApiService roomModel;
        private string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomUpdateViewModel"/> class.
        /// </summary>
        public RoomUpdateViewModel()
        {
            this.roomModel = App.Services.GetRequiredService<IRoomDatabaseService>();
            this.errorMessage = string.Empty;
            this.SaveChangesCommand = new RelayCommand(this.SaveChanges);
            this.LoadRooms();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the collection of rooms to update.
        /// </summary>
        public ObservableCollection<RoomModel> Rooms { get; set; } = new ObservableCollection<RoomModel>();

        /// <summary>
        /// Gets or sets the error message displayed to the user.
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
        /// Gets the command that triggers saving room changes.
        /// </summary>
        public ICommand SaveChangesCommand { get; }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Loads the list of rooms into the <see cref="Rooms"/> collection.
        /// </summary>
        private async void LoadRooms()
        {
            this.Rooms.Clear();
            foreach (RoomModel room in await this.roomModel.GetRooms() ?? Enumerable.Empty<RoomModel>())
            {
                this.Rooms.Add(room);
            }
        }

        /// <summary>
        /// Saves the changes to each room. Validates the room data and reports errors if any occur.
        /// </summary>
        private async void SaveChanges()
        {
            bool hasErrors = false;
            StringBuilder errorMessages = new StringBuilder();

            foreach (RoomModel room in this.Rooms)
            {
                bool isValid = await this.ValidateRoom(room);
                if (!isValid)
                {
                    hasErrors = true;
                    errorMessages.AppendLine("Room " + room.RoomID + ": " + this.ErrorMessage);
                }
                else
                {
                    bool success = await this.roomModel.UpdateRoom(room);
                    if (!success)
                    {
                        errorMessages.AppendLine("Failed to save changes for room: " + room.RoomID);
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
        /// Validates the given room to ensure it has valid data.
        /// </summary>
        /// <param name="room">The room to validate.</param>
        /// <returns>True if the room is valid, otherwise false.</returns>
        private async Task<bool> ValidateRoom(RoomModel room)
        {
            if (room.Capacity <= 0)
            {
                this.ErrorMessage = "Please enter a number >0.";
                return false;
            }

            bool departmentExists = await this.roomModel.DoesDepartmentExist(room.DepartmentID);
            if (!departmentExists)
            {
                this.ErrorMessage = "Department ID doesn’t exist in the Departments Records";
                return false;
            }

            bool equipmentExists = await this.roomModel.DoesEquipmentExist(room.EquipmentID);
            if (!equipmentExists)
            {
                this.ErrorMessage = "Equipment ID doesn’t exist in the Equipment Records";
                return false;
            }

            return true;
        }
    }
}