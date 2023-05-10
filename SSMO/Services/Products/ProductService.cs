using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Models;
using SSMO.Models.Descriptions;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Grades;
using SSMO.Models.Products;
using SSMO.Models.Reports.CreditNote;
using SSMO.Models.Reports.ProductsStock;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Models.Sizes;
using System;
using System.Collections.Generic;
using System.Linq;
using Description = SSMO.Data.Models.Description;

namespace SSMO.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;

        public ProductService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper.ConfigurationProvider;
        }
        public void CreateProduct(ProductSupplierFormModel model, int supplierOrderId)
        {
            var description = dbContext.Descriptions.Where(a => a.Id == model.DescriptionId).Select(n=>n.Name).FirstOrDefault();
            var size = dbContext.Sizes.Where(a => a.Id == model.SizeId).Select(n=>n.Name).FirstOrDefault();
            var grade = dbContext.Grades.Where(a => a.Id == model.GradeId).Select(n=>n.Name).FirstOrDefault();

            var product = new Product
            {
                DescriptionId = model.DescriptionId,
                GradeId = model.GradeId,
                SizeId = model.SizeId,
                PurchaseFscClaim = model.FscClaim,
                PurchaseFscCertificate = model.FscCertificate,
                PurchasePrice = model.PurchasePrice,
                Pallets = model.Pallets,
                SheetsPerPallet = model.SheetsPerPallet,
                SupplierOrderId = supplierOrderId,
                OrderedQuantity = model.Quantity,
                Unit = Enum.Parse<Unit>(model.Unit),
                TotalSheets = model.Pallets*model.SheetsPerPallet
            };

            //var dimensionArray = size.Split('/').ToArray();
            //var countArray = dimensionArray.Count();
            //decimal sum = 1M;

            //for (int i = 0; i < countArray; i++)
            //{
            //    sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 4);
            //}

            //if (model.QuantityM3 != 0)
            //{
            //    product.OrderedQuantity = model.QuantityM3;
            //}
            //else
            //{
            //    product.OrderedQuantity = Math.Round(sum * model.Pallets * model.SheetsPerPallet, 4);
            //}

            product.QuantityAvailableForCustomerOrder = product.OrderedQuantity;
            product.QuantityLeftForPurchaseLoading = product.OrderedQuantity;
            product.PurchaseAmount = Math.Round(model.PurchasePrice * product.OrderedQuantity, 4);
            dbContext.Products.Add(product);
            dbContext.SaveChanges();


            var order = dbContext.SupplierOrders.Where(a => a.Id == supplierOrderId).FirstOrDefault();           
            order.Amount += product.PurchaseAmount;
            order.Products.Add(product);

            dbContext.SaveChanges();
        }

        public void AddDescription(string name, string bgName)
        {
            dbContext.Descriptions.Add(new Description { Name = name, BgName = bgName });
            dbContext.SaveChanges();
            return;
        }

        public void AddGrade(string name)
        {
            dbContext.Grades.Add(new Grade { Name = name });
            dbContext.SaveChanges();
            return;
        }

        public void AddSize(string name)
        {
            dbContext.Sizes.Add(new Size { Name = name });
            dbContext.SaveChanges();
            return;
        }

        public bool DescriptionExist(string name)
        {
            var check = dbContext.Descriptions.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }

        public bool GradeExist(string name)
        {
            var check = dbContext.Grades.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }

        public bool SizeExist(string name)
        {
            var check = dbContext.Sizes.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<string> GetDescriptions()
        {
            return dbContext.Descriptions.Select(a => a.Name).ToList();
        }
        public IEnumerable<string> GetSizes()
        {
            return dbContext.Sizes.Select(a => a.Name).ToList();
        }

        public IEnumerable<string> GetGrades()
        {
            return dbContext.Grades.Select(a => a.Name).ToList();
        }

        public bool EditProduct(int id, int customerorderId,
            int supplierOrderId, string description, string grade,
            string size, string fscCert, string fscClaim,
            int pallets, int sheetsPerPallet, decimal price, decimal orderedQuantity, string unit)
        {


            var product = dbContext.Products.Find(id);
            //var descriptionEdit = dbContext.Descriptions.Where(a => a.Name == description).Select(a => a.Id).FirstOrDefault();
            //var gradeEdit = dbContext.Grades.Where(a => a.Name == grade).Select(a => a.Id).FirstOrDefault();
            //var sizeEdit = dbContext.Sizes.Where(a => a.Name == size).Select(a => a.Id).FirstOrDefault();

            if (product == null || product.QuantityAvailableForCustomerOrder < orderedQuantity || product.QuantityAvailableForCustomerOrder <= 0)
            {
                return false;
            }

            product.CustomerOrderProductDetails= new List<CustomerOrderProductDetails>();

            var customerOrderProduct = new CustomerOrderProductDetails()
            {
            ProductId = product.Id,
            CustomerOrderId= customerorderId,
            FscClaim = fscClaim,
            FscCertificate = fscCert,
            Pallets = pallets,
            SheetsPerPallet = sheetsPerPallet,
            Quantity= orderedQuantity,
            SellPrice = price,
            SupplierOrderId = supplierOrderId,
            Unit = Enum.Parse<Unit>(unit),
            TotalSheets = product.Pallets * product.SheetsPerPallet,
            Amount = Math.Round(price * orderedQuantity, 4),
            AutstandingQuantity= orderedQuantity
            };

            product.CustomerOrderProductDetails.Add(customerOrderProduct);
            
            product.QuantityAvailableForCustomerOrder -= customerOrderProduct.Quantity;

            //var dimensionArray = size.Split('/').ToArray();
            //var countArray = dimensionArray.Count();
            //decimal sum = 1M;

            //for (int i = 0; i < countArray; i++)
            //{
            //    sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 4);
            //}


            //if (quantityM3 != 0)
            //{
            //    product.OrderedQuantity = quantityM3;
            //}
            //else
            //{
            //    product.OrderedQuantity = Math.Round(sum * product.TotalSheets, 4);
            //}

            var customerOrder = dbContext.CustomerOrders.Where(i => i.Id == customerorderId).FirstOrDefault();
           // customerOrder.CustomerOrderProducts = new List<CustomerOrderProductDetails>();  

            customerOrder.Amount += customerOrderProduct.Amount;
           // customerOrder.CustomerOrderProducts.Add(customerOrderProduct);

            dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<ProductPerSupplierOrderDetails> Details(List<int> supplierOrderserId)
        {
            var products = dbContext.Products
                .Where(a => supplierOrderserId.Contains(a.SupplierOrderId ?? 0))
                .ProjectTo<ProductPerSupplierOrderDetails>(mapper)
                .ToList();

            foreach (var item in products)
            { 
                item.Description = GetDescriptionName(item.DescriptionId);
                item.Grade = GetGradeName(item.GradeId);
                item.Size = GetSizeName(item.SizeId);
            }

            return products;
        }

        public ICollection<ProductCustomerFormModel> DetailsPerCustomerOrder(int customerId)
        {
            var products = dbContext.CustomerOrderProductDetails
                .Where(a => a.CustomerOrderId == customerId)
                .ProjectTo<ProductCustomerFormModel>(mapper)
                .ToList();

            foreach (var product in products)
            {
                var mainProduct = dbContext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                product.Description = GetDescriptionName(mainProduct.DescriptionId);
                product.Grade = GetGradeName(mainProduct.GradeId);
                product.Size = GetSizeName(mainProduct.SizeId);

            }
            return products;
        }

        public ICollection<string> GetFascCertMyCompany()
        {
            var fscCert = dbContext.MyCompanies.Select(f => f.FSCSertificate).ToList();
            return fscCert;
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

        public decimal CalculateDeliveryCostOfTheProductInCo(decimal quantity, decimal totalQuantity, decimal deliveryCost)
        {
            var cost = deliveryCost / totalQuantity * quantity;
            return cost;
        }

        public ICollection<ProductsForEditSupplierOrder> ProductsDetailsPerSupplierOrder(int supplierOrderId)
        {
            
            var products = dbContext.Products
                 .Where(a => a.SupplierOrderId == supplierOrderId)
                 .ProjectTo<ProductsForEditSupplierOrder>(mapper)
                 .ToList();

            foreach (var item in products)
            {
                item.Description = GetDescriptionName(item.DescriptionId);
                item.Grade = GetGradeName(item.GradeId);
                item.Size = GetSizeName(item.SizeId);              
            }
            return products;       
        }

        public void ClearProductQuantityWhenDealIsFinished
            (int productId, decimal quantity, decimal oldQuantity)
        {
            var product = dbContext.Products
                .Where(id => id.Id == productId)
                .FirstOrDefault();

            if (product != null)
            {
                product.QuantityAvailableForCustomerOrder += quantity - oldQuantity;
            }

            dbContext.SaveChanges();
        }

        public ProductsAvailabilityCollectionViewModel ProductsOnStock
            (int? descriptionId, int? gradeId, int? sizeId, int currentPage = 1, int productsPerPage = int.MaxValue)
        {
            var products = dbContext.Products
               .Where(d => d.LoadedQuantityM3 != 0);
               

            if (descriptionId != null)
            {
                products = products
                         .Where(d => d.DescriptionId == descriptionId);
            }

            if (gradeId != null)
            {
                products = products
                         .Where(d => d.GradeId == gradeId);
            }

            if ( sizeId != null)
            {
                products = products
                         .Where(d => d.SizeId == sizeId);

            }

            var productsOnStok = new List<ProductAvailabilityDetailsViewModel>();
            string supplierOrderDeliveryAddress = null;

            foreach (var product in products.ToList())
            {
                var statusId = dbContext.Statuses
                    .Where(n=>n.Name == "Active")
                    .Select(id=>id.Id)
                    .FirstOrDefault();

                var order = dbContext.CustomerOrders
                    .Where(pr => pr.Id == product.CustomerOrderId && pr.StatusId == statusId)
                    .FirstOrDefault();

                if(order == null) continue;

                //var purchase = dbContext.Documents
                //    .Where(p=>p.Id == product.PurchaseDocumentId)
                //    .FirstOrDefault();

                //if (purchase != null)
                //{
                //     supplierOrderDeliveryAddress = dbContext.SupplierOrders
                //        .Where(dI => dI.Documents.Any(p => p.Id == purchase.Id))
                //        .Select(ad => ad.DeliveryAddress)
                //        .FirstOrDefault();
                //}

                var customerName = dbContext.Customers
                    .Where(o => o.Id == order.CustomerId)
                    .Select(n=>n.Name)
                    .FirstOrDefault();

                var supplierName = dbContext.Suppliers
                    .Where(d => d.Documents.Any(o => o.CustomerOrderProducts.Select(a=>a.CustomerOrderId).FirstOrDefault() == order.Id))
                    .Select(n => n.Name)
                    .FirstOrDefault();

                var availableProduct = new ProductAvailabilityDetailsViewModel
                {
                    CostPrice = product.CostPrice,
                    CustomerOrderId = product.CustomerOrderId,
                    LoadedQuantity = product.LoadedQuantityM3,
                    OrderedQuantity = product.OrderedQuantity,
                    FSCClaim = product.FscClaim,
                    FSCSertificate = product.FscSertificate,
                    DeliveryAddress = supplierOrderDeliveryAddress,
                    CustomerOrderNumber = order.OrderConfirmationNumber,
                    CustomerName = customerName,
                    PurchasePrice = product.PurchasePrice,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    OrderDate = order.Date,
                   // PurchaseDate = purchase.Date,
                    //PurchaseNumber = purchase.PurchaseNumber,
                    Price = product.Price,
                    SupplierName = supplierName,
                    Unit = product.Unit.ToString()
                };
                productsOnStok.Add(availableProduct);
            }

            var productCollection = productsOnStok.Skip((currentPage - 1) * productsPerPage).Take(productsPerPage);

            var productListAndCount = new ProductsAvailabilityCollectionViewModel
            {
                 Products= productCollection,
                 TotalProducts = productsOnStok.Count()
            };

            return productListAndCount;
        }

        public ICollection<DescriptionForProductSearchModel> DescriptionIdAndNameList()
        {
            return dbContext.Descriptions
                .Select(n=> new DescriptionForProductSearchModel
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList();
        }
        public ICollection<SizeForProductSearchModel> SizeIdAndNameList()
        {
            return dbContext.Sizes
               .Select(n => new SizeForProductSearchModel
               {
                   Id = n.Id,
                   Name = n.Name
               }).ToList();
        }

        public ICollection<GradeForProductSearchModel> GradeIdAndNameList()
        {
            return dbContext.Grades
                .Select(n => new GradeForProductSearchModel
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList();
        }
        public void ResetToNullLoadingQuantityIfPurchaseIsChanged(int productId)
        {
            var product = dbContext.Products
                .Where(i => i.Id == productId) 
                .FirstOrDefault();

            product.LoadedQuantityM3 = 0;   
        }

        public void NewLoadingQuantityToEditPurchase(int productId, int purchaseId)
        {
            var product = dbContext.Products
                .Where(i => i.Id == productId).FirstOrDefault();

            product.LoadedQuantityM3 = product.OrderedQuantity;
            dbContext.SaveChanges();
        }
        public ICollection<string> GetUnits()
        {
           return Enum.GetNames(typeof(Unit));           
        }
        public ICollection<string> FscClaimList()
        {
           return dbContext.Products
                .Select(f=>f.FscClaim)
                .Distinct()
                .ToList();
        }

        public List<ProductsForInvoiceViewModel> ProductsForInvoice(List<int> customerOrders)
        {
           var customerOrdersIdList = dbContext.CustomerOrders
                .Where(i=> customerOrders.Contains(i.OrderConfirmationNumber))
                .Select(i=>i.Id) 
                .ToList();

            var products = dbContext.CustomerOrderProductDetails
                .Where(i => customerOrdersIdList.Contains(i.CustomerOrderId) && i.AutstandingQuantity > 0)
                .ToList();

            var invoicedProducts = new List<ProductsForInvoiceViewModel>();

            foreach (var product in products)
            {
                var mainProduct = dbContext.Products
                    .Where(i=>i.Id == product.ProductId).FirstOrDefault();
                //TODO dali invoicedquantity da e = loadedquantity?
                if (mainProduct.LoadedQuantityM3 > 0)
                {
                    var productForInvoice = new ProductsForInvoiceViewModel
                    {
                        Id = product.Id,
                        CustomerOrderId = product.CustomerOrderId,
                        ProductId = product.ProductId,
                        Unit = product.Unit,
                        Pallets = product.Pallets,
                        SheetsPerPallet = product.SheetsPerPallet,
                        InvoicedQuantity = product.Quantity,
                        SellPrice = product.SellPrice,
                        FscCertificate = product.FscCertificate,
                        FscClaim = product.FscClaim,
                        PurchaseCostPrice = new List<PurchaseProductCostPriceViewModel>() ,
                        Descriptions = new List<DescriptionForInvoiceViewModel>(),
                        Sizes = new List<SizeForInvoiceViewModel>(),
                        Grades = new List<GradeForInvoiceViewModel>()
                    };

                    var descriptionId = dbContext.Products
                   .Where(i => i.Id == mainProduct.Id)
                   .Select(d => d.DescriptionId).FirstOrDefault();

                    var gradeId = dbContext.Products
                        .Where(i => i.Id == mainProduct.Id)
                        .Select(d => d.GradeId).FirstOrDefault();

                    var sizeId = dbContext.Products
                        .Where(i => i.Id == mainProduct.Id)
                        .Select(d => d.SizeId).FirstOrDefault();


                    productForInvoice.Description = GetDescriptionName(descriptionId);
                    productForInvoice.Grade = GetGradeName(gradeId);
                    productForInvoice.Size = GetSizeName(sizeId);
                    productForInvoice.DescriptionId = descriptionId;
                    productForInvoice.GradeId = gradeId;
                    productForInvoice.SizeId = sizeId;

                    var descriptions = new DescriptionForInvoiceViewModel
                    {
                        Id = descriptionId,
                        Name = GetDescriptionName(descriptionId)
                    };

                    var grades = new GradeForInvoiceViewModel
                    {
                        Id = gradeId,
                        Name = GetGradeName(gradeId)
                    };

                    var sizes = new SizeForInvoiceViewModel
                    {
                        Id = sizeId,
                        Name = GetSizeName(sizeId)
                    };

                    productForInvoice.Descriptions.Add(descriptions);
                    productForInvoice.Grades.Add(grades);
                    productForInvoice.Sizes.Add(sizes);

                    var purchaseProductCostPrice = dbContext.PurchaseProductDetails
                        .Where(pi => pi.ProductId == mainProduct.Id)
                        .Select(c => new PurchaseProductCostPriceViewModel
                        {
                            PurchaseCostPriceId = c.Id,
                            CostPrice = c.CostPrice,
                        }).ToList();

                    productForInvoice.PurchaseCostPrice = purchaseProductCostPrice;

                    invoicedProducts.Add(productForInvoice);
                }
            }
            return invoicedProducts;           
        }

        public void CreateNewProductOnEditSupplierOrder(NewProductsForSupplierOrderModel modelProduct)
        {
            if (modelProduct == null) return;

            var product = new Product
            {
                DescriptionId = modelProduct.DescriptionId,
                GradeId = modelProduct.GradeId,
                SizeId = modelProduct.SizeId,
                PurchaseFscClaim = modelProduct.FscClaim,
                PurchaseFscCertificate = modelProduct.FscCertificate,
                PurchasePrice = modelProduct.PurchasePrice,
                Pallets = modelProduct.Pallets,
                SheetsPerPallet = modelProduct.SheetsPerPallet,
                SupplierOrderId = modelProduct.SupplierOrderId,
                OrderedQuantity = modelProduct.Quantity,
                Unit = Enum.Parse<Unit>(modelProduct.Unit),
                TotalSheets = modelProduct.Pallets * modelProduct.SheetsPerPallet
            };
            
            product.QuantityAvailableForCustomerOrder = modelProduct.Quantity;
            product.QuantityLeftForPurchaseLoading = modelProduct.Quantity;
            product.PurchaseAmount = Math.Round(modelProduct.PurchasePrice * modelProduct.Quantity, 4);

           // dbContext.Products.Add(product);

            var order = dbContext.SupplierOrders.Where(a => a.Id == modelProduct.SupplierOrderId).FirstOrDefault();
            order.Amount += product.PurchaseAmount;
            order.Products.Add(product);

            dbContext.SaveChanges();
        }

        public bool AddNewProductsToEditedInvoice(int id, List<ProductsForInvoiceViewModel> products)
        {
            if(id == 0) return false;

            var invoice = dbContext.Documents
                .Where(i => i.Id == id)
                .FirstOrDefault();

            foreach (var product in products)
            {
                if(product.InvoicedQuantity == 0) continue;

                var mainProduct = dbContext.Products
                    .Where(co => co.Id == product.ProductId).FirstOrDefault();

                mainProduct.SoldQuantity += product.InvoicedQuantity;

                var customerOrderProduct = dbContext.CustomerOrderProductDetails
                    .Where(i => i.ProductId == product.ProductId && i.CustomerOrderId == product.CustomerOrderId)
                    .FirstOrDefault();

                var customerOrder = dbContext.CustomerOrders
                    .Where(i => i.Id == product.CustomerOrderId)
                    .FirstOrDefault();

                if(invoice.CustomerId != customerOrder.CustomerId)
                {
                    invoice.CustomerId= customerOrder.CustomerId;
                }

                customerOrderProduct.AutstandingQuantity -= product.InvoicedQuantity;

                var purchaseProductDetail = dbContext.PurchaseProductDetails
                    .Where(a => a.Id == product.PurchaseCostPriceId).FirstOrDefault();

                var supplierOrder = dbContext.SupplierOrders
                    .Where(a => a.Id == purchaseProductDetail.SupplierOrderId).FirstOrDefault();

                var invoiceProduct = new InvoiceProductDetails
                {
                    CustomerOrderId = product.CustomerOrderId,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    FscCertificate = product.FscCertificate,
                    FscClaim = product.FscClaim,
                    ProductId = product.ProductId,
                    PurchaseProductDetailsId = purchaseProductDetail.Id,
                    SellPrice = product.SellPrice,
                    Unit = product.Unit,
                    VehicleNumber = product.VehicleNumber,
                    InvoicedQuantity = product.InvoicedQuantity,
                    BgPrice = product.SellPrice * invoice.CurrencyExchangeRateUsdToBGN,
                    CustomerOrderProductDetailsId = customerOrderProduct.Id,

                };

                invoiceProduct.TotalSheets = product.Pallets * product.SheetsPerPallet;
                invoiceProduct.Amount = invoiceProduct.SellPrice * invoiceProduct.InvoicedQuantity;
                invoiceProduct.BgAmount = invoiceProduct.BgPrice * invoiceProduct.InvoicedQuantity;
               
                invoice.SupplierOrderId = supplierOrder.Id;
                invoice.InvoiceProducts.Add(invoiceProduct);
                invoiceProduct.CustomerOrderProductDetailsId = customerOrderProduct.Id;

                dbContext.SaveChanges();
            }

            foreach (var product in invoice.InvoiceProducts)
            {
                var purchaseProductDetailCostPrice = dbContext.PurchaseProductDetails
                    .Where(a => a.Id == product.PurchaseProductDetailsId)
                    .Select(p=>p.CostPrice)
                    .FirstOrDefault();

                product.DeliveryCost = CalculateDeliveryCostOfTheProductInCo
                   (product.InvoicedQuantity, invoice.TotalQuantity, invoice.DeliveryTrasnportCost);
                product.Profit = (product.SellPrice - purchaseProductDetailCostPrice) * 
                    product.InvoicedQuantity - product.DeliveryCost;
            }

            dbContext.SaveChanges();
            return true;
        }

        public List<ProductForCreditNoteViewModelPerInvoice> ProductsForCreditNotePerInvoice(int invoiceId)
        {
            if(invoiceId == 0) return null;

            var products = dbContext.InvoiceProductDetails
                .Where(i => i.InvoiceId == invoiceId)
                .ToList();

            var productsForEdit = new List<ProductForCreditNoteViewModelPerInvoice>();    

            foreach (var product in products)
            {
                var mainProduct = dbContext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                productsForEdit.Add(new ProductForCreditNoteViewModelPerInvoice
                {
                    ProductId = product.ProductId,
                    FscClaim = product.FscClaim,
                    FscCertificate = product.FscCertificate,
                    SellPrice = product.SellPrice,
                    InvoicedQuantity = product.InvoicedQuantity,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Unit = product.Unit,
                    DescriptionId = mainProduct.DescriptionId,
                    GradeId = mainProduct.GradeId,
                    SizeId = mainProduct.SizeId,
                    Description = GetDescriptionName(mainProduct.DescriptionId),
                    Grade = GetGradeName(mainProduct.GradeId),
                    Size = GetSizeName(mainProduct.SizeId)
                });
            }

            return productsForEdit;
        }
    }
}
