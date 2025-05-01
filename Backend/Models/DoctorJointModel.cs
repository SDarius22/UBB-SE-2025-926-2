namespace Backend.Models
{
    public class DoctorJointModel
    {
        // Primary key
        public int DoctorId { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int DepartmentId { get; set; }

        // Properties from database
        public double Rating { get; set; }
        public string LicenseNumber { get; set; }

        // Navigation properties
        public virtual UserModel User { get; set; }
        public virtual DepartmentModel Department { get; set; }


        // Parameterless constructor required by EF Core
        public DoctorJointModel() { }

        // Constructor for application code
        public DoctorJointModel(int doctorId, int userId, int departmentId,
                              double rating, string licenseNumber)
        {
            DoctorId = doctorId;
            UserId = userId;
            DepartmentId = departmentId;
            Rating = rating;
            LicenseNumber = licenseNumber;
        }
    }
}