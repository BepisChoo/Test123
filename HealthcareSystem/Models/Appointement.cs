using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; } // Primary Key

        [Required]
        public int PatientId { get; set; } // Foreign Key to User
        [ForeignKey("PatientId")]
        public User Patient { get; set; }

        [Required]
        public int DoctorId { get; set; } // Foreign Key to User
        [ForeignKey("DoctorId")]
        public User Doctor { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentTime { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; } // e.g., "Pending", "Confirmed", "Completed"
    }
}