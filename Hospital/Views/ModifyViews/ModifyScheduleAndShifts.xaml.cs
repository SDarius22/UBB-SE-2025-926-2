namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A page that combines the Modify Schedule and Modify Shift views.
    /// </summary>
    public sealed partial class ModifyScheduleAndShifts : Page
    {
        public ModifyScheduleAndShifts()
        {
            this.InitializeComponent();
            this.ScheduleFrame.Navigate(typeof(ModifyScheduleView));
            this.ShiftFrame.Navigate(typeof(ModifyShiftView));
        }
    }
}
