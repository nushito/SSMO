using Microsoft.AspNetCore.Mvc;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.Purchase;
using SSMO.Services.CustomerOrderService;
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
  
        public DocumentsController
            (ISupplierOrderService supplierOrderService, 
            IPurchaseService purchaseService, ICustomerOrderService customerOrderService,
            IInvoiceService invoiceService, IMycompanyService mycompanyService)
        {
            this.supplierOrderService = supplierOrderService;
            this.purchaseService = purchaseService;
            this.customerOrderService = customerOrderService;
            this.invoiceService = invoiceService;   
            this.mycompanyService = mycompanyService;
        }


        public IActionResult AddPurchase(SupplierOrderListModel model)
        {
            var suppliersList = this.supplierOrderService.GetSuppliers();

            var supplierOrdersList = this.purchaseService.GetSupplierOrders(
                model.SupplierName, model.CurrentPage, SupplierOrderListModel.SupplierOrdersPerPage);

            model.SupplierOrderNumbers = supplierOrdersList;
            model.SupplierNames = suppliersList;

            return View(model);  
        }

        [HttpGet]
        public IActionResult PurchaseDetails(string supplierOrderNumber)
        {
            return View(new PurchaseDetailsFormModel
            {
                SupplierOrderNumber = supplierOrderNumber
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PurchaseDetails(string supplierOrderNumber, PurchaseDetailsFormModel model)
        {
            if(!ModelState.IsValid)
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
                model.GrossWeight, model.Duty, model.Factoring,model.CustomsExpenses, model.FiscalAgentExpenses,
                model.ProcentComission, model.PurchaseTransportCost, model.BankExpenses, model.OtherExpenses);

            if (!purchase)
            {
                return View();
            }

            return View("Home");
        }

        [HttpGet]
        public IActionResult CustomerOrderToInvoice()
        {
            var customerOrdersList = customerOrderService.AllCustomerOrderNumbers();
            ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice();

            var companiesNames = mycompanyService.GetCompany();

            var collectionCustomerOrders = new CustomerOrderNumbersListView
            {
                OrderConfirmationNumberList = customerOrdersList,
                MyCompanyNames = companiesNames

            };


            return View(collectionCustomerOrders);
        }

        [HttpPost]
        public IActionResult CustomerOrderToInvoice(CustomerOrderNumbersListView model)
        {
            model.OrderConfirmationNumberList = customerOrderService.AllCustomerOrderNumbers();
            model.MyCompanyNames = mycompanyService.GetCompany();

            //if (!ModelState.IsValid)
            //{
            //     new CustomerOrderNumbersListView
            //    {
            //        OrderConfirmationNumberList = customerOrdersList,
            //        MyCompanyNames = companiesNames

            //    };
            //}

            ViewBag.CheckInvoice = invoiceService.CheckFirstInvoice();
            //if (!ViewBag.CheckInvoice)
            //{
            //    model.Number = 0;

            //}

            return RedirectToAction("CreateInvoice",
                new
                {
                    orderConfirmationNumber = model.OrderConfirmationNumber,
                    date = model.Date,
                    currencyExchangeRateUsdToBGN = model.CurrencyExchangeRateUsdToBGN,
                    number = model.Number,
                    mycompanyname = model.MyCompanyName
                });
        }

        public IActionResult CreateInvoice(int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN, int number, string mycompanyname)
        {
            var invoiceForPrint = invoiceService.CreateInvoice(orderConfirmationNumber, date, currencyExchangeRateUsdToBGN, number, mycompanyname);    

            return View(invoiceForPrint);
        }

    }
}
