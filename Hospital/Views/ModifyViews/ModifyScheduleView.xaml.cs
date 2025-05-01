namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.UpdateViewModels;
    using Hospital.DatabaseServices.Interfaces;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyScheduleView : Page
    {
        private ScheduleUpdateViewModel viewModel;
        private IScheduleDatabaseService _scheduleModel;

        public ModifyScheduleView()
        {
            this.InitializeComponent();
            this.viewModel = new ScheduleUpdateViewModel(_scheduleModel);
            this.DataContext = this.viewModel;
        }
    }
}
