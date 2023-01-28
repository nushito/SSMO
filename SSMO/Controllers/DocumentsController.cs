using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Infrastructure;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.BgInvoice;
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
using System.IO;
using IronPdf;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.RenderTree;
using SSMO.Models.Documents.Invoice;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Hosting;

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
        private readonly IHostingEnvironment _hostingEnv;

        public DocumentsController
            (ISupplierOrderService supplierOrderService,
            IPurchaseService purchaseService, ICustomerOrderService customerOrderService,
            IInvoiceService invoiceService, IMycompanyService mycompanyService,
            IDocumentService documentService, ISupplierService supplierService, IHostingEnvironment hosting)
        {
            this.supplierOrderService = supplierOrderService;
            this.purchaseService = purchaseService;
            this.customerOrderService = customerOrderService;
            this.invoiceService = invoiceService;
            this.mycompanyService = mycompanyService;
            this.documentService = documentService;
            this.supplierService = supplierService;
            hosting = _hostingEnv;
        }

        public IActionResult AddPurchase(SupplierOrderListModel model)
        {
            if (model.SupplierName != null)
            {
                string userId = this.User.UserId();
                var userIdMyCompany = mycompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

                if (!userIdMyCompany.Contains(userId))
                {
                    return BadRequest();
                }

            }
            var suppliersList = this.supplierService.GetSupplierNames();

            var supplierOrdersList = this.purchaseService.GetSupplierOrdersForPurchase(
                model.SupplierName, model.CurrentPage, SupplierOrderListModel.SupplierOrdersPerPage);

            model.SupplierOrderNumbers = supplierOrdersList;
            model.SupplierNames = (System.Collections.Generic.ICollection<string>)suppliersList;

            return View(model);
        }

        [HttpGet]
        public IActionResult PurchaseDetails(string supplierOrderNumber)
        {
            if (supplierOrderNumber != null)
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

            if (model.GrossWeight < model.NetWeight)
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
                model.TruckNumber, model.FSCSertificate, model.FSCClaim, model.Swb);

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
                    number = model.Number,
                    currencyExchangeRateUsdToBGN = model.CurrencyExchangeRateUsdToBGN,
                    mycompanyname = model.MyCompanyName,
                    truckNumber = model.TruckNumber,
                    deliveryCost = model.DeliveryCost,
                    swb = model.Swb
                });
        }

        public IActionResult CreateInvoice(
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN,
          int number, string mycompanyname, string truckNumber, decimal deliveryCost, string swb)
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
                (orderConfirmationNumber, date, currencyExchangeRateUsdToBGN, number, mycompanyname, truckNumber, deliveryCost, swb);
         
            if (invoiceForPrint == null)
            {
                return View();
            }

            return View(invoiceForPrint);
        }
        public IActionResult ChooseBgInvoice(BgInvoiceForPrintChooseModel model)
        {
            string userId = this.User.UserId();
            var companiesCollect = mycompanyService.GetCompaniesUserId();

            if (!companiesCollect.Contains(userId))
            { return BadRequest(); }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            var documentNumbers = documentService.GetBgInvoices();

            if (documentNumbers == null)
            {
                ModelState.AddModelError(string.Empty, "Missing");
                return View(model);
            }
            model.DocumentNumbers = documentNumbers;
            return View(model);
        }
        public IActionResult BgInvoice(int documentNumber)
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
            var bgInvoice = invoiceService.CreateBgInvoiceForPrint(documentNumber);
            if (bgInvoice == null) return View();

            return View(bgInvoice);
        }
        public IActionResult ChoosePackingListForPrint(ChoosePackingListFromInvoicesViewModel model)
        {
            string userId = this.User.UserId();
            var companiesCollect = mycompanyService.GetCompaniesUserId();
            if (!companiesCollect.Contains(userId)) { return BadRequest(); }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var invoiceCollectionNumbers = documentService.GetPackingList();
            if (invoiceCollectionNumbers == null)
            {
                ModelState.AddModelError(string.Empty, "Missing");
                return View(model);
            }
            model.PckingListNumbers = invoiceCollectionNumbers;
            model.PackingListForPrint = documentService.PackingListForPrint(model.PackingListNumber);
            return View(model);
        }

       
    }
}
