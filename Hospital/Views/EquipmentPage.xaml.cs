namespace Hospital.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Models;
    
    
    using Microsoft.Extensions.DependencyInjection;
    using Hospital.ApiClients;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EquipmentPage : Page
    {
        private readonly EquipmentApiService _equipmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentPage"/> class.
        /// </summary>
        public EquipmentPage()
        {
            _equipmentModel = App.Services.GetRequiredService<EquipmentApiService>();
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
            var list = await this._equipmentModel.GetEquipmentsAsync();

            foreach (EquipmentModel equipment in list)
            {
                this.Equipments.Add(equipment);
            }
        }
    }
}
