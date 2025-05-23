using System.ComponentModel.DataAnnotations;
using IdentityService.Enums;

namespace IdentityService.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        public UserRole Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Specialization { get; set; }
        public string? MedicalHistory { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public UserRole Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; }
    }

    public class AssignDoctorDto
    {
        [Required]
        public string PatientId { get; set; } = null!;
        
        [Required]
        public string DoctorId { get; set; } = null!;
    }
}