namespace Hospital.Views.DeleteViews
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A page that combines the Delete Schedules and Delete Shifts views.
    /// </summary>
    public sealed partial class DeleteSchedulesAndShifts : Page
    {
        public DeleteSchedulesAndShifts()
        {
            this.InitializeComponent();
            this.ScheduleFrame.Navigate(typeof(DeleteScheduleView));
            this.ShiftFrame.Navigate(typeof(DeleteShiftView));
        }
    }
}

