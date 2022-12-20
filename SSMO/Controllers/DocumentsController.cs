using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Infrastructure;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.Packing_List;
using SSMO.Models.Documents.Purchase;
using SSMO.Services;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.Documents;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.Documents.Purchase;
using SSMO.Services.MyCompany;
using SSMO.Services.SupplierOrders;
using System;

namespace SSMO.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly ISupplierOrderService supplierOrderService;
        private readonly IPurchaseService purchaseService;
        private readonly ICustomerOrderService customerOrderService;
        private readonly IInvoiceService invoiceService;
        private readonly IMycompanyService mycompanyService;
        private readonly IDocumentService documentService;
        private readonly ISupplierService supplierService;
     
        public DocumentsController
            (ISupplierOrderService supplierOrderService, 
            IPurchaseService purchaseService, ICustomerOrderService customerOrderService,
            IInvoiceService invoiceService, IMycompanyService mycompanyService, 
            IDocumentService documentService, ISupplierService supplierService)
        {
            this.supplierOrderService = supplierOrderService;
            this.purchaseService = purchaseService;
            this.customerOrderService = customerOrderService;
            this.invoiceService = invoiceService;   
            this.mycompanyService = mycompanyService;
            this.documentService = documentService;
            this.supplierService = supplierService;
        }

        public IActionResult AddPurchase(SupplierOrderListModel model)
        {
            if(model.SupplierName != null)
            {
                string userId = this.User.UserId();
                var userIdMyCompany = mycompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

                if (!userIdMyCompany.Contains(userId))
                {
                    return BadRequest();
                }

            }
            var suppliersList = this.supplierOrderService.GetSuppliers();

            var supplierOrdersList = this.purchaseService.GetSupplierOrdersForPurchase(
                model.SupplierName, model.CurrentPage, SupplierOrderListModel.SupplierOrdersPerPage);

            model.SupplierOrderNumbers = supplierOrdersList;
            model.SupplierNames = suppliersList;

            return View(model);  
        }

        [HttpGet]
        public IActionResult PurchaseDetails(string supplierOrderNumber)
        {
            if(supplierOrderNumber != null)
            {
                string userId = this.User.UserId();
                var userIdMyCompany = mycompanyService.GetUserIdMyCompanyBySupplierOrdreNum(supplierOrderNumber);

                if (userIdMyCompany != userId)
                {
                    return BadRequest();
                }
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new PurchaseDetailsFormModel
            {
                SupplierOrderNumber = supplierOrderNumber,
                SupplierFSCCertificate = supplierService.GetSupplierFscCertificateByOrderNumber(supplierOrderNumber)
        });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult PurchaseDetails(string supplierOrderNumber, PurchaseDetailsFormModel model)
        {
            string userId = this.User.UserId();
            string userIdMyCompany = mycompanyService.GetUserIdMyCompanyBySupplierOrdreNum(supplierOrderNumber);
            if (userIdMyCompany != userId)
            {
                return BadRequest();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrEmpty(supplierOrderNumber))
            {
                return BadRequest();
            }

            if(model.GrossWeight < model.NetWeight)
            {
                ModelState.AddModelError("NetWeight", "Net Weight should be less than Gross Weight");
                return View(model);
            }

            var purchase = purchaseService.CreatePurchaseAsPerSupplierOrder(
                supplierOrderNumber, model.Number, model.Date,
                model.PaidStatus, model.NetWeight,
                model.GrossWeight, model.Duty, model.Factoring,
                model.CustomsExpenses, model.FiscalAgentExpenses,
                model.ProcentComission, model.PurchaseTransportCost, 
                model.BankExpenses, model.OtherExpenses, model.Vat, 
                model.TruckNumber,model.FSCSertificate, model.FSCClaim);

            if (!purchase)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CustomerOrderToInvoice()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");

            }
            var customerOrdersList = customerOrderService.AllCustomerOrderNumbers();
            ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice();

            var companiesNames = mycompanyService.GetCompaniesNames();

            var collectionCustomerOrders = new CustomerOrderNumbersListView
            {
                OrderConfirmationNumberList = customerOrdersList,
                MyCompanyNames = companiesNames
            };

            return View(collectionCustomerOrders);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CustomerOrderToInvoice(CustomerOrderNumbersListView model)
        {
            string userId = this.User.UserId();
            string userIdMyCompany = mycompanyService.GetUserIdMyCompanyByName(model.MyCompanyName);

            if (userIdMyCompany != userId)
            {
                return BadRequest();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            model.OrderConfirmationNumberList = customerOrderService.AllCustomerOrderNumbers();
            model.MyCompanyNames = mycompanyService.GetCompaniesNames();
            ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice();

            if (!ModelState.IsValid)
            {
                new CustomerOrderNumbersListView
                {
                    OrderConfirmationNumberList = customerOrderService.AllCustomerOrderNumbers(),
                MyCompanyNames = mycompanyService.GetCompaniesNames()

                 };
                ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice();
            }

            return RedirectToAction("CreateInvoice",
                new
                {
                    orderConfirmationNumber = model.OrderConfirmationNumber,
                    date = model.Date,
                    currencyExchangeRateUsdToBGN = model.CurrencyExchangeRateUsdToBGN,
                    number = model.Number,
                    mycompanyname = model.MyCompanyName,
                    truckNumber = model.TruckNumber,
                    deliveryCost = model.DeliveryCost,
                    grossWeight = model.GrossWeight,
                    netWeight = model.NetWeight
                });
        }

        public IActionResult CreateInvoice(
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN, 
            int number, string mycompanyname, string truckNumber, decimal deliveryCost, decimal grossWeight, decimal netWeight)
        {
            string userId = this.User.UserId();
            string userIdMyCompany = mycompanyService.GetUserIdMyCompanyByName(mycompanyname);
           
            if (userIdMyCompany != userId)
            {
                return BadRequest();
            }
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            var invoiceForPrint = invoiceService.CreateInvoice
                (orderConfirmationNumber, date, currencyExchangeRateUsdToBGN, number, mycompanyname,truckNumber,deliveryCost,
                grossWeight, netWeight);    

            return View(invoiceForPrint);
        }
       
        //TODO Bank Details on the invoice
        public IActionResult BgInvoice(int documentNumber, decimal currencyExchangeRateUsdToBGN)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            //TODO Fix amount with currencyexchange
            var bgInvoice = invoiceService.CreateBgInvoice(documentNumber, currencyExchangeRateUsdToBGN);
            if (bgInvoice == null) return View();

            return View(bgInvoice);
        }
        public IActionResult ChoosePackingListForPrint(ChoosePackingListFromInvoicesViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");

            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var invoiceCollectionNumbers = documentService.GetPackingList();
            if(invoiceCollectionNumbers == null)
            {
                ModelState.AddModelError(string.Empty, "Missing");
                return View(model);
            }
            model.PckingListNumbers = invoiceCollectionNumbers;
            model.PackingListForPrint = documentService.PackingListForPrint(model.PackingListNumber);
            return View(model);
        }

        public IActionResult PackingListForPrint(PackingListForPrintViewModel model, int invoiceNumber)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return View();
        }

    }
}
