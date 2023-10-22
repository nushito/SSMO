using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Infrastructure;
using SSMO.Models.ServiceOrders;
using SSMO.Services;
using SSMO.Services.Documents;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.Documents.Purchase;
using SSMO.Services.MyCompany;
using SSMO.Services.TransportService;
using System.Drawing.Text;
using System.Text.Json;

namespace SSMO.Controllers
{
    public class ServiceOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly IMycompanyService mycompanyService;
        private readonly IDocumentService documentService;
        private readonly ICurrency currencyService;
        private readonly IPurchaseService purchaseService;
        private readonly ITransportService  transportService;
        public ServiceOrdersController(ISupplierService supplierService,IMycompanyService mycompanyService,
            IDocumentService documentService, ICurrency currencyService, IPurchaseService purchaseService,
            ITransportService transportService)
        {
            this.supplierService = supplierService;  
            this.mycompanyService = mycompanyService;
            this.documentService = documentService;
            this.currencyService = currencyService;
            this.purchaseService = purchaseService;
            this.transportService = transportService;
        }

        [HttpGet]
        public IActionResult AddServiceOrder()
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            var serviceOrderModel = new ServiceOrderFormModel
            {
                Suppliers = supplierService.GetSuppliers(),
                MyCompanies = mycompanyService.GetCompaniesForTransportOrder(),
                InvoiceDocumentNumbers = documentService.GetInvoiceList(),
                Currencies = currencyService.AllCurrency()
            };

            ViewBag.FirstTransport = transportService.FirstTransport();

            return View(serviceOrderModel); ;
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddServiceOrder(ServiceOrderFormModel model)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }
            return View();
        }

        
        [HttpGet]
        public IActionResult GetPurchaseDocumentNumbers(string id)
        {
            if (id == null)
            {
                id = "0";
            }
            var sellerId = int.Parse(id.ToString());
            var selectedInvoices = purchaseService.GetPurchaseInvoices(sellerId);
            return Json(selectedInvoices, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });
        }
    }
}
