using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityService.DTOs;
using IdentityService.Services;
using IdentityService.Enums;

namespace IdentityService.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IIdentityService identityService, ILogger<AdminController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        [HttpPost("doctors")]
        public async Task<ActionResult<UserDto>> CreateDoctor(RegisterUserDto model)
        {
            try
            {
                model.Role = UserRole.Doctor;
                var result = await _identityService.RegisterAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Creating doctor failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("assign-doctor")]
        public async Task<ActionResult> AssignDoctorToPatient(AssignDoctorDto model)
        {
            try
            {
                var result = await _identityService.AssignDoctorToPatientAsync(
                    model.PatientId, model.DoctorId);
                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Assigning doctor failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetDoctors()
        {
            var doctors = await _identityService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("patients")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetPatients()
        {
            var patients = await _identityService.GetPatientsAsync();
            return Ok(patients);
        }

        [HttpPost("deactivate/{userId}")]
        public async Task<ActionResult> DeactivateUser(string userId)
        {
            var result = await _identityService.DeactivateUserAsync(userId);
            return result ? Ok() : NotFound();
        }
    }
}