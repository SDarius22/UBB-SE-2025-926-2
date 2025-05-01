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
