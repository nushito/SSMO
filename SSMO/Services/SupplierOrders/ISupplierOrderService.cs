using SSMO.Models.Documents.Purchase;
using SSMO.Models.Products;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Services.Documents.Purchase;
using System;
using System.Collections.Generic;

namespace SSMO.Services.SupplierOrders
{
    public interface ISupplierOrderService
    {
        public int CreateSupplierOrder(int myCompanyId,
                int supplierId,
                DateTime Date,
                string number,
               int customerOrderNumber ,
                int statusId, int currencyId, string fscClaim, int vat,
                DateTime datePaidAmount, decimal paidAvance,  bool paidStatus, 
                string loadingAddress, string deliveryAddress, string deliveryTerms);
        void TotalAmountAndQuantitySum(int supplierOrderId);

        public SupplierOrderPaymentCollectionModel GetSupplierOrders(string supplierName, int currentpage, int supplierOrdersPerPage);

        public EditSupplierOrderPaymentModel GetSupplierOrderForEdit(string supplierOrderNumber);

        public bool EditSupplierOrderPayment(string supplierOrderNumber, decimal paidAdvance, DateTime date, bool paidStatus);

        public ICollection<SupplierOrdersListForPurchaseEditModel> GetSupplierOrdersNumbers();

    }
}
