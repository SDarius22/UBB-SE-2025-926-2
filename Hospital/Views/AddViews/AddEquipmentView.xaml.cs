namespace Hospital.Views.AddViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.AddViewModels;
    using Hospital.DatabaseServices.Interfaces;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEquipmentView : Page
    {
        private EquipmentAddViewModel viewModel;
        private IEquipmentDatabaseService _equipmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEquipmentView"/> class.
        /// </summary>
        public AddEquipmentView()
        {
            this.InitializeComponent();
            this.viewModel = new EquipmentAddViewModel(_equipmentModel);
            this.DataContext = this.viewModel;
        }
    }
}
