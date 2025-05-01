using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDatabaseService _userService;

        public UserController(IUserDatabaseService userService)
        {
            _userService = userService;
        }

        // GET: api/user/check-role/5?role=doctor
        [HttpGet("check-role/{userId}")]
        public async Task<ActionResult<bool>> CheckUserRole(int userId, [FromQuery] string role)
        {
            try
            {
                var hasRole = await _userService.UserExistsWithRole(userId, role);
                return Ok(hasRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking user role: {ex.Message}");
            }
        }
    }
}
