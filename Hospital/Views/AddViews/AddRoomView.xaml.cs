namespace Hospital.Views.AddViews
{
    using Microsoft.UI.Xaml.Controls;
    using Hospital.ViewModels.AddViewModels;

    /// <summary>
    /// Represents a page for adding a room. This page can be used either standalone or within a Frame navigation.
    /// It binds to the <see cref="RoomAddViewModel"/> to handle the logic and data for adding a new room.
    /// </summary>
    public sealed partial class AddRoomView : Page
    {
        private RoomAddViewModel viewModel;
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRoomView"/> class.
        /// Sets up the view model and data context for the page.
        /// </summary>
        public AddRoomView()
        {
            this.InitializeComponent();
            this.viewModel = new RoomAddViewModel();
            this.DataContext = this.viewModel;
        }
    }
}
