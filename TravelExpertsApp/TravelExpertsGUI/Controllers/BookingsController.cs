using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TravelExpertsData;
using TravelExpertsGUI.Data;

namespace TravelExpertsGUI.Controllers
{
    public class BookingsController : Controller
    {
        TravelExpertsContext context = new TravelExpertsContext();

        /*public IActionResult Index()
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
        }*/

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

            var bookingDMO = createBookingDMO(id, bookingDMOdata);

            if (!ModelState.IsValid)
            {
                // If the model is invalid, return to the form view
                return View(bookingDMO);
            }

            // get package by id
            var package = context.Packages.Include(p => p.ProductSuppliers).Where(p => p.PackageId == id).FirstOrDefault();

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
                ProductSupplierId = package.ProductSuppliers.First().ProductSupplierId
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
            // get package by id
            var package = context.Packages.Include(p => p.ProductSuppliers).Where(p => p.PackageId == id).FirstOrDefault();

            // create new list of product suppliers for supplier name and product name
            List<ProductSupplierDMO> productSupplierDMO = new List<ProductSupplierDMO>();

            var suppliers = context.Suppliers.ToDictionary(s => s.SupplierId, s => s.SupName);
            var products = context.Products.ToDictionary(p => p.ProductId, p => p.ProdName);

            foreach (var ps in package.ProductSuppliers)
            {
                var productSupplier = new ProductSupplierDMO()
                {
                    SupName = suppliers.ContainsKey(ps.SupplierId) ? suppliers[ps.SupplierId] : "Unknown Supplier",
                    ProdName = products.ContainsKey(ps.ProductId) ? products[ps.ProductId] : "Unknown Product"
                };

                productSupplierDMO.Add(productSupplier);
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
                ProductSuppliers = productSupplierDMO,
                RegionId = bookingDMOdata?.RegionId ?? null,
                pkgClassId = bookingDMOdata?.pkgClassId ?? null,
                TripTypeId = bookingDMOdata?.TripTypeId ?? null,
                Travelers = bookingDMOdata?.Travelers ?? 0
            };

            return bookingDMO;
        }
    }
}
