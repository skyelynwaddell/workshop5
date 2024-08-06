using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpertsData;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using TravelExpertsGUI.Models;

namespace TravelExpertsGUI.Controllers
{
    /// <summary>
    /// Controller for handling account-related actions such as login, logout, signup, and profile management.
    /// </summary>
    public class AccountController : Controller
    {
        private TravelExpertsContext _context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public AccountController(TravelExpertsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after login.</param>
        /// <returns>The login view.</returns>
        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <param name="returnUrl">The URL to return to after login. Defaults to "/Home/Index".</param>
        /// <returns>The login view.</returns>
        public IActionResult Login(string returnUrl = "/Home/Index")
        {
            // If a return URL is provided, store it in TempData for later use
            if (returnUrl != null)
            {
                TempData["returnUrl"] = returnUrl;
            }

            // Return the login view
            return View();
        }

        /// <summary>
        /// Handles the login form submission.
        /// </summary>
        /// <param name="customer">The customer login details.</param>
        /// <returns>A redirect to the appropriate page based on login success or failure.</returns>
        [HttpPost]
        public async Task<IActionResult> LoginAsync(Customer customer)
        {
            // Authenticate the user with the provided credentials
            Customer cust = CustomerManager.Authenticate(_context, customer.CustUsername, customer.CustPassword);

            // If authentication fails, redirect back to the login page with an error message
            if (cust == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return RedirectToAction("Login", "Account");
            }

            // Store the customer ID in the session
            HttpContext.Session.SetInt32("CurrentCustomerID", cust.CustomerId);

            // Create a list of claims representing the user's identity
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, cust.CustUsername),
            };

            // Create a claims identity using the list of claims and the authentication scheme
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Create a claims principal using the claims identity
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in the user using the claims principal
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // If a return URL is provided, redirect the user to that URL
            if (string.IsNullOrEmpty((TempData["returnUrl"]?.ToString())))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(TempData["returnUrl"]!.ToString()!);
            }
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>A redirect to the home page.</returns>
        public async Task<IActionResult> LogoutAsync()
        {
            // Sign out the user by removing their authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Upon logout, remove customer ID from session
            // This is done to ensure that the session does not persist after the user logs out
            HttpContext.Session.Remove("CurrentCustomerID");
            HttpContext.Session.Clear(); 
            // Redirect the user to the home page after logout
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the signup page.
        /// </summary>
        /// <returns>The signup view.</returns>
        public IActionResult Signup()
        {
            return View();
        }

        /// <summary>
        /// Handles the signup form submission.
        /// </summary>
        /// <param name="model">The registration details.</param>
        /// <returns>A redirect to the login page on success, or the signup page on failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Signup(RegisterViewModel model)
        {
            try
            {
                // Check if the username already exists
                bool usernameExists = _context.Customers.Any(c => c.CustUsername == model.CustUsername);
                if (usernameExists)
                {
                    // If the username already exists, redirect back to the signup page with an error message
                    TempData["ErrorMessage"] = "Username already exists.";
                    return RedirectToAction("Signup", "Account");
                }

                if (ModelState.IsValid)
                {
                    // If the model is valid, create a new customer with the provided registration details
                    var newCustomer = new Customer
                    {
                        CustFirstName = model.CustFirstName!,
                        CustLastName = model.CustLastName!,
                        CustAddress = model.CustAddress!,
                        CustCity = model.CustCity!,
                        CustProv = model.CustProv!,
                        CustPostal = model.CustPostal!,
                        CustCountry = model.CustCountry!,
                        CustHomePhone = model.CustHomePhone!,
                        CustBusPhone = model.CustBusPhone!,
                        CustEmail = model.CustEmail!,
                        CustUsername = model.CustUsername!,
                        CustPassword = model.CustPassword!
                    };
                    CustomerManager.Register(_context, newCustomer);
                    TempData["SuccessMessage"] = "Signup successful. Please log in.";
                    // Redirect to the login page after successful signup
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception error)
            {
                // If an error occurs during signup, redirect back to the signup page with an error message
                TempData["ErrorMessage"] = "Error while signing up! Please try again.";
                return RedirectToAction("Login", "Account");
            }

            // If the model is invalid or an error occurs during signup, return the signup view
            return View();
        }
        
        
        /// <summary>
        /// Displays the profile page.
        /// </summary>
        /// <returns>The profile view.</returns>
        public IActionResult Profile()
        {
            // Retrieve the ID of the currently logged-in customer from the session
            var customerId = HttpContext.Session.GetInt32("CurrentCustomerID");

            // If no customer is logged in, redirect to the login page
            if (customerId == null)
            {
                return RedirectToAction("Login");
            }

            // Find the customer with the retrieved ID
            var customer = CustomerManager.Find(_context, customerId.Value);

            // If the customer is not found, redirect to the login page with an error message
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("Login");
            }

            // Create a new instance of the UpdateViewModel class with the customer's details
            var model = new UpdateViewModel()
            {
                CustUsername = customer.CustUsername,
                CustEmail = customer.CustEmail,
                CustFirstName = customer.CustFirstName,
                CustLastName = customer.CustLastName,
                CustAddress = customer.CustAddress,
                CustCity = customer.CustCity,
                CustProv = customer.CustProv,
                CustPostal = customer.CustPostal,
                CustCountry = customer.CustCountry,
                CustHomePhone = customer.CustHomePhone,
                CustBusPhone = customer.CustBusPhone,
            };

            // Return the profile view with the populated model
            return View(model);
        }

        /// <summary>
        /// Handles the profile update form submission.
        /// </summary>
        /// <param name="model">The updated profile details.</param>
        /// <returns>A redirect to the home page on success, or the profile page on failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateViewModel model)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Retrieve the ID of the currently logged-in customer from the session
                var customerId = HttpContext.Session.GetInt32("CurrentCustomerID");

                // If no customer is logged in, redirect to the login page
                if (customerId == null)
                {
                    return RedirectToAction("Login");
                }

                // Find the customer with the retrieved ID
                var customer = CustomerManager.Find(_context, customerId.Value);

                // If the customer is not found, redirect to the login page with an error message
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Profile not found.";
                    return RedirectToAction("Login");
                }

                // Update customer properties with values from the form
                customer.CustFirstName = model.CustFirstName!;
                customer.CustLastName = model.CustLastName!;
                customer.CustAddress = model.CustAddress!;
                customer.CustCity = model.CustCity!;
                customer.CustProv = model.CustProv!;
                customer.CustPostal = model.CustPostal!;
                customer.CustCountry = model.CustCountry!;
                customer.CustHomePhone = model.CustHomePhone!;
                customer.CustBusPhone = model.CustBusPhone;
                customer.CustEmail = model.CustEmail;
                customer.CustUsername = model.CustUsername!;
                // No password change for now

                // Update the customer in the database
                CustomerManager.Update(_context, customer);

                // Update the authentication cookie to reflect the updated profile in navbar
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, customer.CustUsername),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                // Set a success message for the user
                TempData["SuccessMessage"] = "Profile updated successfully.";

                // Redirect the user to the home page after successful update
                return RedirectToAction("Index", "Home");
            }

            // If model state is invalid, return to the profile page with errors
            return View(model);
        }
    }
}