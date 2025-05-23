namespace Hospital.ViewModels.DeleteViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using Hospital.ApiClients;
    using Hospital.Models;
    using Hospital.Utils;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// ViewModel for deleting rooms. Handles room selection, validation, deletion, and feedback messages.
    /// </summary>
    public class RoomDeleteViewModel : INotifyPropertyChanged
    {
        private readonly RoomApiService roomModel;
        private ObservableCollection<RoomModel> rooms;
        private int roomID;
        private string errorMessage = string.Empty;
        private string messageColor = "Red";

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomDeleteViewModel"/> class.
        /// </summary>
        public RoomDeleteViewModel()
        {
            this.roomModel = App.Services.GetRequiredService<RoomApiService>();
            LoadRooms();
            this.DeleteRoomCommand = new RelayCommand(this.RemoveRoom);
        }


        private async void LoadRooms()
        {
            var list = await this.roomModel.GetRoomsAsync();
            this.Rooms = new ObservableCollection<RoomModel>(list);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the collection of rooms.
        /// </summary>
        public ObservableCollection<RoomModel> Rooms
        {
            get { return this.rooms; }
            set { this.SetProperty(ref this.rooms, value); }
        }

        /// <summary>
        /// Gets or sets the ID of the selected room.
        /// </summary>
        public int RoomID
        {
            get => this.roomID;
            set
            {
                this.roomID = value;
                this.OnPropertyChanged(nameof(this.RoomID));
                this.OnPropertyChanged(nameof(this.CanDeleteRoom));
            }
        }

        /// <summary>
        /// Gets or sets the message to display after a delete attempt.
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
        /// Gets or sets the color of the error or success message.
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
        /// Gets the command used to delete a room.
        /// </summary>
        public ICommand DeleteRoomCommand { get; }

        /// <summary>
        /// Gets a value indicating whether a room can be deleted.
        /// </summary>
        public bool CanDeleteRoom => this.RoomID > 0;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Determines whether a room can be deleted.
        /// </summary>
        private bool CanExecuteDeleteRoom()
        {
            return this.RoomID > 0;
        }

        /// <summary>
        /// Deletes the selected room if it exists.
        /// </summary>
        private async void RemoveRoom()
        {
            if (this.RoomID == 0)
            {
                this.ErrorMessage = "No room was selected";
                return;
            }

            if (!await this.roomModel.DoesRoomExistAsync(this.RoomID))
            {
                this.ErrorMessage = "RoomID doesn't exist in the records";
                return;
            }

            bool success = await this.roomModel.DeleteRoomAsync(this.RoomID);
            this.ErrorMessage = success ? "Room deleted successfully" : "Failed to delete room";

            if (success)
            {
                this.Rooms = new ObservableCollection<RoomModel>(await this.roomModel.GetRoomsAsync() ?? Enumerable.Empty<RoomModel>());
            }
        }

        /// <summary>
        /// Sets the field value and raises the <see cref="PropertyChanged"/> event if the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The backing field.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The name of the property. Automatically set by the compiler.</param>
        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                this.OnPropertyChanged(propertyName ?? throw new ArgumentNullException(nameof(propertyName)));
            }
        }
    }
}