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
            // Only navigate to the default view (Schedule) in constructor
            this.ScheduleFrame.Navigate(typeof(AddScheduleView));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var pivotItem = e.AddedItems[0] as PivotItem;
                if (pivotItem != null)
                {
                    // Only navigate if the frame hasn't been navigated to this type before
                    if (pivotItem.Header.ToString() == "Add Schedule" &&
                        this.ScheduleFrame.SourcePageType != typeof(AddScheduleView))
                    {
                        this.ScheduleFrame.Navigate(typeof(AddScheduleView));
                    }
                    else if (pivotItem.Header.ToString() == "Add Shift" &&
                             this.ShiftFrame.SourcePageType != typeof(AddShiftView))
                    {
                        this.ShiftFrame.Navigate(typeof(AddShiftView));
                    }
                }
            }
        }
    }
}