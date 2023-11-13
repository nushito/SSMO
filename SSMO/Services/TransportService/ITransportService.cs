using SSMO.Models.Reports.ServiceOrders;
using SSMO.Models.ServiceOrders;
using SSMO.Models.TransportCompany;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Services.TransportService
{
    public interface ITransportService
    {
        public bool FirstTransport();

        public bool CreateTransportCompany
            (string name, string eik, string vat,string phoneNumber, 
            string email, string city, string country, string address, string manager,string userId );

        public List<TransportCompanyListViewModel> TransportCompanies();

        public EditTransportCompanyFormModel GetTransportCompanyForEdit(int id);

        public bool EditTransportCompany(int id, string name, string eik, string vat, string phoneNumber,
            string email, string city, string country, string address, string manager);

        public bool CreateTransportOrder
            (int? number,DateTime date, int transportCompanyId, string loadingAddress, string eta, string etd,
            string deliveryAddress, string truckNumber,decimal cost,int myCompanyId, int? supplierId,
            int? supplierOrderId, int? customerId, int? customerOrderId, int? fiscalAgent, 
            int currencyId,int? vat, string driverName, string driverPhone, string comment, string weight,
            string paymentMethod, string paymentTerms, string payer);

        public ServiceOrderForEditViewModel GetForEditTransportOrder(int id);

        public bool EditTransportOrder(int id, DateTime date, int transportCompanyId, string loadingAddress,
             string eta, string etd, string deliveryAddress, string truckNumber, decimal cost,
            int myCompanyId, int? supplierOrderId, int? customerOrderId,
            int? fiscalAgent, int currencyId, int? vat, string driverName, string driverPhone,
            string comment, string weight, string paymentMethod, string paymentTerms, string payer);
        public ServiceOrderDetailsPrintViewModel ServiceOrderPrintDetails(int id);

    }
}
