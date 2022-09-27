using HospitalClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OpenXmlPowerTools;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using static HospitalClient.Controllers.HomeController;

namespace HospitalClient.Controllers
{
    public class PatientController : Controller
    {
        bool idGenerated = false;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(PatientRegistration patient)
        {
            patient.PhoneNumber = "";
            patient.PatientName = "";
            //patient.ConfirmPassword = patient.Password;
            //List<AppointmentBooking> a = new List<AppointmentBooking>();
            //patient.AppointmentBookings = a;
            using (var httpClient = new HttpClient())
            {

                StringContent content = new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7094/api/Patient/Login", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //patient = JsonConvert.DeserializeObject<PatientRegistration>(apiResponse);
                }
            }
            if (patient != null)
            {
                HttpContext.Session.SetString("PatientName", patient.PatientName);
                HttpContext.Session.SetString("PatientId", patient.PatientId);

            }
            return RedirectToAction("Index", "Patient");

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Patient");
        }
        public IActionResult PatientRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PatientRegister(PatientRegistration patient)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7094/api/Patient/Registration", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    patient = JsonConvert.DeserializeObject<PatientRegistration>(apiResponse);
                }
            }
            return RedirectToAction("Index", "Patient");
        }
        
        [HttpGet]
        public async Task<IActionResult> AppointmentSelectDoctor()
        {

            List<DoctorRegistration> doctors = new List<DoctorRegistration>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:7094/api/Doctor/Details");
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    doctors = JsonConvert.DeserializeObject<List<DoctorRegistration>>(Response);
                }
            }
            var doctorList = doctors.Select(s => new { Text = s.DoctorName, Value = s.DoctorId }).ToList();
            ViewBag.DoctorList = new SelectList(doctorList, "Value", "Text");
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AppointmentSelectDoctor(AppointmentBooking appointment)
        {

            if (appointment.AppointmentTime == null)
            {
                DoctorRegistration doctor = new();
                var DoctorID = appointment.DoctorId;
                using (var Client = new HttpClient())
                {
                    Client.DefaultRequestHeaders.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await Client.GetAsync("https://localhost:7094/api/Patient/DoctorId?DoctorId=" + DoctorID);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        doctor = JsonConvert.DeserializeObject<DoctorRegistration>(Response);
                    }
                }

                HttpContext.Session.SetString("DoctorName", doctor.DoctorName);
                HttpContext.Session.SetString("DoctorId", doctor.DoctorId);
                DateTime starttime = DateTime.ParseExact(doctor.StartTime, "HH.mm", null);
                DateTime endtime = DateTime.ParseExact(doctor.EndTime, "HH.mm", null);


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
                }

                    //List of Available Slots
                    
                    List<string> AvailableSlots = new();


                    while (starttime < endtime)
                    {
                        DateTime timeinterval1, timeinterval2;
                        timeinterval1 = starttime;

                        starttime = starttime.AddMinutes(30);

                        timeinterval2 = starttime;

                        if (starttime < endtime)
                        {
                            AvailableSlots.Add(timeinterval1.ToString("HH.mm") + " to " + timeinterval2.ToString("HH.mm"));

                        }
                        else
                        {
                            AvailableSlots.Add(timeinterval1.ToString("HH.mm") + " to " + endtime.ToString("HH.mm"));
                        }

                    }

                List<SelectListItem> item = AvailableSlots.ConvertAll(a =>
                {
                    return new SelectListItem()
                    {
                        Text = a.ToString(),
                        Value = a.ToString(),

                        Selected = false,

                    };
                });


                ViewBag.item = item;


            }
            else
            {
                // Step2 Completed -Appointment time selected

            
                using (var httpClient = new HttpClient())
                {
                   StringContent content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, "application/json");
                   using (var response = await httpClient.PostAsync("https://localhost:7094/api/Patient/BookAppointment", content))
                   {
                     string apiResponse = await response.Content.ReadAsStringAsync();
                     appointment = JsonConvert.DeserializeObject<AppointmentBooking>(apiResponse);
                   }
                    }

                    appointment.DoctorId = HttpContext.Session.GetString("DoctorId");
                    appointment.DoctorName = HttpContext.Session.GetString("DoctorName");
                    appointment.PatientId = HttpContext.Session.GetString("PatientId");
                    appointment.PatientName = HttpContext.Session.GetString("PatientName");

                    #region EMAIL
                    var senderEmail = new MailAddress("hospitalgrace1@gmail.com", "Admin-Grace Hospitals");
                    var receiverEmail = new MailAddress(appointment.PatientId, "Receiver");
                    var password = "dennxqcxrdmtoqeg";
                    var sub = " Appointment Details ";
                    var body = "Hello " + appointment.PatientName + "!  Your Appointment is confirmed.                The Appointment Details are: " + "                           " +
                        " Appointment Id: " + appointment.AppointmentId + "                                              Appointment Date: " + appointment.AppointmentDate
                        + "          Appointment Timing: " + appointment.AppointmentTime + "                            Doctor Name: Dr." + appointment.DoctorName + "                                                " +
                        "       Please make sure to be present at hospital before the Appointment Time. For further queries, Please Contact us! Telephone: 044-44431," +
                        "044-44432,044-44433";

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

                    return RedirectToAction("Index", "Patient");
                

            }

            return View(appointment);
        }
        public async Task<IActionResult> AppointmentDetails()
        {

            string PatientId = HttpContext.Session.GetString("PatientId");
            List<AppointmentBooking> AppointmentDetails = new();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(" https://localhost:7094/api/AppointmentBooking/PatientId?PatientId=" + PatientId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    AppointmentDetails = JsonConvert.DeserializeObject<List<AppointmentBooking>>(apiResponse);
                }
            }
            return View(AppointmentDetails);
        }
        public async Task<IActionResult> Details()
        {
           string PatientId = HttpContext.Session.GetString("PatientId");
           PatientRegistration details = new();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7094/api/Patient/PatientId?PatientId=" + PatientId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    details = JsonConvert.DeserializeObject<PatientRegistration>(apiResponse);
                }
            }
            return View(details);
        }
        [HttpGet]
        public async Task<IActionResult> EditPatient(string PatientId)
        {
            TempData["PatientId"] = PatientId;
            PatientRegistration patient = new();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7094/api/Patient/PatientId?PatientId=" + PatientId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    patient = JsonConvert.DeserializeObject<PatientRegistration>(apiResponse);
                }
            }
            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> EditPatient(PatientRegistration patient)
        {
            patient.PatientId = HttpContext.Session.GetString("PatientId");
            PatientRegistration obj = new PatientRegistration();
            using (var httpClient = new HttpClient())
            {
                string PatientId = patient.PatientId;
                StringContent content = new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("https://localhost:7094/api/Patient/" + PatientId, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    obj = JsonConvert.DeserializeObject<PatientRegistration>(apiResponse);
                }
            }
            return RedirectToAction("Details");
        }
    }
}


