using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Models;
using IdentityService.Enums;

namespace IdentityService.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
            {
                throw new Exception("Username already exists");
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                Specialization = model.Specialization,
                MedicalHistory = model.MedicalHistory,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors)}");
            }

            await _userManager.AddToRoleAsync(user, model.Role.ToString());

            return MapToUserDto(user);
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new Exception("Invalid username or password");
            }

            if (!user.IsActive)
            {
                throw new Exception("Account is deactivated");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return MapToUserDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetDoctorsAsync()
        {
            var doctors = await _userManager.Users
                .Where(u => u.Role == UserRole.Doctor && u.IsActive)
                .ToListAsync();

            return doctors.Select(MapToUserDto);
        }

        public async Task<IEnumerable<UserDto>> GetPatientsAsync()
        {
            var patients = await _userManager.Users
                .Where(u => u.Role == UserRole.Patient && u.IsActive)
                .ToListAsync();

            return patients.Select(MapToUserDto);
        }

        public async Task<bool> AssignDoctorToPatientAsync(string patientId, string doctorId)
        {
            var doctorPatient = new DoctorPatient
            {
                DoctorId = doctorId,
                PatientId = patientId,
                AssignedDate = DateTime.UtcNow
            };

            _context.DoctorPatients.Add(doctorPatient);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetDoctorPatientsAsync(string doctorId)
        {
            var patients = await _context.DoctorPatients
                .Where(dp => dp.DoctorId == doctorId && dp.IsActive)
                .Select(dp => dp.Patient)
                .ToListAsync();

            return patients.Select(MapToUserDto);
        }

        public async Task<UserDto?> GetPatientDoctorAsync(string patientId)
        {
            var doctorPatient = await _context.DoctorPatients
                .Include(dp => dp.Doctor)
                .FirstOrDefaultAsync(dp => dp.PatientId == patientId && dp.IsActive);

            return doctorPatient?.Doctor != null ? MapToUserDto(doctorPatient.Doctor) : null;
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateUserAsync(string userId, RegisterUserDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Specialization = model.Specialization;
            user.MedicalHistory = model.MedicalHistory;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private static UserDto MapToUserDto(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                IsActive = user.IsActive
            };
        }
    }
}