namespace Backend.Models
{
    public class TimeSlotModel
    {
        public DateTime TimeSlot { get; set; }
        public string Time { get; set; }
        public string Appointment { get; set; }
        public string HighlightStatus { get; set; }

        public TimeSlotModel()
        {
            HighlightStatus = "None";
        }
    }
}
