using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDatabaseService _userService;
        private TokenProvider tokenProvider;

        public UserController(IUserDatabaseService userService)
        {
            _userService = userService;
            tokenProvider = new TokenProvider();
        }

        // GET: api/user/check-role/5?role=doctor
        [HttpGet("check-role/{userId}")]
        [Authorize]
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

        // GET: api/user/login
        [HttpGet("login")]
        public async Task<ActionResult<string>> Login([FromQuery] string username, [FromQuery] string password)
        {
            int userId = await _userService.GetUserId(username, password);

            string token = "";

            if (userId != 0)
            {
                token = tokenProvider.Create(userId);
                return Ok(token);
            }

            return BadRequest("user doesn't exist");
        }
    }
}
