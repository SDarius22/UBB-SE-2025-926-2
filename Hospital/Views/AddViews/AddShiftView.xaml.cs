namespace Hospital.Views.AddViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.AddViewModels;
    using Hospital.DatabaseServices;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddShiftView : Page
    {
        private ShiftAddViewModel viewModel;
        private IShiftsDatabaseService _shiftModel;
        public AddShiftView()
        {
            this.InitializeComponent();
            this.viewModel = new ShiftAddViewModel(_shiftModel);
            this.DataContext = this.viewModel;
        }
    }
}