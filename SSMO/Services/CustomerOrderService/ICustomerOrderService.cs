using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Services.TransportService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSMO.Services.CustomerOrderService
{
    public interface ICustomerOrderService
    {
        public Task<int> CreateOrder
            (string num, DateTime date, int customer, 
            int company, string deliveryTerms, 
            string loadingAddress, string deliveryAddress,
            int currency,string origin,  
            int vat, int statusId,List<int> supplierOrders, 
            string comment, List<int> banks, string type, int? fiscalAgentId,
            string dealType, string dealDescription, int? fscText, string paymentTerms,
            string eta, string etd);

        public Task<int> CreateFirstOrder
            (int number, string num, DateTime date, int customer,
            int company, string deliveryTerms,
            string loadingAddress, string deliveryAddress,
            int currency, string origin, 
            int vat, int statusId, List<int> supplierOrders,
            string comment, List<int> banks, string type, int? fiscalAgentId,
            string dealType, string dealDescription, int? fscTextm, string paymentTerms,
            string eta, string etd);

        public SSMO.Data.Models.CustomerOrder OrderPerIndex(int id);
        public SSMO.Data.Models.CustomerOrder OrderPerNumber(int number);
        public bool CheckOrderNumberExist(int number);

        public void CheckCustomerOrderStatus(int id);

        public void CustomerOrderCounting(int customerorderId);

        public bool AnyCustomerOrderExist();

        public List<CustomerOrdersJsonList> CustomerOrderCollection(int customerorderId,int mycompanyId);
        public ICollection<CustomerOrderForInvoiceViewModel> AllCustomerOrderNumbers();

        public ICollection<CustomerOrderNumbersByCustomerViewModel> CustomerOrderNumbersPerInvoice(int id);

        public EditCustomerOrderPaymentModel GetCustomerOrderPaymentForEdit(int orderConfirmationNumber);
        public bool EditCustomerOrdersPayment(int orderConfirmationNumber, bool paidStatus, decimal paidAdvance, DateTime date);
        public int CustomerOrderNumber(int supplierOrderId);
        public int CustomerOrderNumberById(int id);

        public ICollection<BankDetailsViewModel> GetBanks();
        public CustomerOrderPrintViewModel GetCustomerOrderPrint(int id);
        public ICollection<CustomerOrderJsonListForServiceOrder> CustomerOrdersForService(int id);
    }
}
