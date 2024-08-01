using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using TravelExpertsData;
using System.ComponentModel.DataAnnotations;

namespace TravelExpertsGUI.Data
{
    public class BookingDMO
    {
        [DisplayName("Package ID")]
        public int? PackageId { get; set; }

        [DisplayName("Name")]
        public string? PkgName { get; set; }

        [DisplayName("Description")]
        public string? PkgDesc { get; set; }

        [DisplayName("Start Date")]
        public DateTime? PkgStartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? PkgEndDate { get; set; }

        [DisplayName("Base Price")]
        public decimal? PkgBasePrice { get; set; }

        [DisplayName("Regions")]
        public List<Region>? Regions { get; set; }

        public string? RegionId { get; set; }

        [DisplayName("Class")]
        public List<Class>? Classes { get; set; }

        public string? pkgClassId { get; set; }

        [DisplayName("Type of Trip")]
        public List<TripType>? TripTypes { get; set; }

        public string? TripTypeId { get; set; }

        [Required(ErrorMessage = "Number of travelers cannot be empty")]
        [Range(1, 6, ErrorMessage = "Number of travelers must be between 1 and 6.")]
        public int Travelers {  get; set; }

        [DisplayName("Supplied Products")]
        public List<ProductSupplierDMO>? ProductSuppliers { get; set; }
    }
}
