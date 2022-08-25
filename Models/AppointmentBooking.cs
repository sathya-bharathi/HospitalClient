using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalClient.Models
{
    public class AppointmentBooking
    {


        public int AppointmentId { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString ="{0:dd-MMM-yyyy}",ApplyFormatInEditMode =true)]
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? DoctorId { get; set; }

        public string? PatientId { get; set; }

        [NotMapped]
        public string? DoctorName { get; set; }
         
        [NotMapped]
        public string? PatientName { get; set; }

    }
}
