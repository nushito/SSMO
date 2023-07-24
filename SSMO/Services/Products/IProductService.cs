using SSMO.Data.Enums;
using SSMO.Models.Descriptions;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Grades;
using SSMO.Models.Products;
using SSMO.Models.Reports.CreditNote;
using SSMO.Models.Reports.DebitNote;
using SSMO.Models.Reports.ProductsStock;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Models.Sizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Products
{
    public interface IProductService
    {
        public void CreateProduct(ProductSupplierFormModel model, int supplierOrderId);
        public void CreateNewProductOnEditSupplierOrder(NewProductsForSupplierOrderModel products);    
        public bool DescriptionExist(string name);
        public void AddDescription(string name, string bgName);
        public void AddGrade(string name);
        public bool GradeExist(string name);
        public void AddSize(string name);
        public bool SizeExist(string name);
        public IEnumerable<string> GetDescriptions();
        public IEnumerable<string> GetSizes();
        public IEnumerable<string> GetGrades();
        public ICollection<string> GetUnits();
        public IEnumerable<ProductPerSupplierOrderDetails> Details(List<int> supplierOrderserId);
        public bool CreateCustomerOrderProduct(int id, int customerorderId,
            int supplierOrderId, string description, string grade,
            string size, string fscCert, string fscClaim,
            int pallets, int sheetsPerPallet, decimal price, decimal orderedQuantity, string unit);
        public ICollection<ProductCustomerFormModel> DetailsPerCustomerOrder(int id);
        public ICollection<ProductsForEditSupplierOrder> ProductsDetailsPerSupplierOrder(int supplierOrderId);
        public ICollection<string> GetFascCertMyCompany();
        public string GetDescriptionName(int id);
        public string GetGradeName(int id);
        public string GetSizeName(int id);
        public decimal CalculateDeliveryCostOfTheProductInCo
            (decimal quantity, decimal quantityM3, decimal totalQuantity, decimal deliveryCost, Unit unit, string size);
        public void ClearProductQuantityWhenDealIsFinished(int productId, decimal quantity, decimal oldQuantity);
        public ProductsAvailabilityCollectionViewModel ProductsOnStock
            (int? descriptionId, int? gradeId, int? sizeId, int currentPage, int productsPerPage);
        public ICollection<DescriptionForProductSearchModel> DescriptionIdAndNameList();
        public ICollection<SizeForProductSearchModel> SizeIdAndNameList();
        public ICollection<GradeForProductSearchModel> GradeIdAndNameList();
        public void ResetToNullLoadingQuantityIfPurchaseIsChanged(int productId);
        public void NewLoadingQuantityToEditPurchase(int productId, int purchaseId);
        public ICollection<string> FscClaimList();
        public List<ProductsForInvoiceViewModel> ProductsForInvoice(List<int> customerOrders);
        public bool AddNewProductsToEditedInvoice(int id, List<ProductsForInvoiceViewModel> products);
       // public List<EditProductForCompanyInvoicesViewModel> NewProductsForEditInvoice(List<int> customerOrders);
       public List<ProductForCreditNoteViewModelPerInvoice> ProductsForCreditNotePerInvoice(int invoiceId);
        public decimal ConvertStringSizeToQubicMeters(string size);
        public void ReviseAutstandingQuantity(int customerOrderDetailId, decimal quantity);
        public List<NewProductsFromOrderEditedDebitNoteViewModel> ProductsForDebitNotePerInvoice(int invoiceId);

        public List<DescriptionsViewModel> DescriptionCollection();
        public List<SizeViewModel> SizeCollection();
        public List<GradeViewModel> GradeCollection();  
        
    }
}
