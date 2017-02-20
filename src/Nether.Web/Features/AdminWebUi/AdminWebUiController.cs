using Microsoft.AspNetCore.Mvc;

namespace Nether.Web.Features.AdminWebUi
{
    [Route("admin")]
    public class AdminWebUiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}