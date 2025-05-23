using IdentityService.DTOs;

namespace IdentityService.Services
{
    public interface IIdentityService
    {
        Task<UserDto> RegisterAsync(RegisterUserDto model);
        Task<string> LoginAsync(LoginDto model);
        Task<UserDto> GetUserByIdAsync(string id);
        Task<IEnumerable<UserDto>> GetDoctorsAsync();
        Task<IEnumerable<UserDto>> GetPatientsAsync();
        Task<bool> AssignDoctorToPatientAsync(string patientId, string doctorId);
        Task<IEnumerable<UserDto>> GetDoctorPatientsAsync(string doctorId);
        Task<UserDto?> GetPatientDoctorAsync(string patientId);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> UpdateUserAsync(string userId, RegisterUserDto model);
    }
}