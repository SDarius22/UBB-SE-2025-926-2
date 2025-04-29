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
            this.DepartmentFrame.Navigate(typeof(AddDepartmentView));
        }
    }
}
