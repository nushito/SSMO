using System.Collections.Generic;

namespace SSMO.Models.Suppliers
{
    public class EditSupplierViewModel
    {
        public string SupplierName { get; set; }
        public IEnumerable<string> SupplierNames { get; set; }
        public EditSupplierFormModel SupplierForEdit { get; set; }
    }
}
