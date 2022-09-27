

using HospitalClient.Models;

namespace HospitalAPI.Token
{
    public class StudentToken
    {
        public Admin? admin { get; set; }
        public string? adminToken { get; set; }
    }
}