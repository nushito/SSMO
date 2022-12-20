﻿using SSMO.Models.Products;
using SSMO.Models.Reports.ProductsStock;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Products
{
    public interface IProductService
    {
        public void CreateProduct(ProductCustomerFormModel model, int customerorderId);
        public bool DescriptionExist(string name);
        public void AddDescription(string name, string bgName);
        public void AddGrade(string name);
        public bool GradeExist(string name);
        public void AddSize(string name);
        public bool SizeExist(string name);
        public IEnumerable<string> GetDescriptions();
        public IEnumerable<string> GetSizes();
        public IEnumerable<string> GetGrades();

        public IEnumerable<ProductSupplierDetails> Details(int customerId);

        public bool EditProduct(int id, int customerorderId,
            int supplierOrderId,
            string description, string grade,
            string size, string purchaseFscCert, string purchaseFscClaim,
            int pallets, int sheetsPerPallet, decimal purchasePrice, decimal quantityM3);
        public ICollection<ProductCustomerFormModel> DetailsPerCustomerOrder(int id);
        public ICollection<ProductsForEditSupplierOrder> ProductsDetailsPerSupplierOrder(int supplierOrderId);
        public ICollection<string> GetFascCertMyCompany();

        public string GetDescriptionName(int id);
        public string GetGradeName(int id);
        public string GetSizeName(int id);
        public decimal CalculateDeliveryCostOfTheProductInCo(decimal quantity, decimal totalQuantity, decimal deliveryCost);

        public void ClearProductQuantityWhenDealIsFinished(int productId);
        public IEnumerable<ProductAvailabilityDetailsViewModel> ProductsOnStock
            (int descriptionId, int gradeId, int sizeId, int currentPage, int productsPerPage);

        public ICollection<DescriptionForProductSearchModel> DescriptionIdAndNameList();
        public ICollection<SizeForProductSearchModel> SizeIdAndNameList();
        public ICollection<GradeForProductSearchModel> GradeIdAndNameList();

    }
}
