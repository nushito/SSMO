﻿using SSMO.Models.Products;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Reports
{
    public interface IReportsService
    {
        IEnumerable<CustomerOrderDetailsModel> AllCustomerOrders(string name, int currentpage, int customerOrdersPerPage);
        CustomerOrderDetailsModel Details(int id);
        bool EditCustomerOrder(int id,string number,
                System.DateTime date,
                int myCompanyId,
                string deliveryTerms,
                string loadingPlace,
                string deliveryAddress,
               int currencyId, int status, 
               string fscClaim,string fscCertificate, 
               decimal paidAdvance, bool paidStatus,
               List<ProductCustomerFormModel> products);
        public IEnumerable<CustomerOrderListViewBySupplier> GetCustomerOrdersBySupplier(int customerId, string supplierId);

        public IEnumerable<CustomerInvoicePaymentDetailsModel> CustomersInvoicesPaymentDetails(string customerName, int currentpage, int customerInvoicePerPage);
        public IEnumerable<SupplierInvoicePaymentDetailsModel> SuppliersInvoicesPaymentDetails(string supplierName, int currentpage, int supplierInvoicePerPage);

        public CustomerOrderForEdit CustomerOrderDetailsForEdit(int id);

        public IEnumerable<CustomerOrderDetailsPaymentModel> CustomerOrdersPaymentDetails(string customerName, int currentpage, int customerOrdersPerPage);



    }
}
