using HospitalClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HospitalClient.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(DoctorRegistration doctor)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(doctor), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7094/api/Doctor/Login", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    doctor = JsonConvert.DeserializeObject<DoctorRegistration>(apiResponse);
                }
            }
            if (doctor != null)
            {
                HttpContext.Session.SetString("DoctorName", doctor.DoctorName);
                HttpContext.Session.SetString("DoctorId", doctor.DoctorId);

            }
            return RedirectToAction("Index", "Doctor");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Doctor");
        }
       
    }
}
