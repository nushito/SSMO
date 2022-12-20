using SSMO.Models.Products;
using SSMO.Models.Reports.PaymentsModels;
using System;
using System.Collections.Generic;

namespace SSMO.Services.CustomerOrderService
{
    public interface ICustomerOrderService
    {
        public int CreateOrder
            (string num, DateTime date, int customer, 
            int company, string deliveryTerms, 
            string loadingAddress, string deliveryAddress,
            int currency,string origin, bool paidStatus, 
            int vat, int statusId);

        public int CreateFirstOrder
            (int number, string num, DateTime date, int customer,
            int company, string deliveryTerms,
            string loadingAddress, string deliveryAddress,
            int currency, string origin, bool paidStatus,
            int vat, int statusId);

        public SSMO.Data.Models.CustomerOrder OrderPerIndex(int id);
        public SSMO.Data.Models.CustomerOrder OrderPerNumber(int number);

        public bool CheckOrderNumberExist(int number);

        //public bool EditProductAsPerSupplierSpec(int productId,int descriptionId,
        //    int sizeId,
        //    int gradeId,
        //    string fscClaim,
        //    string fscCertificate,
        //    int cusomerOrderId,
        //    decimal quantity,
        //    decimal purchasePrice,
        //    int pallets,
        //    int sheetsPerPallet
        //    );
       
        public void CustomerOrderCounting(int customerorderId);

        public bool AnyCustomerOrderExist();

        public ICollection<int> AllCustomerOrderNumbers();

        public EditCustomerOrderPaymentModel GetCustomerOrderPaymentForEdit(int orderConfirmationNumber);
        public bool EditCustomerOrdersPayment(int orderConfirmationNumber, bool paidStatus, decimal paidAdvance);
        public int CustomerOrderNumber(int supplierOrderId);
        public int CustomerOrderNumberById(int id); 
        
    }
}
