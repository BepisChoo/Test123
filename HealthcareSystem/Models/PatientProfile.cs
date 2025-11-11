using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthcareSystem.Models;

namespace HealthcareSystem.Models
{
    public class PatientProfile
    {
        [Key]
        public int PatientId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? BloodType { get; set; }

        [StringLength(500)]
        public string? MedicalHistory { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }


        public virtual User User { get; set; }


    }
}