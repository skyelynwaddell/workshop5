using Microsoft.AspNetCore.Mvc;

namespace TravelExpertsGUI.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Book()
        {
            return View();
        }
    }
}
