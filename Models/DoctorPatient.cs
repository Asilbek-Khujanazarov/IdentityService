namespace IdentityService.Models
{
    public class DoctorPatient
    {
        public string DoctorId { get; set; } = null!;
        public string PatientId { get; set; } = null!;
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; } = true;
        
        public virtual ApplicationUser Doctor { get; set; } = null!;
        public virtual ApplicationUser Patient { get; set; } = null!;
    }
}