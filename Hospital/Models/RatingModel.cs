using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class RatingModel
    {
        public int RatingId { get; set; }
        public int MedicalRecordId { get; set; }
        public float NumberStars { get; set; }
        public string Motivation { get; set; }

        public RatingModel(int ratingId, int medicalRecordId, float numberStars, string motivation)
        {
            RatingId = ratingId;
            MedicalRecordId = medicalRecordId;
            NumberStars = numberStars;
            Motivation = motivation;
        }
    }
}
