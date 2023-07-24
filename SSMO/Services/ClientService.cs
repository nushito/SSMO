using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.ProductsStock;
using System.Collections.Generic;

namespace SSMO.Services
{
    public class ClientService
    {
        private static InvoiceDetailsViewModel _clientModel;
        private static IEnumerable<ProductAvailabilityDetailsViewModel> _productOnStock;
        public static void AddClient(InvoiceDetailsViewModel clientModel)
        {
            _clientModel = clientModel;
        }
        public static InvoiceDetailsViewModel GetClient()
        {
            return _clientModel;
        }

        public static void AddProductsOnStock(IEnumerable<ProductAvailabilityDetailsViewModel> productOnStockModel)
        {
            _productOnStock = productOnStockModel;
        }

        public static IEnumerable<ProductAvailabilityDetailsViewModel> GetProductOnStock()
        {
            return _productOnStock;
        }
    }
}
