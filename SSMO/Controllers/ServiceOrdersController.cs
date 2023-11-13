using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data.Models;
using SSMO.Infrastructure;
using SSMO.Models.ServiceOrders;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.Documents;
using SSMO.Services.Documents.Purchase;
using SSMO.Services.FiscalAgent;
using SSMO.Services.MyCompany;
using SSMO.Services.SupplierOrders;
using SSMO.Services.TransportService;
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
        private readonly ICustomerService customerService;
        private readonly ISupplierOrderService supplierOrderService;
        private readonly ICustomerOrderService customerOrderService;
        private readonly IFiscalAgentService fiscalAgentService;
        public ServiceOrdersController(ISupplierService supplierService,IMycompanyService mycompanyService,
            IDocumentService documentService, ICurrency currencyService, IPurchaseService purchaseService,
            ITransportService transportService, ICustomerService customerService, 
            ISupplierOrderService supplierOrderService, ICustomerOrderService customerOrderService,
            IFiscalAgentService fiscalAgentService)
        {
            this.supplierService = supplierService;  
            this.mycompanyService = mycompanyService;
            this.documentService = documentService;
            this.currencyService = currencyService;
            this.purchaseService = purchaseService;
            this.transportService = transportService;
            this.customerService = customerService;
            this.supplierOrderService = supplierOrderService;
            this.customerOrderService = customerOrderService;
            this.fiscalAgentService= fiscalAgentService;
        }

        [HttpGet]
        public IActionResult AddServiceOrder()
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();

            if (!myCompanyUsersId.Contains(userId)) 
            { return BadRequest(); }

            var serviceOrderModel = new ServiceOrderFormModel
            {
                Suppliers = supplierService.GetSuppliers(userId),//spisak dostavchici
                MyCompanies = mycompanyService.GetCompaniesForTransportOrder(),//spisak na moite firmi
                Customers = customerService.CustomersListForService(userId),//spisyk klienti
                Currencies = currencyService.AllCurrency(),//spisak valuti
                FiscalAgents = fiscalAgentService.FiscalAgentsCollection(),
                TransportCompany = transportService.TransportCompanies()
            };

            //vrashta dali e 1st trasnport, za da mu dadem nomer ot koito da zapochvat zayavkite
            ViewBag.FirstTransport = transportService.FirstTransport();
          
            return View(serviceOrderModel); ;
        }

        //create new transport/service order
        [HttpPost]
        [Authorize]
        public IActionResult AddServiceOrder(ServiceOrderFormModel model)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            if(!ModelState.IsValid) 
            {
                new ServiceOrderFormModel
                {
                    Suppliers = supplierService.GetSuppliers(userId),//spisak dostavchici
                    MyCompanies = mycompanyService.GetCompaniesForTransportOrder(),//spisak na moite firmi
                    Customers = customerService.CustomersListForService(userId),//spisyk klienti
                    Currencies = currencyService.AllCurrency(),//spisak valuti
                    FiscalAgents = fiscalAgentService.FiscalAgentsCollection(),
                    TransportCompany = transportService.TransportCompanies()
                };
            }

            //create transport order
            var checkOrder = transportService.CreateTransportOrder
                (model.Number, model.Date, model.TransportCompanyId, model.LoadingAddress, model.Eta, model.Etd,
                model.DeliveryAddress, model.TruckNumber, model.Cost, model.MyCompanyId, model.SupplierId, model.SupplierOrderId,
                model.CustomerId, model.CustomerOrderNumberId, model.FiscalAgentId, model.CurrencyId,
                model.Vat, model.DriverName, model.DriverPhone,model.Comment, model.GrossWeight,
                model.PaymentMethod, model.PaymentTerms, model.Payer);

            if (!checkOrder) 
            {
             return BadRequest(); 
            }
            
            return RedirectToAction("Index","Home");
        }

       // get supplier spec as per supplierId
        [HttpGet]
        public IActionResult GetSupplierOrdersNumbers(string id)
        {
            if (id == null)
            {
                id = "0";
            }
            var sellerId = int.Parse(id.ToString());
            var selectedOrders = supplierOrderService.GetSupplierOrder(sellerId);

            return Json(selectedOrders, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });
        }

        //get customer orders as per chosen customer
        [HttpGet]
        public IActionResult GetCustomerOrdersNumbers(string id)
        {
            if (id == null)
            {
                id = "0";
            }
            var custmerId = int.Parse(id.ToString());
            var selectedOrders = customerOrderService.CustomerOrdersForService(custmerId);

            return Json(selectedOrders, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });
        }
    }
}
