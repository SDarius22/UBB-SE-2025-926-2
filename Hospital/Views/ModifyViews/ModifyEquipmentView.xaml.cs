namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.UpdateViewModels;
    using Hospital.DatabaseServices.Interfaces;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyEquipmentView : Page
    {
        private EquipmentUpdateViewModel viewModel;
        private IEquipmentDatabaseService _equipmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyEquipmentView"/> class.
        /// </summary>
        public ModifyEquipmentView()
        {
            this.InitializeComponent();
            this.viewModel = new EquipmentUpdateViewModel(_equipmentModel);
            this.DataContext = this.viewModel;
        }
    }
}