namespace Hospital.Views.AddViews
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A page that combines the Add Schedule and Add Shift views.
    /// </summary>
    public sealed partial class AddScheduleAndShifts : Page
    {
        public AddScheduleAndShifts()
        {
            this.InitializeComponent();
            this.ScheduleFrame.Navigate(typeof(AddScheduleView));
            this.ShiftFrame.Navigate(typeof(AddShiftView));
        }
    }
}
