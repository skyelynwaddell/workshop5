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

        /// <summary>
        /// Lists all packages
        /// </summary>
        /// <returns></returns>
        public IActionResult Packages()
        {
            // get all packages
            var packages = _context.Packages.Select(p => p);

            return View(packages);
        }

        /// <summary>
        /// Lists package details and get's booking specifics
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            // create booking view model
            var bookingDMO = createBookingDMO(id);

            ViewBag.Id = id;

            return View(bookingDMO);
        }

        /// <summary>
        /// Http Post for details method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookingDMOdata"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Details(int id, BookingDMO bookingDMOdata)
        {
            // redirect user if not logged in
            if (HttpContext.Session.GetInt32("CurrentCustomerID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // create booking dmo to send to view
            var bookingDMO = createBookingDMO(id, bookingDMOdata);

            if (!ModelState.IsValid)
            {
                // If the model is invalid, return to the form view
                return View(bookingDMO);
            }

            // get package by id
            var package = _context.Packages.Include(p => p.PackagesProductsSuppliers).Where(p => p.PackageId == id).FirstOrDefault();

            // add booking with customerId
            var booking = new Booking()
            {
                BookingDate = DateTime.Now,
                CustomerId = HttpContext.Session.GetInt32("CurrentCustomerID"),
                PackageId = package.PackageId,
                TripTypeId = bookingDMO.TripTypeId,
                TravelerCount = bookingDMO.Travelers
            };

            // add booking
            _context.Bookings.Add(booking);

            // save changes
            _context.SaveChanges();

            var bookingId = _context.Bookings.Where(b => b.BookingDate == booking.BookingDate).FirstOrDefault().BookingId;
            int? productSupplierId = package.PackagesProductsSuppliers.FirstOrDefault().ProductSupplierId;

            var bookingDetails = new BookingDetail()
            {
                TripStart = package.PkgStartDate,
                TripEnd = package.PkgEndDate,
                BasePrice = package.PkgBasePrice,
                AgencyCommission = package.PkgAgencyCommission,
                Description = package.PkgName,
                BookingId = bookingId, // get new booking bookingId
                RegionId = bookingDMO.RegionId,
                ClassId = bookingDMO.pkgClassId,
                ProductSupplierId = productSupplierId ?? 1
            };

            // add bookingDetails
            _context.BookingDetails.Add(bookingDetails);

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Successfully booked new package";
            return RedirectToAction("Bookings", "Account");
        }

        /// <summary>
        /// Create booking view model
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <param name="bookingDMOdata">Current Booking DMO data or default is null</param>
        /// <returns></returns>
        private BookingDMO createBookingDMO (int id, BookingDMO bookingDMOdata = null)
        {
            // get package
            var package = _context.Packages
                .Include(p => p.PackagesProductsSuppliers)
                .ThenInclude(pps => pps.ProductSupplier)
                .ThenInclude(ps => ps.Product)
                .FirstOrDefault(p => p.PackageId == id);

            // Create a new list of product suppliers for supplier name and product name
            List<ProductSupplierDMO> productSupplierDMOS = new List<ProductSupplierDMO>();

            // Fetch all suppliers and products if needed
            var productSuppliers = _context.ProductsSuppliers
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

            // add each product supplier to new list for booking view model as product supplier view model
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


            // create booking dmo
            var bookingDMO = new BookingDMO()
            {
                PackageId = package.PackageId,
                PkgName = package.PkgName,
                PkgDesc = package.PkgName,
                PkgStartDate = package.PkgStartDate,
                PkgEndDate = package.PkgEndDate,
                PkgBasePrice = package.PkgBasePrice,
                Regions = _context.Regions.ToList(),
                Classes = _context.Classes.ToList(),
                TripTypes = _context.TripTypes.ToList(),
                ProductSuppliers = productSupplierDMOS,
                RegionId = bookingDMOdata?.RegionId ?? null,
                pkgClassId = bookingDMOdata?.pkgClassId ?? null,
                TripTypeId = bookingDMOdata?.TripTypeId ?? null,
                Travelers = bookingDMOdata?.Travelers ?? 0
            };

            return bookingDMO;
        }
    }
}