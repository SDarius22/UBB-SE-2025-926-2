namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.UpdateViewModels;
    

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyEquipmentView : Page
    {
        private EquipmentUpdateViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyEquipmentView"/> class.
        /// </summary>
        public ModifyEquipmentView()
        {
            this.InitializeComponent();
            this.viewModel = new EquipmentUpdateViewModel();
            this.DataContext = this.viewModel;
        }
    }
}