using Microsoft.AspNetCore.Mvc;

namespace TravelExpertsGUI.Controllers
{
    public class LoginRegisterController : Controller
    {
        /// <summary>
        /// Login [GET]
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login [POST]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoginAsync()
        {
            return View();
        }

        /// <summary>
        /// Logout [POST]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Logout()
        {
            return View();
        }

        /// <summary>
        /// Signup [GET]
        /// </summary>
        /// <returns></returns>
        public IActionResult Signup()
        {
            return View();
        }

        /// <summary>
        /// RegisterUser [POST]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RegisterUser()
        {
            return View();
        }
    }
}
