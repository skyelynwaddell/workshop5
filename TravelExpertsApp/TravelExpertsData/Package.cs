using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TravelExpertsData;

public partial class Package
{
    [Key]
    public int PackageId { get; set; }

    [StringLength(50)]
    [DisplayName("Name")]
    public string PkgName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    [DisplayName("Start Date")]
    public DateTime? PkgStartDate { get; set; }

    [Column(TypeName = "datetime")]
    [DisplayName("End Date")]
    public DateTime? PkgEndDate { get; set; }

    [StringLength(50)]
    [DisplayName("Description")]
    public string? PkgDesc { get; set; }

    [Column(TypeName = "money")]
    [DisplayName("Price")]
    public decimal PkgBasePrice { get; set; }

    [Column(TypeName = "money")]
    public decimal? PkgAgencyCommission { get; set; }

    [InverseProperty("Package")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Package")]
    public virtual ICollection<PackagesProductsSupplier> PackagesProductsSuppliers { get; set; } = new List<PackagesProductsSupplier>();
}
