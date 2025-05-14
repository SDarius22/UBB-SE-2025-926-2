using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingDatabaseService _ratingService;

        public RatingController(IRatingDatabaseService ratingService)
        {
            _ratingService = ratingService;
        }

        // GET: api/rating/medicalrecord/5
        [HttpGet("medicalrecord/{medicalRecordId}")]
        [Authorize]
        public async Task<ActionResult<RatingModel>> GetRatingByMedicalRecord(int medicalRecordId)
        {
            try
            {
                var rating = await _ratingService.FetchRating(medicalRecordId);
                if (rating == null)
                    return NotFound($"No rating found for medical record ID {medicalRecordId}");

                return Ok(rating);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving rating: {ex.Message}");
            }
        }

        // POST: api/rating
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> AddRating([FromBody] RatingModel rating)
        {
            try
            {
                var success = await _ratingService.AddRating(rating);
                if (!success)
                    return BadRequest("Failed to add rating");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding rating: {ex.Message}");
            }
        }

        // DELETE: api/rating/5
        [HttpDelete("{ratingId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveRating(int ratingId)
        {
            try
            {
                var success = await _ratingService.RemoveRating(ratingId);
                if (!success)
                    return NotFound($"No rating found with ID {ratingId}");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error removing rating: {ex.Message}");
            }
        }
    }
}
