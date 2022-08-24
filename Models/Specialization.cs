namespace HospitalClient.Models
{
    public class Specialization
    {
        public Specialization()
        {
            DoctorRegistrations = new HashSet<DoctorRegistration>();
        }

        public int SpecializationId { get; set; }
        public string? SpecializationName { get; set; }

        public virtual ICollection<DoctorRegistration>? DoctorRegistrations { get; set; }

       
    }
}
