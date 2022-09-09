using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalClient.Models
{
    public class AppointmentBooking
    {
        public int AppointmentId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? DoctorId { get; set; }

        public string? PatientId { get; set; }

        [NotMapped]
        public string? DoctorName { get; set; }
         
        [NotMapped]
        public string? PatientName { get; set; }
        public virtual DoctorRegistration? Doctor { get; set; }
        public virtual PatientRegistration? Patient { get; set; }

    }
}
