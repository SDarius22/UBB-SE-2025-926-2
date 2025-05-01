namespace Hospital.Views.AddViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.AddViewModels;
    using Hospital.DatabaseServices.Interfaces;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddScheduleView : Page
    {
        private ScheduleAddViewModel viewModel;
        private IScheduleDatabaseService _scheduleModel;
        public AddScheduleView()
        {
            this.InitializeComponent();
            this.viewModel = new ScheduleAddViewModel(_scheduleModel);
            this.DataContext = this.viewModel;
        }
    }
}
