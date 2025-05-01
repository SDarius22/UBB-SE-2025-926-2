namespace Hospital.Views.ModifyViews
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using Hospital.ViewModels.UpdateViewModels;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Hospital.DatabaseServices.Interfaces;

    /// <summary>
    /// Represents a page for modifying an existing room. This page can be used either standalone or within a Frame navigation.
    /// It binds to the <see cref="RoomUpdateViewModel"/> to handle the logic and data for updating a room.
    /// </summary>
    public sealed partial class ModifyRoomView : Page
    {
        private RoomUpdateViewModel viewModel;
        private IRoomDatabaseService _roomModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyRoomView"/> class.
        /// Sets up the view model and data context for the page.
        /// </summary>
        public ModifyRoomView()
        {
            this.InitializeComponent();
            this.viewModel = new RoomUpdateViewModel(_roomModel);
            this.DataContext = this.viewModel;
        }
    }
}
