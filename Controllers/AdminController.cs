using HospitalClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;

namespace HospitalClient.Controllers
{
    public class AdminController : Controller
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
        public async Task<IActionResult> Login(Admin admin)
        {
            admin.Name = "";
            
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7094/api/Admin/Login", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    admin = JsonConvert.DeserializeObject<Admin>(apiResponse);
                }
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Admin");
        }
        public async Task<IActionResult> DoctorRegister()
        {
            List<Specialization> specialization = new List<Specialization>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:7094/api/Specialization/Specialization");
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    specialization = JsonConvert.DeserializeObject<List<Specialization>>(Response);
                }
            }
            var client1 = specialization.Select(s => new { Text =s.SpecializationName, Value = s.SpecializationId }).ToList();
            ViewBag.ClientsList1 = new SelectList(client1, "Value", "Text");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DoctorRegister(DoctorRegistration doctor)

        {
           
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(doctor), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7094/api/Admin/Registration", content))
                {
                    //string apiResponse = await response.Content.ReadAsStringAsync();
                    //doctor = JsonConvert.DeserializeObject<DoctorRegistration>(apiResponse);
                }
            }
            #region EMAIL
            var senderEmail = new MailAddress("librarymanagement13@gmail.com", "Sathya");
            var receiverEmail = new MailAddress(doctor.DoctorId, "Receiver");
            var password = "kigksgbmzemtqrax";
            var sub = "Hello " + doctor.DoctorName;
            var body = "Hello " + doctor.DoctorName + " Your User Id is your EmailId "  + " .Your Password is " + doctor.Password + " Use these Credentials to Login";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body
            })
            {
                smtp.Send(mess);
            }
            #endregion
            return RedirectToAction("Index", "Admin");
        }
        
    }
}
