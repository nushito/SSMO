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
using SSMO.Models.Documents.Invoice;
using SSMO.Services.Customer;
using SSMO.Data.Models;

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
        private readonly ICustomerService customerService;
        private readonly ICurrency currency;
      
        public DocumentsController
            (ISupplierOrderService supplierOrderService,
            IPurchaseService purchaseService, ICustomerOrderService customerOrderService,
            IInvoiceService invoiceService, IMycompanyService mycompanyService,
            IDocumentService documentService, ISupplierService supplierService, ICreditNoteService creditNoteService,
            IProductService productService, IDebitNoteService debitNoteService, ICustomerService customerService,
            ICurrency currency)
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
            this.customerService= customerService;
            this.currency= currency;    
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
        public IActionResult PurchaseDetails(string supplierOrderNumber, int id)
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

            var purchaseModel = new PurchaseDetailsFormModel
            {
                SupplierOrderNumber = supplierOrderNumber,
                SupplierFSCCertificate = supplierService.GetSupplierFscCertificateByOrderNumber(supplierOrderNumber),
                ProductDetails = new List<PurchaseProductAsSupplierOrderViewModel>()
            };

            var productsBySupplier = purchaseService.Products(id);
            purchaseModel.ProductDetails = productsBySupplier;
            return View(purchaseModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult PurchaseDetails(int id, string supplierOrderNumber, PurchaseDetailsFormModel model)
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

            var purchase = purchaseService.CreatePurchaseAsPerSupplierOrder(id,
                model.Number, model.Date,
                model.PaidStatus, model.NetWeight,
                model.GrossWeight, model.Duty, model.Factoring,
                model.CustomsExpenses, model.FiscalAgentExpenses,
                model.ProcentComission, model.PurchaseTransportCost,
                model.BankExpenses, model.OtherExpenses, model.Vat,
                model.TruckNumber,model.Swb, model.ProductDetails, model.Incoterms,
                model.PaidAvance, model.DatePaidAmount);

            if (!purchase)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult CustomerOrdersForInvoice()
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            var customers = new CustomerOrdersForInvoice
            {
                Customers = customerService.GetCustomerNamesAndId(),
                MyCompanies = mycompanyService.GetCompaniesForInvoice()
            };

            return View(customers);
        }

        [HttpPost]
        public IActionResult CustomerOrdersForInvoice(CustomerOrdersForInvoice model)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return  RedirectToAction("InvoiceDetails",  new
            {
                customerId = model.CustomerId,
                selectedCustomerOrders = model.SelectedCustomerOrders,
                myCompanyId = model.MyCompanyId
            });
        }
        [HttpGet]
        public IActionResult GetCustomerOrders(string id)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            if (id == null)
            {
                id = "0";
            }

            var customerId = int.Parse(id.ToString());
            var customerOrdersNum = customerOrderService.CustomerOrderCollection(customerId);
            return Json(customerOrdersNum, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });
        }

        [HttpGet]
        public IActionResult InvoiceDetails(int customerId, List<int> selectedCustomerOrders, int myCompanyId)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return RedirectToAction("Index", "Home"); }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

           ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice(myCompanyId);

            var companiesNames = mycompanyService.GetCompaniesNames();
           
            var collectionCustomerOrders = new CustomerOrderNumbersListView
            {
                SelectedCustomerOrders = selectedCustomerOrders,
                MyCompanyNames = companiesNames,
                Products = new List<ProductsForInvoiceViewModel>(),
                Currencies = currency.AllCurrency(),
                CustomerId= customerId
            };

            collectionCustomerOrders.Products = productService.ProductsForInvoice(selectedCustomerOrders);

            ViewBag.Orders = selectedCustomerOrders;
            TempData["orders"] = JsonConvert.SerializeObject(selectedCustomerOrders);

            return View(collectionCustomerOrders);
        }


        [HttpPost]
        [Authorize]
        public IActionResult InvoiceDetails
            (CustomerOrderNumbersListView model, int customerId, int myCompanyId) 
            
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

            model.MyCompanyNames = mycompanyService.GetCompaniesNames();
           ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice(myCompanyId);
           
            if (!ModelState.IsValid)
            {
                new CustomerOrderNumbersListView
                {
                    SelectedCustomerOrders = JsonConvert.DeserializeObject<List<int>>(TempData["orders"].ToString()),
                    MyCompanyNames = mycompanyService.GetCompaniesNames(),
                    Products = new List<ProductsForInvoiceViewModel>(),
                    Currencies = currency.AllCurrency(),
                    CustomerId = customerId
                };
                ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice(myCompanyId);
            }

            foreach (var product in model.Products)
            {
                product.Amount = product.SellPrice * product.InvoicedQuantity;
                product.BgAmount = product.Amount * model.CurrencyExchangeRateUsdToBGN;
                product.TotalSheets = product.Pallets * product.SheetsPerPallet;
            }

            TempData["products"] = JsonConvert.SerializeObject(model.Products);

            return RedirectToAction("CreateInvoice",
                new
                {
                    date = model.Date,
                    number = model.Number,
                    currencyExchangeRateUsdToBGN = model.CurrencyExchangeRateUsdToBGN,
                    mycompanyname = model.MyCompanyName,
                    truckNumber = model.TruckNumber,
                    deliveryCost = model.DeliveryCost,
                    swb = model.Swb,                                      
                    netWeight = model.NetWeight,
                    grossWeight = model.GrossWeight,
                    incoterms = model.Incoterms,
                    customerId= customerId,
                    currencyId = model.CurrencyId,
                    vat = model.Vat,
                    myCompanyId = myCompanyId
                });
        }

        public IActionResult CreateInvoice(
            DateTime date, decimal currencyExchangeRateUsdToBGN,
          int number, string mycompanyname, string truckNumber, decimal deliveryCost, 
          string swb, decimal netWeight, decimal grossWeight, string incoterms, 
          int customerId, int currencyId, int vat, int myCompanyId)
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

            List<ProductsForInvoiceViewModel> products = JsonConvert.DeserializeObject<List<ProductsForInvoiceViewModel>>(TempData["products"].ToString());
            List<int> orders = JsonConvert.DeserializeObject<List<int>>(TempData["orders"].ToString());

            var invoiceForPrint = invoiceService.CreateInvoice
                (orders, products, date, currencyExchangeRateUsdToBGN, number, 
                mycompanyname, truckNumber, deliveryCost, swb, netWeight, grossWeight, incoterms, 
                customerId, currencyId, vat, myCompanyId);
         
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
            (int invoiceId, DateTime date, bool quantityBack, string deliveryAddress)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }
            if (invoiceId == 0) return BadRequest();
         
            List<AddProductsToCreditAndDebitNoteFormModel> productsForCredit = JsonConvert.DeserializeObject<List<AddProductsToCreditAndDebitNoteFormModel>>(TempData["products"].ToString());
            var creditNote = creditNoteService.CreateCreditNote(invoiceId, date, quantityBack, deliveryAddress, productsForCredit);

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
                new { invoiceId = model.InvoiceId, date = model.Date, 
                    moreQuantity = model.MoreQuantity });
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
