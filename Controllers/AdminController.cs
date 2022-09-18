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
                if (admin != null)
                {
                    HttpContext.Session.SetString("Name", admin.Name);
                    HttpContext.Session.SetString("AdminId", admin.AdminId);
                    TempData["Success"] = "Logged in!";
                }
                else
                {
                    TempData["Message"] = "Log in Failed! Please Check your Username and Password!";
                    return RedirectToAction("Login","Admin");
                }
            }

            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logged out!";
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
            TempData["Mes"] = "Registered Successfully and Email has been Sent!";

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
            var senderEmail = new MailAddress("hospitalgrace1@gmail.com", "Admin-Grace Hospitals");
            var receiverEmail = new MailAddress(doctor.DoctorId, "Receiver");
            var password = "dennxqcxrdmtoqeg";
            var sub = "Hello " + doctor.DoctorName;
            var body = "Hello " + doctor.DoctorName + "! Your User Id is your EmailId "  + " .Your Password is " + doctor.Password + ". Use these Credentials to Login.";

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
        public async Task<IActionResult> DoctorDetails()

        {
            List<DoctorRegistration> DoctorDetails = new List<DoctorRegistration>();


            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("https://localhost:7094/api/Doctor/Details");

                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    DoctorDetails = JsonConvert.DeserializeObject<List<DoctorRegistration>>(Response);

                }
                return View(DoctorDetails);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditDoctor(string DoctorId)
        {
            TempData["DoctorId"] = DoctorId;
            DoctorRegistration doctor = new DoctorRegistration();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7094/api/Doctor/DoctorId?DoctorId=" + DoctorId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    doctor = JsonConvert.DeserializeObject<DoctorRegistration>(apiResponse);
                }
            }
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
            var client1 = specialization.Select(s => new { Text = s.SpecializationName, Value = s.SpecializationId }).ToList();
            ViewBag.ClientsList1 = new SelectList(client1, "Value", "Text");
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(DoctorRegistration doctor)
        {
            DoctorRegistration obj = new DoctorRegistration();
            using (var httpClient = new HttpClient())
            {
                string DoctorId = doctor.DoctorId;
                StringContent content = new StringContent(JsonConvert.SerializeObject(doctor), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("https://localhost:7094/api/Doctor/" + DoctorId, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    obj = JsonConvert.DeserializeObject<DoctorRegistration>(apiResponse);
                }
            }
            return RedirectToAction("DoctorDetails");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteDoctor(string DoctorId)
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

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(DoctorRegistration doctor)
        {
            string DoctorId = Convert.ToString(TempData["DoctorId"]);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7094/api/Doctor/" + DoctorId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("DoctorDetails");
        }
        public async Task<IActionResult> AppointmentDetails()

        {
            List<AppointmentBooking> AppointmentDetails = new List<AppointmentBooking>();


            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("https://localhost:7094/api/AppointmentBooking/Details");

                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    AppointmentDetails = JsonConvert.DeserializeObject<List<AppointmentBooking>>(Response);

                }
                return View(AppointmentDetails);
            }
        }
            public async Task<IActionResult> PatientDetails()

            {
                List<PatientRegistration> PatientDetail = new List<PatientRegistration>();


                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("https://localhost:7094/api/Patient/Details");

                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        PatientDetail = JsonConvert.DeserializeObject<List<PatientRegistration>>(Response);

                    }
                    return View(PatientDetail);
                }
            }
    }
}
