using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using TravelExpertsData;
using TravelExpertsGUI.Data;

namespace TravelExpertsGUI.Controllers
{
    /*
     * Author: Samuel Adeogun
     */
    public class BookingsController : Controller
    {
        private TravelExpertsContext _context { get; set; }

        public BookingsController(TravelExpertsContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult MyBookingDetails()
        {
            try
            {
                int? customerId = HttpContext.Session.GetInt32("CurrentCustomerID");
                if (customerId == null)
                {
                    RedirectToAction("Login", "Account");
                }

                (List<BookingDetail> myBooking, decimal totalCost) = BookingDetailManager.GetDetailsByCustomer(_context, (int)customerId!);
                ViewBag.TotalCost = totalCost;

                return View(myBooking);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Login", "Account");
            }
        }

        /*/*public IActionResult Index()
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

        public IActionResult Packages()
        {
            // get all packages
            var packages = context.Packages.Select(p => p);

            return View(packages);
        }

        public IActionResult Details(int id)
        {
            var bookingDMO = createBookingDMO(id);

            ViewBag.Id = id;

            return View(bookingDMO);
        }

        [HttpPost]
        public IActionResult Details(int id, BookingDMO bookingDMOdata)
        {
            // redirect user if not logged in

            // create booking dmo to send to view
            var bookingDMO = createBookingDMO(id, bookingDMOdata);

            if (!ModelState.IsValid)
            {
                // If the model is invalid, return to the form view
                return View(bookingDMO);
            }

            // get package by id
            var package = context.Packages.Include(p => p.PackagesProductsSuppliers).Where(p => p.PackageId == id).FirstOrDefault();

            // add booking with customerId
            var booking = new Booking()
            {
                BookingDate = DateTime.Now,
                CustomerId = 104, // Default value, will be grabbed from cookie later on
                PackageId = package.PackageId,
                TripTypeId = bookingDMO.TripTypeId,
                TravelerCount = bookingDMO.Travelers
            };

            // add booking
            context.Bookings.Add(booking);

            // save changes
            context.SaveChanges();

            var bookingDetails = new BookingDetail()
            {
                TripStart = package.PkgStartDate,
                TripEnd = package.PkgEndDate,
                BasePrice = package.PkgBasePrice,
                AgencyCommission = package.PkgAgencyCommission,
                BookingId = context.Bookings.Where(b => b.BookingDate == booking.BookingDate).FirstOrDefault().BookingId, // get new booking bookingId
                RegionId = bookingDMO.RegionId,
                ClassId = bookingDMO.pkgClassId,
                ProductSupplierId = package.PackagesProductsSuppliers.First().ProductSupplierId
            };

            // add bookingDetails with
            context.BookingDetails.Add(bookingDetails);

            context.SaveChanges();

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm(int id)
        {
            return View();
        }

        private BookingDMO createBookingDMO (int id, BookingDMO bookingDMOdata = null)
        {
            var package = context.Packages
                .Include(p => p.PackagesProductsSuppliers)
                .ThenInclude(pps => pps.ProductSupplier)
                .ThenInclude(ps => ps.Product)
                .FirstOrDefault(p => p.PackageId == id);

            // Create a new list of product suppliers for supplier name and product name
            List<ProductSupplierDMO> productSupplierDMOS = new List<ProductSupplierDMO>();

            // Fetch all suppliers and products if needed
            var productSuppliers = context.ProductsSuppliers
                .Include(ps => ps.Product)
                .Include(ps => ps.Supplier)
                .ToList();

            // Use dictionaries for fast lookups
            var products = productSuppliers
                .Select(ps => ps.Product)
                .Distinct()
                .ToDictionary(p => p.ProductId, p => p.ProdName);

            var suppliers = productSuppliers
                .Select(ps => ps.Supplier)
                .Distinct()
                .ToDictionary(s => s.SupplierId, s => s.SupName);

            foreach (var pps in package.PackagesProductsSuppliers)
            {
                var productSupplier = productSuppliers
                    .FirstOrDefault(ps => ps.ProductSupplierId == pps.ProductSupplierId);

                if (productSupplier != null)
                {
                    var productName = products.GetValueOrDefault(productSupplier.ProductId, "Unknown Product");
                    var supplierName = suppliers.GetValueOrDefault(productSupplier.SupplierId, "Unknown Supplier");

                    var productSupplierDMO = new ProductSupplierDMO()
                    {
                        ProdName = productName,
                        SupName = supplierName
                    };

                    productSupplierDMOS.Add(productSupplierDMO);
                }
            }


            // create dmo thingy???
            var bookingDMO = new BookingDMO()
            {
                PackageId = package.PackageId,
                PkgName = package.PkgName,
                PkgDesc = package.PkgName,
                PkgStartDate = package.PkgStartDate,
                PkgEndDate = package.PkgEndDate,
                PkgBasePrice = package.PkgBasePrice,
                Regions = context.Regions.ToList(),
                Classes = context.Classes.ToList(),
                TripTypes = context.TripTypes.ToList(),
                ProductSuppliers = productSupplierDMOS,
                RegionId = bookingDMOdata?.RegionId ?? null,
                pkgClassId = bookingDMOdata?.pkgClassId ?? null,
                TripTypeId = bookingDMOdata?.TripTypeId ?? null,
                Travelers = bookingDMOdata?.Travelers ?? 0
            };

            return bookingDMO;
        }*/
    }
}