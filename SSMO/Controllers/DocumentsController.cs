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
using SSMO.Models.Documents.CreditNote;
using SSMO.Services.Documents.Credit_Note;
using System.Collections.Generic;
using SSMO.Services.Products;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Documents;
using SSMO.Services.Documents.DebitNote;

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
        private readonly ICreditNoteService creditNoteService;
        private readonly IProductService productService;
        private readonly IDebitNoteService debitNoteService;
      
        public DocumentsController
            (ISupplierOrderService supplierOrderService,
            IPurchaseService purchaseService, ICustomerOrderService customerOrderService,
            IInvoiceService invoiceService, IMycompanyService mycompanyService,
            IDocumentService documentService, ISupplierService supplierService, ICreditNoteService creditNoteService,
            IProductService productService, IDebitNoteService debitNoteService)
        {
            this.supplierOrderService = supplierOrderService;
            this.purchaseService = purchaseService;
            this.customerOrderService = customerOrderService;
            this.invoiceService = invoiceService;
            this.mycompanyService = mycompanyService;
            this.documentService = documentService;
            this.supplierService = supplierService;        
            this.creditNoteService = creditNoteService;
            this.productService= productService;
            this.debitNoteService= debitNoteService;
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

        [HttpGet]
        public IActionResult ChooseInvoiceForCreditNote()
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            var model = new ChooseInvoiceForCreditNoteViewModel
            {              
                MyCompanies = mycompanyService.GetAllCompanies(),
                Products = new List<AddProductsToCreditAndDebitNoteFormModel>()               
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ChooseInvoiceForCreditNote
            (ChooseInvoiceForCreditNoteViewModel model,IFormCollection collection)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            if(!ModelState.IsValid) 
            { 
                return View(new ChooseInvoiceForCreditNoteViewModel
                {
                    MyCompanies = mycompanyService.GetAllCompanies(),
                    Products = new List<AddProductsToCreditAndDebitNoteFormModel>()
                }); 
            }
            model.Products = new List<AddProductsToCreditAndDebitNoteFormModel>();

            int loopsNum = 0;

            foreach (var key in collection.Keys)
            {
                if (key.Contains("Description"))
                {
                    loopsNum++;
                }
            }

            for (int i = 1; i <= loopsNum; i++)
            {
                var description = collection["DescriptionId["+ i +"]"];
                var grade = collection["GradeId["+ i +"]"];
                var size = collection["SizeId["+ i +"]"];
                var unit = collection["Unit["+ i +"]"];
                var price = collection["Price["+ i +"]"].ToString();
                var pallets = collection["Pallets["+ i +"]"].ToString();
                var sheetsPerPallet = collection["SheetsPerPallet[" + i + "]"].ToString();
                var fscClaim = collection["FscClaim["+i+"]"];
                var fscCertificate = collection["FscCertificate[" + i + "]"];
                var quantity = collection["Quantity["+ i +"]"].ToString();
                var product = new AddProductsToCreditAndDebitNoteFormModel
                {
                    DescriptionId = int.Parse(description.ToString()),
                    GradeId = int.Parse(grade.ToString()),
                    SizeId = int.Parse(size.ToString()),
                    Unit = unit,
                    Price = decimal.Parse(price.ToString()),
                    FscClaim = fscClaim,    
                    FscSertificate = fscCertificate,
                    Quantity = decimal.Parse(quantity.ToString()),
                    Pallets = int.Parse(pallets.ToString()),
                    SheetsPerPallet = int.Parse(sheetsPerPallet.ToString())
                };
                model.Products.Add(product);
            }

            TempData["products"] = JsonConvert.SerializeObject(model.Products);

            return RedirectToAction("CreateCreditNote",
                new { invoiceId = model.InvoiceId, date = model.Date, quantityBack = model.QuantityBack});
        }

        public IActionResult CreateCreditNote
            (int invoiceId, DateTime date, bool quantityBack)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }
            if (invoiceId == 0) return BadRequest();
         // 
            List<AddProductsToCreditAndDebitNoteFormModel> productsForCredit = JsonConvert.DeserializeObject<List<AddProductsToCreditAndDebitNoteFormModel>>(TempData["products"].ToString());
            var creditNote = creditNoteService.CreateCreditNote(invoiceId, date, quantityBack, productsForCredit);

            return View(creditNote);  
        }

        [HttpGet]
        public IActionResult ChooseInvoiceForDebitNote()
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }
         
            var model = new DebitNoteChooseInvoiceViewModel
            {
                MyCompanies = mycompanyService.GetAllCompanies(),
                Products = new List<AddProductsToCreditAndDebitNoteFormModel>()
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult ChooseInvoiceForDebitNote(DebitNoteChooseInvoiceViewModel model, IFormCollection collections)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }
            if (!ModelState.IsValid)
            {
                return View(new DebitNoteChooseInvoiceViewModel
                {
                    MyCompanies = mycompanyService.GetAllCompanies(),
                    Products = new List<AddProductsToCreditAndDebitNoteFormModel>()
                });
            }

            model.Products = new List<AddProductsToCreditAndDebitNoteFormModel>();
            int loopsNum = 0;

            foreach (var key in collections.Keys)
            {
                if (key.Contains("Description"))
                {
                    loopsNum++;
                }
            }

            for (int i = 1; i <= loopsNum; i++)
            {
                var description = collections["DescriptionId[" + i + "]"];
                var grade = collections["GradeId[" + i + "]"];
                var size = collections["SizeId[" + i + "]"];
                var unit = collections["Unit[" + i + "]"];
                var price = collections["Price[" + i + "]"].ToString();                
                var fscClaim = collections["FscClaim[" + i + "]"];
                var fscCertificate = collections["FscCertificate[" + i + "]"];
                var quantity = collections["Quantity[" + i + "]"].ToString();
                var product = new AddProductsToCreditAndDebitNoteFormModel
                {
                    DescriptionId = int.Parse(description.ToString()),
                    GradeId = int.Parse(grade.ToString()),
                    SizeId = int.Parse(size.ToString()),
                    Unit = unit,
                    Price = decimal.Parse(price.ToString()),
                    FscClaim = fscClaim,
                    FscSertificate = fscCertificate,
                    Quantity = decimal.Parse(quantity.ToString())                    
                };
                model.Products.Add(product);
            }

            TempData["products"] = JsonConvert.SerializeObject(model.Products);

            return RedirectToAction("CreateDebitNote",
                new { invoiceId = model.InvoiceId, date = model.Date, moreQuantity = model.MoreQuantity });
        }

        public IActionResult CreateDebitNote(int invoiceId, DateTime date, bool moreQuantity)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }
            if (invoiceId == 0) return BadRequest();
           
            List<AddProductsToCreditAndDebitNoteFormModel> productsForDebit = JsonConvert.DeserializeObject<List<AddProductsToCreditAndDebitNoteFormModel>>(TempData["products"].ToString());
            var debitNoteForPrint = debitNoteService.CreateDebitNote(invoiceId, date, moreQuantity,productsForDebit);

            return View(debitNoteForPrint);
        }

        [HttpGet]
        public IActionResult GetInvoiceNumbers(string id)
        {
            if (id == null)
            {
                id = "0";
            }
            var sellerId = int.Parse(id.ToString());           
            var selectedInvoices = invoiceService.GetInvoiceDocumentNumbers(sellerId);
            return Json(selectedInvoices, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });
        }
       
       //TODO This is not working!!!
        public IActionResult GetProductsForDebitNote(string id)
        {
            if (id == null) { id = "0"; }
            
          //  var productList = 
            return Json(new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });
        }
    }
}
