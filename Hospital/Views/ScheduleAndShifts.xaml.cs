namespace Hospital.Views
{
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Models;
    using Hospital.DatabaseServices;
    using System.Threading.Tasks;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///
    public sealed partial class ScheduleAndShifts : Page
    {
        private readonly ShiftsDatabaseService shiftModel = new ();
        private readonly ScheduleDatabaseService scheduleModel = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAndShifts"/> class.
        /// </summary>
        public ScheduleAndShifts()
        {
            this.InitializeComponent();
            this.LoadAsync();
        }

        /// <summary>
        /// Gets or Sets the Shifts.
        /// </summary>
        public ObservableCollection<ShiftModel> Shifts { get; set; } = new ();

        /// <summary>
        /// Gets or Sets the Schedules.
        /// </summary>
        public ObservableCollection<ScheduleModel> Schedules { get; set; } = new ();

        private async Task LoadAsync()
        {
            this.Shifts.Clear();
            var shifts = await this.shiftModel.GetShifts();
            foreach (ShiftModel shift in shifts)
            {
                this.Shifts.Add(shift);
            }

            this.Schedules.Clear();
            foreach (ScheduleModel schedule in this.scheduleModel.GetSchedules())
            {
                this.Schedules.Add(schedule);
            }
        }
    }
}
