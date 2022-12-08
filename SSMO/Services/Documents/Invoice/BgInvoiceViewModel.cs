using System.Collections.Generic;

namespace SSMO.Services.Documents.Invoice
{
    public class BgInvoiceViewModel
    {
        public BGMyCompanyInvoicePrintViewModel BgMyCompany { get; set; }
        public BGCustomerForInvoicePrint BgCustomer { get; set; }
        public IEnumerable<BGProductsForBGInvoiceViewModel> BgProducts { get; set; }
    }
}
