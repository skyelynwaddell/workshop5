using Microsoft.AspNetCore.Mvc;

namespace TravelExpertsGUI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
