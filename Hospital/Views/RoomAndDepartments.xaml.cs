namespace Hospital.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Models;
    
    
    using Microsoft.Extensions.DependencyInjection;
    using Hospital.ApiClients;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoomAndDepartments : Page
    {
        private readonly RoomApiService roomModel;
        private readonly DepartmentsApiService departmentModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomAndDepartments"/> class.
        /// </summary>
        public RoomAndDepartments()
        {
            roomModel = App.Services.GetRequiredService<RoomApiService>();
            departmentModel = App.Services.GetRequiredService<DepartmentsApiService>();
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
            var list = await this.departmentModel.GetDepartmentsAsync();
            foreach (DepartmentModel department in list)
            {
                this.Departments.Add(department);
            }

            this.Rooms.Clear();
            List<RoomModel>? rooms = await this.roomModel.GetRoomsAsync();
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
