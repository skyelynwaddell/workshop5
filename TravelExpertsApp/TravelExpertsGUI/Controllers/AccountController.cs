using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpertsData;

namespace TravelExpertsGUI.Controllers
{
    public class AccountController : Controller
    {
        TravelExpertsContext context = new TravelExpertsContext();

        public IActionResult Index()
        {
            // Add verification

            // get all bookings linked to customers
            int id = 1;
            var bookings = context.Bookings.Where(b => b.CustomerId == id);

            return View(bookings);
        }

        // 
        public IActionResult Bookings(int id)
        {
            // get booking
            var booking = context.Bookings.Where(p => p.BookingId == id).FirstOrDefault();

            return View(booking);
        }
    }
}
