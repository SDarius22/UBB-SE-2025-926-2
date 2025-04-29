namespace Hospital.Views.ModifyViews
{
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// A page that combines the Modify Room and Modify Department views.
    /// </summary>
    public sealed partial class ModifyRoomAndDepartments : Page
    {
        public ModifyRoomAndDepartments()
        {
            this.InitializeComponent();
            this.RoomFrame.Navigate(typeof(ModifyRoomView));
            this.DepartmentFrame.Navigate(typeof(ModifyDepartmentView));
        }
    }
}
