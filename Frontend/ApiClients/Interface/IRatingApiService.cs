using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IRatingApiService
{
    public Task<RatingModel> GetRatingByMedicalRecordAsync(int medicalRecordId);

    public Task<bool> AddRatingAsync(RatingModel rating);

    public Task<bool> RemoveRatingAsync(int ratingId);
}