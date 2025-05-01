namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.UpdateViewModels;
    using Hospital.DatabaseServices;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyShiftView : Page
    {
        private ShiftUpdateViewModel viewModel;
        private IShiftsDatabaseService _shiftModel;

        public ModifyShiftView()
        {
            this.InitializeComponent();
            this.viewModel = new ShiftUpdateViewModel(_shiftModel);
            this.DataContext = this.viewModel;
        }
    }
}
