using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorInformationController : ControllerBase
    {
        private readonly IDoctorInformationDatabaseService _doctorInfoService;

        public DoctorInformationController(IDoctorInformationDatabaseService doctorInfoService)
        {
            _doctorInfoService = doctorInfoService;
        }

        // GET: api/doctorinformation/{doctorId}
        [HttpGet("{doctorId}")]
        [Authorize]
        public async Task<ActionResult<DoctorInformationModel>> GetDoctorInformation(int doctorId)
        {
            var info = await _doctorInfoService.GetDoctorInformation(doctorId);
            if (info == null)
                return NotFound("Doctor information not found");

            return Ok(info);
        }
    }
}
