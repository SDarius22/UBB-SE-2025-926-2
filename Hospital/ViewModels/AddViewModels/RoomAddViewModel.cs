namespace Hospital.ViewModels.AddViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Hospital.ApiClients;
    using Hospital.Models;
    using Hospital.Utils;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// ViewModel for adding rooms. Handles room data binding, validation, and interaction with the RoomModel.
    /// </summary>
    public class RoomAddViewModel : INotifyPropertyChanged
    {
        private readonly RoomApiService roomModel;
        private int capacity;
        private int departmentID;
        private string errorMessage = string.Empty;
        private int equipmentID;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomAddViewModel"/> class.
        /// </summary>
        public RoomAddViewModel()
        {
            this.roomModel = App.Services.GetRequiredService<RoomApiService>();
            this.SaveRoomCommand = new RelayCommand(this.SaveRoom);
            this.LoadRooms();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the collection of existing rooms.
        /// </summary>
        public ObservableCollection<RoomModel> Rooms { get; set; } = new ObservableCollection<RoomModel>();

        /// <summary>
        /// Gets or sets the capacity for a new room.
        /// </summary>
        public int Capacity
        {
            get => this.capacity;
            set
            {
                this.capacity = value;
                this.OnPropertyChanged(nameof(this.Capacity));
            }
        }

        /// <summary>
        /// Gets or sets the department ID for a new room.
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
        /// Gets or sets the equipment ID for a new room.
        /// </summary>
        public int EquipmentID
        {
            get => this.equipmentID;
            set
            {
                this.equipmentID = value;
                this.OnPropertyChanged(nameof(this.EquipmentID));
            }
        }

        /// <summary>
        /// Gets or sets the error message to be displayed in the UI.
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
        /// Gets the command used to save a new room.
        /// </summary>
        public ICommand SaveRoomCommand { get; }

        /// <summary>
        /// Notifies the UI of a property change.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Loads rooms from the model into the observable collection.
        /// </summary>
        private async void LoadRooms()
        {
            this.Rooms.Clear();

            var list = await this.roomModel?.GetRoomsAsync();

            foreach (RoomModel room in list ?? Enumerable.Empty<RoomModel>())
            {
                this.Rooms.Add(room);
            }

        }

        /// <summary>
        /// Saves a new room to the model after validating it.
        /// </summary>
        private async void SaveRoom()
        {
            var room = new RoomModel
            {
                RoomID = 0,
                Capacity = this.Capacity,
                DepartmentID = this.DepartmentID,
                EquipmentID = this.EquipmentID,
            };

            if (await this.ValidateRoom(room))
            {
                bool success = await this.roomModel.AddRoomAsync(room);
                this.ErrorMessage = success ? "Room added successfully" : "Failed to add room";
                if (success)
                {
                    this.LoadRooms();
                }
            }
        }

        /// <summary>
        /// Validates the specified room data.
        /// </summary>
        /// <param name="room">The room to validate.</param>
        /// <returns>True if the room is valid; otherwise, false.</returns>
        private async Task<bool> ValidateRoom(RoomModel room)
        {
            if (room.Capacity <= 0)
            {
                this.ErrorMessage = "Please enter a number >0 for the capacity.";
                return false;
            }

            if (!await this.roomModel.DoesDepartmentExistAsync(room.DepartmentID))
            {
                this.ErrorMessage = "DepartmentID doesn’t exist in the Departments Records.";
                return false;
            }

            if (!await this.roomModel.DoesEquipmentExistAsync(room.EquipmentID))
            {
                this.ErrorMessage = "EquipmentID doesn’t exist in the Equipments Records.";
                return false;
            }

            return true;
        }
    }
}