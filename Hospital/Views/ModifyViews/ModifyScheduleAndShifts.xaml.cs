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
            //this.ScheduleFrame.Navigate(typeof(ModifyScheduleView));
            this.ShiftFrame.Navigate(typeof(ModifyShiftView));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var pivotItem = e.AddedItems[0] as PivotItem;
                if (pivotItem != null)
                {
                    // Only navigate if the frame hasn't been navigated to this type before
                    if (pivotItem.Header.ToString() == "Modify Schedule" &&
                        this.ScheduleFrame.SourcePageType != typeof(ModifyScheduleView))
                    {
                        this.ScheduleFrame.Navigate(typeof(ModifyScheduleView));
                    }
                    else if (pivotItem.Header.ToString() == "Modify Shift" &&
                             this.ShiftFrame.SourcePageType != typeof(ModifyShiftView))
                    {
                        this.ShiftFrame.Navigate(typeof(ModifyShiftView));
                    }
                }
            }
        }
    }
}
