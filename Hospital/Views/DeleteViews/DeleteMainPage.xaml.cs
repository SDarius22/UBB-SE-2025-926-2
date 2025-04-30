using Microsoft.UI.Xaml.Controls;

namespace Hospital.Views.DeleteViews
{
    public sealed partial class DeleteMainPage : Page
    {
        public DeleteMainPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(DeleteDoctorView));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                switch (selectedItem.Tag)
                {
                    case "DeleteDoctors":
                        ContentFrame.Navigate(typeof(DeleteDoctorView));
                        break;
                    case "DeleteRooms":
                        ContentFrame.Navigate(typeof(DeleteRoomAndDepartments));
                        break;
                    case "DeleteSchedules":
                        ContentFrame.Navigate(typeof(DeleteSchedulesAndShifts));
                        break;
                    case "DeleteDrugs":
                        ContentFrame.Navigate(typeof(DeleteDrugView));
                        break;
                    case "DeleteEquipments":
                        ContentFrame.Navigate(typeof(DeleteEquipmentView));
                        break;
                }
            }
        }
    }
}