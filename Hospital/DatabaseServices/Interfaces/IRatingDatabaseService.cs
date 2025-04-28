using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml.Controls;
using Hospital.Models;

namespace Hospital.DatabaseServices
{
    public interface IRatingDatabaseService
    {
        public RatingModel? FetchRating(int medicalRecordID);

        public bool AddRating(RatingModel rating);

        public bool RemoveRating(int ratingId);


    }
}
