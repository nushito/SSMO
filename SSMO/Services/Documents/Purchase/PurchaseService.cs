using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents.Purchase;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.Purchase;
using SSMO.Services.Products;
using SSMO.Services.SupplierOrders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Documents.Purchase
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        private readonly IProductService productService;
        private readonly ISupplierOrderService supplierOrderService;

        public PurchaseService(ApplicationDbContext dbContext, IConfigurationProvider mapper, IProductService productService,
            ISupplierOrderService supplierOrderService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productService;
            this.supplierOrderService = supplierOrderService;
        }
        public bool CreatePurchaseAsPerSupplierOrder(int id,
            string number, DateTime date, bool paidStatus,
            decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses, 
            int vat, string truckNumber, string swb,List<PurchaseProductAsSupplierOrderViewModel> products, 
            string incoterms, decimal paidAdvance, DateTime? dateOfPayment)
        {
            var supplierOrder = dbContext.SupplierOrders.FirstOrDefault
                (o => o.Id == id);

            if (supplierOrder == null) { return false; }

            var purchase = new Document
            {
                PurchaseNumber = number,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.Purchase,
                SupplierOrder = supplierOrder,
                SupplierOrderId = supplierOrder.Id,
                PaidStatus = paidStatus,
                PaidAvance = paidAdvance,
                DatePaidAmount= dateOfPayment,
                NetWeight = netWeight,
                GrossWeight = brutoWeight,
                Duty = duty,
                Factoring = factoring,
                CustomsExpenses = customsExpenses,                
                FiscalAgentExpenses = fiscalAgentExpenses,
                ProcentComission = procentComission,
                PurchaseTransportCost = purchaseTransportCost,
                BankExpenses = bankExpenses,
                OtherExpenses = otherExpenses,
                CurrencyId = supplierOrder.CurrencyId,
                Incoterms = incoterms,
                Vat = vat,
                TruckNumber = truckNumber,
                Swb= swb,
                SupplierId = supplierOrder.SupplierId,
                MyCompanyId = supplierOrder.MyCompanyId,
                PurchaseProducts = new List<PurchaseProductDetails>(),
                CustomerOrderProducts = new List<CustomerOrderProductDetails>()
            };

            dbContext.Documents.Add(purchase);
            dbContext.SaveChanges();

            foreach (var product in products)
            {
                if(product.OrderedQuantity == 0) { continue; }

                var mainProduct = dbContext.Products.Where(s => s.Id == product.Id).FirstOrDefault();   
                mainProduct.PurchaseProductDetails = new List<PurchaseProductDetails>();

                if(product.OrderedQuantity > mainProduct.OrderedQuantity) { return false; }

                var purchaseProduct = new PurchaseProductDetails
                {
                    ProductId = mainProduct.Id,
                    PurchaseInvoiceId = purchase.Id,
                    PurchasePrice = product.PurchasePrice,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Unit = product.Unit,
                    SupplierOrderId = supplierOrder.Id,
                    FscCertificate = product.PurchaseFscCertificate,
                    FscClaim = product.PurchaseFscClaim,
                    Quantity = (decimal) product.OrderedQuantity,
                    VehicleNumber = product.VehicleNumber
                };
                purchaseProduct.Amount = purchaseProduct.Quantity*purchaseProduct.PurchasePrice;
                purchaseProduct.TotalSheets = purchaseProduct.Pallets*purchaseProduct.SheetsPerPallet;

                mainProduct.PurchaseProductDetails.Add(purchaseProduct);
                mainProduct.LoadedQuantityM3 += purchaseProduct.Quantity;
                mainProduct.QuantityLeftForPurchaseLoading = mainProduct.OrderedQuantity - purchaseProduct.Quantity;
                purchase.PurchaseProducts.Add(purchaseProduct);

                var customerOrder = dbContext.CustomerOrderProductDetails
              .Where(c => c.SupplierOrderId == supplierOrder.Id && c.ProductId == mainProduct.Id)
              .FirstOrDefault();

                purchase.CustomerOrderProducts.Add(customerOrder);
            }
            purchase.Amount = purchase.PurchaseProducts.Sum(purchaseProduct => purchaseProduct.Amount);
            purchase.VatAmount = purchase.Amount * purchase.Vat / 100;
            purchase.TotalAmount = purchase.Amount + purchase.VatAmount ?? 0;
           
            if (purchase == null)
            {
                return false;
            }

            if (purchase.PaidStatus == true)
            {
                purchase.Balance = 0;
                purchase.PaidAvance = purchase.Amount;
            }
            else
            {
                if(paidAdvance > 0)
                {
                    purchase.Balance = purchase.TotalAmount - paidAdvance;
                }
                else
                {
                    purchase.Balance = purchase.TotalAmount;
                }
               
            }

            var expenses = purchase.Duty + purchase.Factoring * purchase.Amount / 100 +
                      purchase.CustomsExpenses + purchase.FiscalAgentExpenses +
                      purchase.ProcentComission * purchase.Amount / 100 + purchase.PurchaseTransportCost +
                      purchase.BankExpenses + purchase.OtherExpenses;

          purchase.TotalQuantity = purchase.PurchaseProducts.Sum(p => p.Quantity);

            foreach (var product in purchase.PurchaseProducts)
            {
                var transportCost = purchase.PurchaseTransportCost / purchase.TotalQuantity * product.Quantity;
                product.CostPrice = (product.Amount + (expenses / purchase.TotalQuantity * product.Quantity)) / product.Quantity;
             
            }

            var productsBySupplierOrder = dbContext.Products
                .Where(p => p.SupplierOrderId == supplierOrder.Id).ToList();

            if(productsBySupplierOrder.Sum(a=>a.OrderedQuantity) == productsBySupplierOrder.Sum(b => b.LoadedQuantityM3))
            {
                supplierOrder.StatusId = dbContext.Statuses
                    .Where(a=>a.Name == "Finished")
                    .Select(i=>i.Id)
                    .FirstOrDefault();
            }

            
            dbContext.SaveChanges();
            return true;
        }

        public bool EditPurchaseInvoice(int id, string number, DateTime date, int supplierOrderId, 
            int vat, decimal netWeight, decimal grossWeight, string truckNumber, string swb, 
            decimal purchaseTransportCost, decimal bankExpenses, decimal duty, decimal customsExpenses, 
            decimal factoring, decimal fiscalAgentExpenses, decimal procentComission, decimal otherExpenses, 
            List<PurchaseProductsForEditFormModel> purchaseProducts)
        {
            if(id == 0) { return false; }   

            var purchaseInvoiceForEdit = dbContext.Documents
                .Where(i=>i.Id == id)
                .FirstOrDefault();

            if(purchaseInvoiceForEdit == null) { return false; }

            purchaseInvoiceForEdit.PurchaseNumber= number;
            purchaseInvoiceForEdit.Date= date;           
            purchaseInvoiceForEdit.Vat= vat;
            purchaseInvoiceForEdit.NetWeight = netWeight;
            purchaseInvoiceForEdit.GrossWeight = grossWeight;
            purchaseInvoiceForEdit.TruckNumber = truckNumber;
            purchaseInvoiceForEdit.Swb= swb;
            purchaseInvoiceForEdit.PurchaseTransportCost= purchaseTransportCost;
            purchaseInvoiceForEdit.BankExpenses= bankExpenses;
            purchaseInvoiceForEdit.Duty= duty;
            purchaseInvoiceForEdit.CustomsExpenses= customsExpenses;
            purchaseInvoiceForEdit.Factoring = factoring;
            purchaseInvoiceForEdit.FiscalAgentExpenses= fiscalAgentExpenses;
            purchaseInvoiceForEdit.ProcentComission= procentComission;
            purchaseInvoiceForEdit.OtherExpenses= otherExpenses;

            var expenses = purchaseInvoiceForEdit.Duty + purchaseInvoiceForEdit.Factoring * purchaseInvoiceForEdit.Amount / 100 +
                   purchaseInvoiceForEdit.CustomsExpenses + purchaseInvoiceForEdit.FiscalAgentExpenses +
                   purchaseInvoiceForEdit.ProcentComission * purchaseInvoiceForEdit.Amount / 100 + purchaseInvoiceForEdit.PurchaseTransportCost +
                   purchaseInvoiceForEdit.BankExpenses + purchaseInvoiceForEdit.OtherExpenses;

            purchaseInvoiceForEdit.TotalQuantity = purchaseInvoiceForEdit.PurchaseProducts.Sum(p => p.Quantity);


            foreach (var product in purchaseProducts)
            {
                var productForEdit = dbContext.PurchaseProductDetails
                   .Where(s => s.Id == product.Id)
                   .FirstOrDefault();

                var mainProduct = dbContext.Products
                    .Where(a=> a.Id == product.ProductId).FirstOrDefault();
                if(mainProduct.OrderedQuantity < product.Quantity)
                {
                    return false;
                }
               
                mainProduct.LoadedQuantityM3 -= productForEdit.Quantity;
                mainProduct.LoadedQuantityM3 += product.Quantity;
                mainProduct.QuantityLeftForPurchaseLoading = mainProduct.OrderedQuantity - mainProduct.LoadedQuantityM3;
                
                productForEdit.PurchasePrice = product.PurchasePrice;
                productForEdit.SheetsPerPallet = product.SheetsPerPallet;   
                productForEdit.Pallets = product.Pallets;
                productForEdit.FscCertificate = product.FscCertificate;
                productForEdit.FscClaim = product.FscClaim;
                productForEdit.Quantity = product.Quantity;
                productForEdit.Amount = product.Quantity * product.PurchasePrice;

                var transportCost = purchaseInvoiceForEdit.PurchaseTransportCost / purchaseInvoiceForEdit.TotalQuantity * product.Quantity;
                productForEdit.CostPrice = (productForEdit.Amount + (expenses / purchaseInvoiceForEdit.TotalQuantity * product.Quantity)) / product.Quantity;
            }
           
            var productsBySupplierOrder = dbContext.Products
                .Where(p => p.SupplierOrderId == supplierOrderId).ToList();

            var supplierOrder = dbContext.SupplierOrders
                .Where(i=>i.Id == purchaseInvoiceForEdit.SupplierOrderId).FirstOrDefault();

            if (productsBySupplierOrder.Sum(a => a.OrderedQuantity) == productsBySupplierOrder.Sum(b => b.LoadedQuantityM3))
            {
                supplierOrder.StatusId = dbContext.Statuses
                    .Where(a => a.Name == "Finished")
                    .Select(i => i.Id)
                    .FirstOrDefault();
            }
            dbContext.SaveChanges();
            return true;
        }

        public bool EditPurchasePayment
            (string number, bool paidStatus, decimal paidAvance, DateTime datePaidAmount)
        {
            if (number == null)
            {
                return false;
            }

            var purchase = dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Purchase && type.PurchaseNumber.ToLower() == number.ToLower())
                .FirstOrDefault(); 
           
            purchase.PaidStatus = paidStatus;
            purchase.PaidAvance = paidAvance;
            purchase.DatePaidAmount = datePaidAmount;
            purchase.Balance = purchase.TotalAmount - purchase.PaidAvance;
            if (purchase.Balance == 0)
            {
                purchase.PaidStatus = true;
            }
            else
            {
                purchase.PaidStatus = false;
            }

            return true;
        }
        public EditPurchasePaymentDetails GetPurchaseForPaymentEdit(string number)
        {
            var purchase = dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Purchase && type.PurchaseNumber.ToLower() == number.ToLower());

            var purchaseForEdit = purchase.ProjectTo<EditPurchasePaymentDetails>(mapper).FirstOrDefault();

            return purchaseForEdit;
        }
        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrdersForPurchase(string supplierName
            , int currentpage, int supplierOrdersPerPage)
        {
            if (String.IsNullOrEmpty(supplierName))
            {
                return new List<PurchaseModelAsPerSpec>();
            }

            var supplierId = dbContext.Suppliers.Where(a => a.Name.ToLower() == supplierName.ToLower())
                .Select(a => a.Id).FirstOrDefault();

            var queryOrders = dbContext.SupplierOrders.
                Where(a => a.SupplierId == supplierId).OrderByDescending(a => a.Date);

            var totalOrders = queryOrders.Count();

            var orders = queryOrders.ProjectTo<PurchaseModelAsPerSpec>(this.mapper).ToList();

            var supplierOrdersList = orders.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage);

            return supplierOrdersList;
        }

        public List<PurchaseProductAsSupplierOrderViewModel> Products(int id)
        {
            
            var products = dbContext.Products.Where(s => s.SupplierOrderId == id);
            var productDetails = products.ProjectTo<PurchaseProductAsSupplierOrderViewModel>(mapper).ToList();
            foreach (var product in productDetails)
            {
                product.Description = productService.GetDescriptionName(product.DescriptionId);
                product.Grade = productService.GetGradeName(product.GradeId);
                product.Size = productService.GetSizeName(product.SizeId);
            }
            return productDetails;
        }

        public PurchaseInvoiceDetailsViewModel PurchaseDetails(int id)
        {
            if(id == 0)
            {
                return null;
            }

            var purchaseInvoice = dbContext.Documents
                .Where(a => a.Id == id)
                .FirstOrDefault();

            var supplierOrderNumber = dbContext.SupplierOrders
                .Where(i=>i.Id == purchaseInvoice.SupplierOrderId)
                .Select(n=>n.Number)
                .FirstOrDefault();

          
            var purchaseForEdit = new PurchaseInvoiceDetailsViewModel
            {
                BankExpenses = purchaseInvoice.BankExpenses,
                CustomsExpenses = purchaseInvoice.CustomsExpenses,
                Date = purchaseInvoice.Date,
                Duty = purchaseInvoice.Duty,
                Factoring = purchaseInvoice.Factoring,
                FiscalAgentExpenses = purchaseInvoice.FiscalAgentExpenses,
                GrossWeight = purchaseInvoice.GrossWeight,
                Incoterms = purchaseInvoice.Incoterms,
                NetWeight = purchaseInvoice.NetWeight,
                PurchaseNumber = purchaseInvoice.PurchaseNumber,
                OtherExpenses = purchaseInvoice.OtherExpenses,
                ProcentComission = purchaseInvoice.ProcentComission,
                PurchaseTransportCost = purchaseInvoice.PurchaseTransportCost,
                SupplierOrderId= purchaseInvoice.SupplierOrderId,
                TruckNumber = purchaseInvoice.TruckNumber,
                Swb = purchaseInvoice.Swb,
                Vat = purchaseInvoice.Vat ?? 0,
                Products = new List<PurchaseProductsDetailsViewModel>()
            };

            purchaseForEdit.SupplierOrderNumber = dbContext.SupplierOrders
                .Where(a => a.Id == purchaseInvoice.SupplierOrderId)
                .Select(n => n.Number)
                .FirstOrDefault();

            var products = dbContext.PurchaseProductDetails
                .Where(pi => pi.PurchaseInvoiceId == id)
                .ToList();

            foreach (var product in products)
            {
                var mainProduct = dbContext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                purchaseForEdit.Products.Add(
                    new PurchaseProductsDetailsViewModel
                    {
                        Description = productService.GetDescriptionName(mainProduct.DescriptionId),
                        Grade = productService.GetGradeName(mainProduct.GradeId),
                        Size = productService.GetSizeName(mainProduct.SizeId),
                        FscCertificate = product.FscCertificate,
                        FscClaim = product.FscClaim,
                        Pallets = product.Pallets,
                        Quantity = product.Quantity,
                        SheetsPerPallet = product.SheetsPerPallet,
                        Unit = product.Unit.ToString(),
                        PurchasePrice = product.PurchasePrice,
                        CostPrice= product.CostPrice,
                        TotalSheets= product.TotalSheets
                    });
            }
            return purchaseForEdit;
        }

        public EditPurchaseViewModel PurchaseDetailsForEdit(int id)
        {
            if (id == 0)
            {
                return null;
            }

            var purchaseInvoice = dbContext.Documents
                .Where(a => a.Id == id)
                .FirstOrDefault();

            var supplierOrderNumbers = dbContext.SupplierOrders;                               

            var supplierOrdersNumbersUpdate = supplierOrderNumbers.ProjectTo<SupplierOrdersListForPurchaseEditModel>(mapper).ToList();

            var purchaseForEdit = new EditPurchaseViewModel
            {
                BankExpenses = purchaseInvoice.BankExpenses,
                CustomsExpenses = purchaseInvoice.CustomsExpenses,
                Date = purchaseInvoice.Date,
                Duty = purchaseInvoice.Duty,
                Factoring = purchaseInvoice.Factoring,
                FiscalAgentExpenses = purchaseInvoice.FiscalAgentExpenses,
                GrossWeight = purchaseInvoice.GrossWeight,
                Incoterms = purchaseInvoice.Incoterms,
                NetWeight = purchaseInvoice.NetWeight,
                PurchaseNumber = purchaseInvoice.PurchaseNumber,
                OtherExpenses = purchaseInvoice.OtherExpenses,
                ProcentComission = purchaseInvoice.ProcentComission,
                PurchaseTransportCost = purchaseInvoice.PurchaseTransportCost,
                SupplierOrderId = purchaseInvoice.SupplierOrderId,
                SupplierOrders = supplierOrdersNumbersUpdate,
                TruckNumber = purchaseInvoice.TruckNumber,
                Swb = purchaseInvoice.Swb,
                Vat = purchaseInvoice.Vat ?? 0,
                PurchaseProducts = new List<PurchaseProductsForEditFormModel>()
            };

            var products = dbContext.PurchaseProductDetails
                .Where(a => a.PurchaseInvoiceId == id);               

            purchaseForEdit.PurchaseProducts = products.ProjectTo< PurchaseProductsForEditFormModel >(mapper).ToList();

            foreach (var product in purchaseForEdit.PurchaseProducts)
            {
                var mainProduct = dbContext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                product.Description = productService.GetDescriptionName(mainProduct.DescriptionId);
                product.Grade = productService.GetGradeName(mainProduct.GradeId);
                product.Size = productService.GetSizeName(mainProduct.SizeId);
            }

            return purchaseForEdit; 
        }
    }
}
