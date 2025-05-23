﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Hospital.Models;
using Hospital.Utils;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hospital.ApiClients;

namespace Hospital.ViewModels.AddViewModels
{
    /// <summary>
    /// ViewModel for adding equipment.
    /// </summary>
    public class EquipmentAddViewModel : INotifyPropertyChanged
    {
        private readonly EquipmentApiService equipmentModel;
        private string name = string.Empty;
        private string type = string.Empty;
        private string specification = string.Empty;
        private int stock;
        private string errorMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentAddViewModel"/> class.
        /// </summary>
        public EquipmentAddViewModel()
        {
            this.equipmentModel = App.Services.GetRequiredService<EquipmentApiService>();
            this.SaveEquipmentCommand = new RelayCommand(this.SaveEquipment);
            this.LoadEquipments();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the collection of equipment.
        /// </summary>
        public ObservableCollection<EquipmentModel> Equipments { get; set; } = new ObservableCollection<EquipmentModel>();

        /// <summary>
        /// Gets or sets the name of the equipment.
        /// </summary>
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }

        /// <summary>
        /// Gets or sets the type of the equipment.
        /// </summary>
        public string Type
        {
            get => this.type;
            set
            {
                this.type = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
        }

        /// <summary>
        /// Gets or sets the specification of the equipment.
        /// </summary>
        public string Specification
        {
            get => this.specification;
            set
            {
                this.specification = value;
                this.OnPropertyChanged(nameof(this.Specification));
            }
        }

        /// <summary>
        /// Gets or sets the stock of the equipment.
        /// </summary>
        public int Stock
        {
            get => this.stock;
            set
            {
                this.stock = value;
                this.OnPropertyChanged(nameof(this.Stock));
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
                this.OnPropertyChanged(nameof(this.ErrorMessage));
            }
        }

        /// <summary>
        /// Gets the command to save the equipment.
        /// </summary>
        public ICommand SaveEquipmentCommand { get; }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Loads the equipment from the database.
        /// </summary>
        private async void LoadEquipments()
        {
            this.Equipments.Clear();
            var equipments = await this.equipmentModel.GetEquipmentsAsync();
            foreach (EquipmentModel equipment in equipments)
            {
                this.Equipments.Add(equipment);
            }
        }

        /// <summary>
        /// Saves the equipment to the database.
        /// </summary>
        private async void SaveEquipment()
        {
            var equipment = new EquipmentModel
            {
                EquipmentID = 0,
                Name = this.Name,
                Type = this.Type,
                Specification = this.Specification,
                Stock = this.Stock,
            };

            if (this.ValidateEquipment(equipment))
            {
                bool success = await this.equipmentModel.AddEquipmentAsync(equipment);
                this.ErrorMessage = success ? "Equipment added successfully" : "Failed to add equipment";
                if (success)
                {
                    this.LoadEquipments();
                }
            }
        }

        /// <summary>
        /// Validates the equipment.
        /// </summary>
        /// <param name="equipment">The equipment to validate.</param>
        /// <returns>True if the equipment is valid, otherwise false.</returns>
        private bool ValidateEquipment(EquipmentModel equipment)
        {
            if (string.IsNullOrEmpty(equipment.Name))
            {
                this.ErrorMessage = "Please enter the name of the equipment.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(equipment.Type))
            {
                this.ErrorMessage = "Please enter the type of the equipment.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(equipment.Specification))
            {
                this.ErrorMessage = "Please enter the specifications of the equipment.";
                return false;
            }

            if (equipment.Stock <= 0)
            {
                this.ErrorMessage = "Please enter a number >0 for the stock.";
                return false;
            }

            return true;
        }
    }
}