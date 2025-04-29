namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.UpdateViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyShiftView : Page
    {
        private ShiftUpdateViewModel viewModel;

        public ModifyShiftView()
        {
            this.InitializeComponent();
            this.viewModel = new ShiftUpdateViewModel();
            this.DataContext = this.viewModel;
        }
    }
}
