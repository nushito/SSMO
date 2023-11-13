using SSMO.Models.Reports.CustomerOrderCascadeDropDown;
using SSMO.Models.TransportCompany;
using SSMO.Services.Curruncies;
using SSMO.Services.Suppliers;
using System;
using System.Collections.Generic;

namespace SSMO.Models.ServiceOrders
{
    public class ServiceOrderFormModel
    {
        public int? Number { get; set; }
        public DateTime Date { get; set; }
        public int TransportCompanyId { get; set; }
        public ICollection<TransportCompanyListViewModel> TransportCompany { get; set; }       
        public int CurrencyId { get; set; }
        public ICollection<GetCurrencyModel> Currencies { get; set; }
        public decimal Cost { get; set; }
        public int? Vat { get; set; } = 0;//v % kolko se nachislqva DDS       
        public string LoadingAddress { get; set; }
        public string Etd { get; set; } // loading date
        public string Eta { get; set; }// estimate delivery date
        public string DeliveryAddress { get; set; }
        public string TruckNumber { get; set; }
        public decimal AmountAfterVat { get; set; }
        public int MyCompanyId { get; set; }
        public ICollection<MyCompaniesForTrasnportOrderViewModel> MyCompanies { get; set; }
        public int? SupplierId { get; set; }       
        public int? SupplierOrderId { get; set; }
        public ICollection<AllSuppliers> Suppliers { get; set; }
        public int? CustomerOrderNumberId { get; set; }
        public int? CustomerId { get; set; }
        public IEnumerable<CustomersListForServiceOrderViewModel> Customers { get; set; }
        public int? FiscalAgentId { get; set; }
        public ICollection<FiscalAgentsForServiceOrderVireModel> FiscalAgents { get; set; }      
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string Comment { get; set; }
        public string GrossWeight { get; set; }
        public string PaymentTerms { get; set; }
        public string PaymentMethod { get; set; }
        public string Payer { get; set; }
    }
}
