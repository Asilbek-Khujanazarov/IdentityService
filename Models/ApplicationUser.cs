using Microsoft.AspNetCore.Identity;
using IdentityService.Enums;

namespace IdentityService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ProfileImage { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public new string? PhoneNumber { get; set; }
        public string? Specialization { get; set; } // For doctors
        public string? MedicalHistory { get; set; } // For patients
    }
}