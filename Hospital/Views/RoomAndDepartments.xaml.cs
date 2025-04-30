namespace Hospital.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Models;
    using Hospital.DatabaseServices;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoomAndDepartments : Page
    {
        private readonly RoomDatabaseService roomModel = new ();
        private readonly DepartmentsDatabaseService departmentModel = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomAndDepartments"/> class.
        /// </summary>
        public RoomAndDepartments()
        {
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

        private void Load()
        {
            this.Departments.Clear();
            foreach (DepartmentModel department in this.departmentModel.GetDepartments())
            {
                this.Departments.Add(department);
            }

            this.Rooms.Clear();

            List<RoomModel>? rooms = this.roomModel.GetRooms();
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
