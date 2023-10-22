using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.Purchase;
using SSMO.Models.Products;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Services.Documents.Purchase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSMO.Services.SupplierOrders
{
    public interface ISupplierOrderService
    {
        public Task<int> CreateSupplierOrder(int myCompanyId,
                int supplierId,
                DateTime Date,
                string number,              
                int statusId, int currencyId, string fscClaim, int vat,                 
                string loadingAddress, string deliveryAddress, string deliveryTerms);
        Task TotalAmountAndQuantitySum(int supplierOrderId);

        public SupplierOrderPaymentCollectionModel GetSupplierOrders
            (string supplierName,DateTime startDate, DateTime endDate, int currentpage, int supplierOrdersPerPage);

        public EditSupplierOrderPaymentModel GetPaymentsPerOrderForEdit(int id);

        public bool EditSupplierOrderPurchasePayment
            (int id, decimal? paidAdvance, DateTime? date, string newCurrency,string action,decimal? exchangeRate, 
           ICollection<PurchaseNewpaymentsPerOrderFormModel> PurchasePayments);

        public ICollection<SupplierOrdersListForPurchaseEditModel> GetSupplierOrdersNumbers();
        public ICollection<SupplierOrdersNumbersListViewModel> GetSupplierOrdersNumbersJsonList(int id);
        public ICollection<SupplierOrdersBySupplier> SuppliersAndOrders();

    }
}
