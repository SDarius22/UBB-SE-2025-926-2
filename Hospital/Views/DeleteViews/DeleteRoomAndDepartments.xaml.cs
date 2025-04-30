namespace Hospital.Views.DeleteViews
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A page that combines the Delete Rooms and Delete Departments views.
    /// </summary>
    public sealed partial class DeleteRoomAndDepartments : Page
    {
        public DeleteRoomAndDepartments()
        {
            this.InitializeComponent();
            this.RoomFrame.Navigate(typeof(DeleteRoomView));
            this.DepartmentFrame.Navigate(typeof(DeleteDepartmentView));
        }
    }
}

