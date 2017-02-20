using Microsoft.AspNetCore.Mvc;

namespace Nether.Web.Features.LandingPage
{
    public class LandingPageController : Controller
    {
        [HttpGet()]
        public IActionResult Index()
        {
            return View();
        }
    }
}