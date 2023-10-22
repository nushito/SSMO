using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Documents.Purchase;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.Purchase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSMO.Services.Documents.Purchase
{
    public interface IPurchaseService
    {
        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrdersForPurchase(string
            supplierName, int currentpage, int supplierOrdersPerPage);

        public Task<bool> CreatePurchaseAsPerSupplierOrder(int id,
            string number, DateTime date, 
            decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses,
            int vat, string truckNumber, string swb, List<PurchaseProductAsSupplierOrderViewModel> products,
            string incoterms, string deliveryAddress, 
            string shippingLine, string eta, bool delayCostCalc, int costPriceCurrencyId);
       

        public PurchaseInvoiceDetailsViewModel PurchaseDetails(int id);

        public EditPurchaseViewModel PurchaseDetailsForEdit(int id);    
        public Task<bool> EditPurchaseInvoice(int id, string number,DateTime date, int supplierOrderId, 
            int vat, decimal netWeight, decimal grossWeight, string truckNumber, string swb, decimal purchaseTransportCost,
            decimal bankExpenses, decimal duty, decimal customsExpenses, decimal factoring, decimal fiscalAgentExpenses, 
            decimal procentComission, decimal otherExpenses, List<PurchaseProductsForEditFormModel> purchaseProducts,
            string deliveryAddress, string shippingLine, string eta);

        public List<PurchaseProductAsSupplierOrderViewModel> Products(int id);
        public IList<PurchaseProductsForDebitNoteViewModel> PurchaseProducts();

        public ICollection<PurchaseInvoiceListJson> GetPurchaseInvoices(int id);
    }
}
