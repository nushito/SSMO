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
        public bool CreatePurchaseAsPerSupplierOrder(
            string supplierOrderNumber, string number, DateTime date, bool paidStatus,
            decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses, 
            int vat, string truckNumber, string fscCertificate, string fscClaim, string swb)
        {
            var supplierOrder = dbContext.SupplierOrders.FirstOrDefault(o => o.Number.ToLower() == supplierOrderNumber.ToLower());
            var amount = supplierOrder.Amount;
            //var totalAmount = supplierOrder.TotalAmount;
            var customerOrder = dbContext.CustomerOrders.Where(sp => sp.Id == supplierOrder.CustomerOrderId)
                                 .FirstOrDefault();
            customerOrder.NetWeight = netWeight;
            customerOrder.GrossWeight = brutoWeight;
            supplierOrder.NetWeight = netWeight;
            supplierOrder.GrossWeight = brutoWeight;

            var purchase = new Document
            {
                PurchaseNumber = number,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.Purchase,
                SupplierOrder = supplierOrder,
                SupplierOrderId = supplierOrder.Id,
                CustomerOrderId = supplierOrder.CustomerOrderId,
                PaidStatus = paidStatus,
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
                Amount = amount,
                CurrencyId = supplierOrder.CurrencyId,
                Incoterms = customerOrder.DeliveryTerms,
                Products = new List<Product>(),
                Vat = vat,
                VatAmount = supplierOrder.VatAmount,
                TotalAmount = supplierOrder.TotalAmount,
                TruckNumber = truckNumber,
                Swb= swb,
                SupplierId = supplierOrder.SupplierId,
                MyCompanyId = supplierOrder.MyCompanyId,
                FSCClaim = fscClaim,
                FSCSertificate = fscCertificate,
                PurchaseProducts = new List<Product>()
            };

           
            var productList = dbContext.Products.Where(s => s.SupplierOrderId == supplierOrder.Id).ToList();
           
            if (purchase == null)
            {
                return false;
            }

            if (purchase.PaidStatus == true)
            {
                supplierOrder.PaidStatus = true;
                supplierOrder.Balance = 0;
                purchase.Balance = 0;
                purchase.PaidAvance = purchase.Amount;
                supplierOrder.PaidAvance = purchase.Amount;
            }
            else
            {
                purchase.Balance = purchase.TotalAmount - purchase.PaidAvance;
            }

            dbContext.Documents.Add(purchase);
            dbContext.SaveChanges();

            var expenses = purchase.Duty + purchase.Factoring * amount / 100 +
                      purchase.CustomsExpenses + purchase.FiscalAgentExpenses +
                      purchase.ProcentComission * amount / 100 + purchase.PurchaseTransportCost +
                      purchase.BankExpenses + purchase.OtherExpenses;

            foreach (var product in productList)
            {
                product.LoadedQuantityM3 = product.OrderedQuantity;
                product.PurchaseTransportCost = purchase.PurchaseTransportCost / supplierOrder.TotalQuantity * product.LoadedQuantityM3;
                product.CostPrice = (product.PurchaseAmount + (expenses / supplierOrder.TotalQuantity * product.LoadedQuantityM3)) / product.LoadedQuantityM3;
                product.PurchaseDocumentId = purchase.Id;
                purchase.PurchaseProducts.Add(product);
            }
            dbContext.SaveChanges();
            return true;
        }

        public bool EditPurchaseInvoice(int id, string number, DateTime date, int supplierOrderId, 
            int vat, decimal netWeight, decimal grossWeight, string truckNumber, string swb, 
            decimal purchaseTransportCost, decimal bankExpenses, decimal duty, decimal customsExpenses, 
            decimal factoring, decimal fiscalAgentExpenses, decimal procentComission, decimal otherExpenses)
        {
            if(id == 0) { return false; }   

            var purchaseInvoiceForEdit = dbContext.Documents
                .Where(i=>i.Id == id)
                .FirstOrDefault();

            if(purchaseInvoiceForEdit == null) { return false; }

            if(purchaseInvoiceForEdit.SupplierOrderId != supplierOrderId)
            {
                var oldProductList = dbContext.Products
                    .Where(s=>s.SupplierOrderId== purchaseInvoiceForEdit.SupplierOrderId)
                    .ToList();

                foreach (var product in oldProductList)
                {
                    productService.ResetToNullLoadingQuantityIfPurchaseIsChanged(product.Id);
                }

                var newProductList = dbContext.Products
                    .Where(s => s.SupplierOrderId == supplierOrderId)
                    .ToList();

                foreach (var product in newProductList) 
                {
                    productService.NewLoadingQuantityToEditPurchase(product.Id, id);
                }

                purchaseInvoiceForEdit.SupplierOrderId = supplierOrderId;

            }

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

        public PurchaseInvoiceDetailsViewModel PurchaseDetailsForEdit(int id)
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
                Number = purchaseInvoice.PurchaseNumber,
                OtherExpenses = purchaseInvoice.OtherExpenses,
                ProcentComission = purchaseInvoice.ProcentComission,
                PurchaseTransportCost = purchaseInvoice.PurchaseTransportCost,
                SupplierOrderNumber = supplierOrderNumber,
                TruckNumber = purchaseInvoice.TruckNumber,
                Swb = purchaseInvoice.Swb,
                Vat = purchaseInvoice.Vat ?? 0,
                Products = new List<PurchaseProductsDetailsViewModel>()
            };

            var products = dbContext.Products
                .Where(pi => pi.PurchaseDocumentId == purchaseInvoice.Id)
                .ToList();

            foreach (var product in products)
            {
                purchaseForEdit.Products.Add(
                    new PurchaseProductsDetailsViewModel
                {
                    Description = productService.GetDescriptionName(product.DescriptionId),
                    Grade = productService.GetGradeName(product.GradeId),
                    Size = productService.GetSizeName(product.SizeId),
                    FscCertificate = product.FSCSertificate,
                    FscClaim = product.FSCClaim,
                    Pallet = product.Pallets,
                    Quantity = product.LoadedQuantityM3,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Unit = product.Unit.ToString(),
                    PurchasePrice = product.PurchasePrice
                });
            }
            return purchaseForEdit;
        }
    }
}
