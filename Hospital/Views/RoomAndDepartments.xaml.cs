namespace Hospital.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Models;
    using Hospital.DatabaseServices;
    using Hospital.DatabaseServices.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoomAndDepartments : Page
    {
        private readonly IRoomDatabaseService roomModel;
        private readonly IDepartmentsDatabaseService departmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomAndDepartments"/> class.
        /// </summary>
        public RoomAndDepartments()
        {
            roomModel = App.Services.GetRequiredService<IRoomDatabaseService>();
            departmentModel = App.Services.GetRequiredService<IDepartmentsDatabaseService>();
            this.InitializeComponent();
            this.Load();
        }

        /// <summary>
        /// Gets or Sets Rooms.
        /// </summary>
        public ObservableCollection<RoomModel> Rooms { get; set; } = new ();

        /// <summary>
        /// Gets or Sets Departments.
        /// </summary>
        public ObservableCollection<DepartmentModel> Departments { get; set; } = new ();

        private async void Load()
        {
            this.Departments.Clear();
            var list = await this.departmentModel.GetDepartments();
            foreach (DepartmentModel department in list)
            {
                this.Departments.Add(department);
            }

            this.Rooms.Clear();

            List<RoomModel>? rooms = this.roomModel.GetRooms().Result;
            if (rooms != null)
            {
                foreach (RoomModel room in rooms)
                {
                    this.Rooms.Add(room);
                }
            }
        }
    }
}
