namespace Hospital.Views.DeleteViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.DeleteViewModels;
    using Hospital.DatabaseServices.Interfaces;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeleteScheduleView : Page
    {
        private ScheduleDeleteViewModel viewmodel;
        private IScheduleDatabaseService _scheduleModel;

        public DeleteScheduleView()
        {
            this.InitializeComponent();
            this.viewmodel = new ScheduleDeleteViewModel();
            this.DataContext = this.viewmodel;
        }
    }
}
