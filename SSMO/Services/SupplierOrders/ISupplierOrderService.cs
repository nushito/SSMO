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
                DateTime datePaidAmount, decimal paidAvance,  bool paidStatus, string loadingAddress, string deliveryAddress);
        void TotalAmountAndQuantitySum(int supplierOrderId);

        public IEnumerable<string> GetSuppliers();

        public IEnumerable<SupplierOrdersPaymentDetailsModel> GetSupplierOrders(string supplierName);

        public EditSupplierOrderPaymentModel GetSupplierOrderForEdit(string supplierOrderNumber);

        public bool EditSupplierOrderPayment(string supplierOrderNumber, decimal paidAdvance, DateTime date, bool paidStatus);

    }
}
