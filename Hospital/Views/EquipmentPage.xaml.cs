namespace Hospital.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Models;
    using Hospital.DatabaseServices;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EquipmentPage : Page
    {
        private readonly EquipmentDatabaseService equipmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentPage"/> class.
        /// </summary>
        public EquipmentPage()
        {
            this.InitializeComponent();
            this.LoadEquiptment();
        }

        /// <summary>
        /// Gets or Sets the Equipments.
        /// </summary>
        public ObservableCollection<EquipmentModel> Equipments { get; set; } = new ();

        private async void LoadEquiptment()
        {
            this.Equipments.Clear();
            var list = await this.equipmentModel.GetEquipments();

            foreach (EquipmentModel equipment in list)
            {
                this.Equipments.Add(equipment);
            }
        }
    }
}
