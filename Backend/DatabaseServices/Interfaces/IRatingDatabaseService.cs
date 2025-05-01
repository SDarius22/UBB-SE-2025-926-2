using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml.Controls;
using Backend.Models;

namespace Backend.DatabaseServices.Interfaces
{
    public interface IRatingDatabaseService
    {
        Task<RatingModel?> FetchRating(int medicalRecordID);

        public Task<bool> AddRating(RatingModel rating);

        public Task<bool> RemoveRating(int ratingId);


    }
}
