using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalClient.Models
{
    public class DoctorRegistration
    {
        public DoctorRegistration()
        {
            AppointmentBookings = new HashSet<AppointmentBooking>();
        }
        public string DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? Qualification { get; set; }
        public int? SpecializationId { get; set; }
        [NotMapped]
        public string? SpecializationName { get; set; }
        public string? Position { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }

        [NotMapped]
        public string? ConfirmPassword { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        public virtual Specialization? Specialization { get; set; }
        public virtual ICollection<AppointmentBooking> AppointmentBookings { get; set; }

    }
}
