namespace Hospital.Views
{
    using System.Collections.ObjectModel;
    using Microsoft.UI.Xaml.Controls;
    using Hospital.Models;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Hospital.ApiClients;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///
    public sealed partial class ScheduleAndShifts : Page
    {
        private readonly ShiftsApiService shiftModel;
        private readonly ScheduleApiService scheduleModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAndShifts"/> class.
        /// </summary>
        public ScheduleAndShifts()
        {
            shiftModel = App.Services.GetRequiredService<ShiftsApiService>();
            scheduleModel = App.Services.GetRequiredService<ScheduleApiService>();
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
            var shifts = await this.shiftModel.GetShiftsAsync();
            foreach (ShiftModel shift in shifts)
            {
                this.Shifts.Add(shift);
            }

            this.Schedules.Clear();
            var schedules = await this.scheduleModel.GetSchedulesAsync();
            foreach (ScheduleModel schedule in schedules)
            {
                this.Schedules.Add(schedule);
            }
        }
    }
}
