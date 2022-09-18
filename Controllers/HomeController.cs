using HospitalClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace HospitalClient.Controllers
{
    public class HomeController : Controller
    {

        #region AUTOREDIRECT
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var canAcess = false;

                // check the refer
                var referer = filterContext.HttpContext.Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    var rUri = new System.UriBuilder(referer).Uri;
                    var req = filterContext.HttpContext.Request;
                    if (req.Host.Host == rUri.Host && req.Host.Port == rUri.Port && req.Scheme == rUri.Scheme)
                    {
                        canAcess = true;
                    }
                }

                // ... check other requirements

                if (!canAcess)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
                }
            }
        }
        #endregion

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}