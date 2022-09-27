using HospitalClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
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
            doctor.DoctorName = "";
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(doctor), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7094/api/Doctor/Login", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //doctor = JsonConvert.DeserializeObject<DoctorRegistration>(apiResponse);
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
        [HttpGet]
        public async Task<IActionResult> Doctor(string DoctorId)
        {
            DoctorRegistration obj = new DoctorRegistration();
            TempData["DoctorId"] = DoctorId;

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7094/api/Doctor/DoctorId?DoctorId=" + DoctorId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    obj = JsonConvert.DeserializeObject<DoctorRegistration>(apiResponse);
                }
            }
            return View(obj);
        }

        public async Task<IActionResult> AppointmentDetails()
        {
            
            string DoctorId = HttpContext.Session.GetString("DoctorId");
            List<AppointmentBooking> AppointmentDetails = new(); 
            using (var httpClient = new HttpClient()) 
            { 
                using (var response = await httpClient.GetAsync(" https://localhost:7094/api/AppointmentBooking/DoctorId?DoctorId="+DoctorId))
                { 
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    AppointmentDetails = JsonConvert.DeserializeObject<List<AppointmentBooking>>(apiResponse); 
                }
            }
            return View(AppointmentDetails);
        }
    }
}
