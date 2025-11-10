using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareSystem.Models
{
    public class DoctorProfile
    {
        [Key]
        public int DoctorId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }

        [StringLength(50)]
        public string? LicenseNumber { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public int YearsOfExperience { get; set; }

        public virtual User User { get; set; }
    }
}