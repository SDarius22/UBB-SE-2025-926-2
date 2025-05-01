namespace Hospital.Views.DeleteViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.DeleteViewModels;
    using Hospital.DatabaseServices.Interfaces;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeleteEquipmentView : Page
    {
        private IEquipmentDatabaseService _equipmentModel;
        private EquipmentDeleteViewModel viewModel;
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteEquipmentView"/> class.
        /// </summary>
        public DeleteEquipmentView()
        {
            this.InitializeComponent();
            this.viewModel = new EquipmentDeleteViewModel();
            this.DataContext = this.viewModel;
        }
    }
}
