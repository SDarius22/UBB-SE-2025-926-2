namespace Hospital.Views.DeleteViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Views.ViewModels.DeleteViewModels;
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeleteScheduleView : Page
    {
        private ScheduleDeleteViewModel viewmodel;

        public DeleteScheduleView()
        {
            this.InitializeComponent();
            this.viewmodel = new ScheduleDeleteViewModel();
            this.DataContext = this.viewmodel;
        }
    }
}
