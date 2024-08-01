using System.ComponentModel;

namespace TravelExpertsGUI.Data
{
    public class ProductSupplierDMO
    {
        [DisplayName("Supplier Name")]
        public string SupName { get; set; }

        [DisplayName("Product Name")]
        public string ProdName { get; set; }
    }
}
