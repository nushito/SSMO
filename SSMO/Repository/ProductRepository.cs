using Irony.Parsing;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Reports.Products;
using SSMO.Models.ServiceOrders;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.Products;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext dbContext;   
        private readonly ICurrency currencyService;
        private readonly ISupplierService supplierService;
        private readonly ICustomerOrderRepository customerOrderRepository;
        public ProductRepository
            (ApplicationDbContext dbContext, ICurrency currencyService, 
            ISupplierService supplierService, ICustomerOrderRepository customerOrderRepository)
        {
            this.dbContext = dbContext;
            this.currencyService = currencyService;
            this.supplierService = supplierService;
            this.customerOrderRepository = customerOrderRepository;
        }

        public string GetDescriptionName(int id)
        {
            var name = dbContext.Descriptions
               .Where(a => a.Id == id)
               .Select(a => a.Name)
               .FirstOrDefault();
            return name;
        }
        public string GetGradeName(int id)
        {
            var name = dbContext.Grades
                .Where(a => a.Id == id)
                .Select(a => a.Name)
                .FirstOrDefault();
            return name;
        }
        public string GetSizeName(int id)
        {
            var name = dbContext.Sizes
                .Where(a => a.Id == id)
                .Select(a => a.Name)
                .FirstOrDefault();
            return name;
        }

        public Product GetMainProduct(int id)
        {
            var mainProduct = dbContext.Products
                 .Where(a => a.Id == id)
                 .FirstOrDefault();

            return mainProduct;          
        }        
        public async Task<ICollection<AllProductsFullNameModel>> ProductsFullName()
        {
            var allProducts = dbContext.Products.ToList();

            var products =  allProducts
                .GroupBy(d => new { d.DescriptionId, d.SizeId, d.GradeId })
                .Select(x => new
                {
                    Description = x.Key.DescriptionId,
                    Size = x.Key.SizeId,
                    Grade = x.Key.GradeId,
                    Id = x.OrderBy(d => d.Id)
                });

            var allProductsFullNames = new List<AllProductsFullNameModel>();

           foreach (var product in products.ToList()) 
            {
                var description = GetDescriptionName(product.Description);
                var grade = GetGradeName(product.Grade);
                var size = GetSizeName(product.Size);
                string fullName = description + " " + size + " " + grade;

                var productFullName = new AllProductsFullNameModel
                {                    
                    FullName= fullName,
                };

                List<int> numbers = product.Id.Select(a => a.Id).ToList();

                productFullName.ProductId = string.Join(",", numbers);
               
                await Task.Run(() => allProductsFullNames.Add(productFullName));
            }
           return allProductsFullNames;
        }

        //purchase details for a Product
        public async Task<ICollection<ProductPurchaseDetails>> ProductFullReport
            (int companyId, ICollection<int> productId, int currentpage, int productsPerPage)
        {
            if(productId == null)
            {
                return new List<ProductPurchaseDetails> ();
            }
            //all supplier orders for my company
            var checkSupplierCompany = dbContext.SupplierOrders
                    .Where(i => i.MyCompanyId == companyId)
                    .Select(i=>i.Id)
                    .ToList();
            //purchase products that match the ID and my company and Supplier Orders
            var purchasedPorduct = dbContext.PurchaseProductDetails
                .Where(i => productId.Contains(i.ProductId) && checkSupplierCompany.Contains(i.SupplierOrderId))
                .Skip((currentpage - 1) * productsPerPage)
                .Take(productsPerPage)
                .OrderByDescending(i=>i.Id)
                .ToList();

            var productDetails = new List<ProductPurchaseDetails>();
         
            //details for the purchases of this product
            foreach (var product in purchasedPorduct)
            {
                var supplierOrder = dbContext.SupplierOrders
                    .Where(i=>i.Id == product.SupplierOrderId)
                    .FirstOrDefault();

                var purchase = dbContext.Documents
                    .Where(i => i.SupplierOrderId == supplierOrder.Id && product.PurchaseInvoiceId == i.Id)
                    .FirstOrDefault();

                var purchaseProduct = new ProductPurchaseDetails
                {
                    SheetsPerPallet = product.SheetsPerPallet,
                    CostPrice = product.CostPrice,
                    Pallets = product.Pallets,
                    Currency = currencyService.GetCurrency(purchase.CurrencyId),
                    CostPriceCurrency = currencyService.GetCurrency(product.CostPriceCurrencyId ?? 0),
                    Date = purchase.Date,
                    DeliveryAddress = purchase.DeliveryAddress,
                    LoadingAddress = purchase.LoadingAddress,
                    PurchaseInvoice = purchase.PurchaseNumber,
                    PurchasePrice = product.PurchasePrice,
                    FscCertificate = product.FscCertificate,
                    FscClaim = product.FscClaim,
                    Quantity = product.Quantity,
                    Unit = product.Unit.ToString(),
                    SupplierOrderNumber = supplierOrder.Number,
                    //AutstandingQuantity = product.qua
                    SupplierName = supplierService.SupplierNameById(supplierOrder.SupplierId)
                };
               //all sells for this product
                var soldProduct = await ProductsSellsDetails(companyId, product.ProductId, product.Id);

                purchaseProduct.SellDetails = soldProduct;
                productDetails.Add(purchaseProduct);

                purchaseProduct.AutstandingQuantity = product.Quantity - soldProduct.Sum(q => q.Quantity);
            }
            var result = await Task.FromResult(productDetails);

            return result;
        }
        //all sells of product
        private async Task<ICollection<ProductSellDetails>> ProductsSellsDetails
            (int companyId, int productId, int purchaseProductId)
        {
            //all customers orders of my company
            var checkCustomerCompany = dbContext.CustomerOrders
                    .Where(i => i.MyCompanyId == companyId)
                    .Select(i => i.Id)
                    .ToList();
            //all sold products with ID = productId which are part of the customer orders
            var invoicedPorducts = dbContext.InvoiceProductDetails
                .Where(i => i.ProductId == productId && checkCustomerCompany
                            .Contains(i.CustomerOrderId??0) && i.PurchaseProductDetailsId 
                            == purchaseProductId)
                .ToList();

            if (!invoicedPorducts.Any()) 
            { 
                return new List<ProductSellDetails>();
            }

            var soldProducts = new List<ProductSellDetails>();

            //details for each sell invoice for this product
            foreach (var product in invoicedPorducts)
            {
                //find purchase for this product
                var purchaseId = dbContext.PurchaseProductDetails
                    .Where(i => i.Id == purchaseProductId)
                    .Select(i => i.PurchaseInvoiceId)
                    .FirstOrDefault();

                var purchaseNumber = dbContext.Documents
                    .Where(i => i.Id == purchaseId)
                    .Select(i => i.PurchaseNumber)
                    .FirstOrDefault();

                var invoice = dbContext.Documents
                    .Find(product.InvoiceId);

                var invoicedProduct = new ProductSellDetails
                {
                    Currency = currencyService.GetCurrency(invoice.CurrencyId),
                    CustomerName = dbContext.Customers
                      .Where(i => i.Id == invoice.CustomerId)
                      .Select(n => n.Name).FirstOrDefault(),
                    OrderConfirmationNumber = customerOrderRepository.GetCustomerOrderNumberById(product.CustomerOrderId) ?? 0,
                    CustomerPoNumber = customerOrderRepository.GetCustomerPoNumberById(product.CustomerOrderId),
                    Date = invoice.Date,
                    DeliveryAddress = invoice.DeliveryAddress,
                    LoadingAddress = invoice.LoadingAddress,
                    InvoiceNumber = invoice.DocumentNumber,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    FscCertificate = product.FscCertificate,
                    FscClaim = product.FscClaim,
                    Quantity = product.InvoicedQuantity,
                    Unit = product.Unit.ToString(),
                    SellPrice = product.SellPrice,
                    Type = invoice.DocumentType.ToString(),
                    Profit = product.Profit,
                    PurchaseInvoiceNumber = purchaseNumber
                };

                soldProducts.Add(invoicedProduct);

                if (product.CreditNoteId != null || product.DebitNoteId != null)
                {
                   var creditOrDebitInvoice = dbContext.Documents
                    .Where(i => i.Id == product.CreditNoteId || product.DebitNoteId == i.Id)
                    .FirstOrDefault();

                    var newProductDetails = new ProductSellDetails
                    {
                        Currency = currencyService.GetCurrency(creditOrDebitInvoice.CurrencyId),
                        CustomerName = dbContext.Customers
                      .Where(i => i.Id == creditOrDebitInvoice.CustomerId)
                      .Select(n => n.Name).FirstOrDefault(),
                        OrderConfirmationNumber = customerOrderRepository.GetCustomerOrderNumberById(product.CustomerOrderId) ?? 0,
                        CustomerPoNumber = customerOrderRepository.GetCustomerPoNumberById(product.CustomerOrderId),
                        Date = creditOrDebitInvoice.Date,
                        DeliveryAddress = creditOrDebitInvoice.DeliveryAddress,
                        LoadingAddress = creditOrDebitInvoice.LoadingAddress,
                        InvoiceNumber = creditOrDebitInvoice.DocumentNumber,                        
                        FscCertificate = product.FscCertificate,
                        FscClaim = product.FscClaim,                        
                        Unit = product.Unit.ToString(),                        
                        Type = creditOrDebitInvoice.DocumentType.ToString(),                        
                        PurchaseInvoiceNumber = purchaseNumber,
                        CreditOrDebitToInvoiceNumber = invoice.DocumentNumber
                    };
                    if(newProductDetails.Type == "CreditNote")
                    {
                        newProductDetails.Pallets = product.CreditNotePallets;
                        newProductDetails.SheetsPerPallet = product.CreditNoteSheetsPerPallet;
                        newProductDetails.Quantity = product.CreditNoteQuantity;
                        newProductDetails.SellPrice = product.CreditNotePrice;
                    }
                    else
                    {
                        newProductDetails.Pallets = product.DebitNotePallets;
                        newProductDetails.SheetsPerPallet = product.DebitNoteSheetsPerPallet;
                        newProductDetails.Quantity = product.DebitNoteQuantity;
                        newProductDetails.SellPrice = product.DebitNotePrice;
                    }
                    soldProducts.Add(newProductDetails);
                }
            }
            var result = await Task.FromResult(soldProducts);
           return result;
        }        
    }
}
