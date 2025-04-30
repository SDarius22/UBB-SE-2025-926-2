namespace Hospital.Views.AddViews
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A page that combines the Add Room and Add Department views.
    /// </summary>
    public sealed partial class AddRoomAndDepartments : Page
    {
        public AddRoomAndDepartments()
        {
            this.InitializeComponent();
            this.RoomFrame.Navigate(typeof(AddRoomView));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var pivotItem = e.AddedItems[0] as PivotItem;
                if (pivotItem != null)
                {
                    // Only navigate if the frame hasn't been navigated to this type before
                    if (pivotItem.Header.ToString() == "Add Room" &&
                        this.RoomFrame.SourcePageType != typeof(AddRoomView))
                    {
                        this.RoomFrame.Navigate(typeof(AddRoomView));
                    }
                    else if (pivotItem.Header.ToString() == "Add Department" &&
                             this.DepartmentFrame.SourcePageType != typeof(AddDepartmentView))
                    {
                        this.DepartmentFrame.Navigate(typeof(AddDepartmentView));
                    }
                }
            }
        }
    }
}
