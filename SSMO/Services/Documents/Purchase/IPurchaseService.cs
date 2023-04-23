﻿using SSMO.Models.Documents.Purchase;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.Purchase;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Purchase
{
    public interface IPurchaseService
    {
        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrdersForPurchase(string
            supplierName, int currentpage, int supplierOrdersPerPage);

        public bool CreatePurchaseAsPerSupplierOrder(int id,
            string number, DateTime date, bool paidStatus,
            decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses,
            int vat, string truckNumber, string swb, List<PurchaseProductAsSupplierOrderViewModel> products,
            string incoterms, decimal paidAdvance, DateTime? dateOfPayment);
        public EditPurchasePaymentDetails GetPurchaseForPaymentEdit(string number);
        public bool EditPurchasePayment(string number, bool paidStatus, decimal paidAvance, DateTime datePaidAmount);

        public PurchaseInvoiceDetailsViewModel PurchaseDetails(int id);

        public EditPurchaseViewModel PurchaseDetailsForEdit(int id);    
        public bool EditPurchaseInvoice(int id, string number,DateTime date, int supplierOrderId, 
            int vat, decimal netWeight, decimal grossWeight, string truckNumber, string swb, decimal purchaseTransportCost,
            decimal bankExpenses, decimal duty, decimal customsExpenses, decimal factoring, decimal fiscalAgentExpenses, 
            decimal procentComission, decimal otherExpenses, List<PurchaseProductsForEditFormModel> purchaseProducts);

        public List<PurchaseProductAsSupplierOrderViewModel> Products(int id);
    }
}
