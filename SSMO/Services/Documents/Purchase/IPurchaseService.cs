﻿using SSMO.Models.Reports.PaymentsModels;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Purchase
{
    public interface IPurchaseService
    {
        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrdersForPurchase(string
            supplierName, int currentpage, int supplierOrdersPerPage);

        public bool CreatePurchaseAsPerSupplierOrder(
            string supplierOrderNumber, string number, DateTime date,
           bool paidStatus, decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses, 
            int vat, string truckNumber, string fscCertificate, string fscClaim);
        public EditPurchasePaymentDetails GetPurchaseForPaymentEdit(string number);
        public bool EditPurchasePayment(string number, bool paidStatus, decimal paidAvance, DateTime datePaidAmount);
    }
}
