using SSMO.Models.Products;
using System;

namespace SSMO.Services.CustomerOrderService
{
    public interface ICustomerOrderService
    {
        public int CreateOrder
            (string num, DateTime date, int customer, 
            int company, string deliveryTerms, 
            string loadingAddress, string deliveryAddress, int currency);

        public SSMO.Data.Models.CustomerOrder OrderPerIndex(int id);
        public SSMO.Data.Models.CustomerOrder OrderPerNumber(string number);

        public bool CheckOrderNumberExist(string number);

        public bool EditProductAsPerSupplierSpec(int productId,int descriptionId,
            int sizeId,
            int gradeId,
            string fscClaim,
            string fscCertificate,
            int cusomerOrderId,
            decimal quantity,
            decimal purchasePrice,
            int pallets,
            int sheetsPerPallet
            );
       
    }
}
