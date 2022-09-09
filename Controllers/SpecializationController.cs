using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using HospitalClient.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalClient.Controllers
{
    public class SpecializationController : Controller
    {
        public async Task<IActionResult> SpecializationDetails()

        {
            List<Specialization> Specializationdetails = new List<Specialization>();


            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("https://localhost:7094/api/Specialization/Specialization");

                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    Specializationdetails = JsonConvert.DeserializeObject<List<Specialization>>(Response);

                }
                return View(Specializationdetails);
            }
        }
        public async Task<IActionResult> Specialities()

        {
            List<Specialization> Specializationdetails = new List<Specialization>();


            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("https://localhost:7094/api/Specialization/Specialization");

                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    Specializationdetails = JsonConvert.DeserializeObject<List<Specialization>>(Response);

                }
                return View(Specializationdetails);
            }
        }
        public IActionResult AddSpecialization()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSpecialization(Specialization specialization)
        {
            Specialization splobj = new Specialization();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(specialization), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7094/api/Specialization", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    splobj = JsonConvert.DeserializeObject<Specialization>(apiResponse);
                }
            }
            return RedirectToAction("SpecializationDetails");
        }
        [HttpGet]
        public async Task<IActionResult> EditSpecialization(int SpecializationId)
        {
            TempData["SpecializationId"] = SpecializationId;
            Specialization splobj = new Specialization();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7094/api/Specialization/SpecializationId?SpecializationId=" + SpecializationId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    splobj = JsonConvert.DeserializeObject<Specialization>(apiResponse);
                }
            }
    
            return View(splobj);
        }

        [HttpPost]
        public async Task<IActionResult> EditSpecialization(Specialization specialization)
        {
            Specialization obj = new Specialization();
            using (var httpClient = new HttpClient())
            {
                int specializationId = specialization.SpecializationId;
                StringContent content = new StringContent(JsonConvert.SerializeObject(specialization), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("https://localhost:7094/api/Specialization/" + specializationId, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    obj = JsonConvert.DeserializeObject<Specialization>(apiResponse);
                }
            }
            return RedirectToAction("SpecializationDetails");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteSpecialization(int SpecializationId)
        {
            Specialization splobj = new Specialization();
            TempData["SpecializationId"] = SpecializationId;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7094/api/Specialization/SpecializationId?SpecializationId=" + SpecializationId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    splobj = JsonConvert.DeserializeObject<Specialization>(apiResponse);
                }
            }
            return View(splobj);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSpecialization(Specialization specialization)
        {
            int specializationId = Convert.ToInt32(TempData["SpecializationId"]);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7094/api/Specialization/" + specializationId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("SpecializationDetails");
        }
    }
}
