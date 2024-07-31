using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpertsData;

namespace TravelExpertsGUI.Controllers
{
    public class LoginRegisterController : Controller
    {
        private TravelExpertsContext _context { get; set; }

        public LoginRegisterController(TravelExpertsContext context)
        {
            _context = context;
        }

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
        public async Task<IActionResult> RegisterUser(
                string username, 
                string email, 
                string password, 
                string confirmpassword, 
                string address, 
                string firstname, 
                string lastname, 
                string busphone, 
                string postalcode, 
                string homephone, 
                string country, 
                string province, 
                string city
            )
        {
            try
            {
                // Validate input

                //check if any of the input fields are Empty or Null
                if (string.IsNullOrWhiteSpace(username) || 
                    string.IsNullOrWhiteSpace(password) || 
                    string.IsNullOrWhiteSpace(confirmpassword) || 
                    string.IsNullOrWhiteSpace(email) ||  
                    string.IsNullOrWhiteSpace(address) || 
                    string.IsNullOrWhiteSpace(city) || 
                    string.IsNullOrWhiteSpace(province) || 
                    string.IsNullOrWhiteSpace(homephone) 
                    )
                {
                    TempData["ErrorMessage"] = "All fields are required.";
                    return RedirectToAction("Signup", "LoginRegister");
                }

                //check if passwords match
                if (password != confirmpassword)
                {
                    TempData["ErrorMessage"] = "Passwords do not match.";
                    return RedirectToAction("Signup", "LoginRegister");
                }

                // Check if the username already exists
                bool usernameExists = _context.Customers.Any(c => c.CustUsername == username);
                if (usernameExists)
                {
                    TempData["ErrorMessage"] = "Username already exists.";
                    return RedirectToAction("Signup", "LoginRegister");
                }

                // Create a new customer
                var newCustomer = new Customer
                {
                    CustUsername =  username,
                    CustEmail = email,
                    CustPassword = password,
                    CustAddress = address,
                    CustFirstName = firstname,
                    CustLastName = lastname,
                    CustBusPhone = busphone,
                    CustPostal = postalcode,
                    CustHomePhone = homephone,
                    CustCountry = country,
                    CustProv = province,
                    CustCity = city
                };

                // Insert into database
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Signup successful. Please log in.";
                return RedirectToAction("Login", "LoginRegister");
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Error while signing up! Please try again.";
                return RedirectToAction("Login", "LoginRegister");
            }
        }
    }
}
