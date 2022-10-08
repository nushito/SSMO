using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.MyCompany;
using SSMO.Services.Products;
using SSMO.Services.Reports;
using SSMO.Services.Status;
using System.Collections.Generic;

namespace SSMO.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService reportService;
        private readonly ICustomerService customerService;
        private readonly ISupplierService supplierService;
        private readonly ICurrency currency;
        private readonly IMycompanyService myCompanyService;
        private readonly IProductService productService;
        private readonly ICustomerOrderService customerOrderService;
        private readonly IMapper mapper;
        private readonly IStatusService statusService;

        public ReportsController(IReportsService service,
           ICustomerService customerService, ISupplierService supplierService,
           ICurrency currency, IMycompanyService mycompanyService, IProductService productService,
           ICustomerOrderService customerOrderService, IStatusService statusService, IMapper mapper)
        {
            this.reportService = service;
            this.customerService = customerService;
            this.supplierService = supplierService;
            this.mapper = mapper;
            this.statusService = statusService;
            this.currency = currency;
            this.myCompanyService = mycompanyService;
            this.productService = productService;
            this.customerOrderService = customerOrderService;
        }


        public IActionResult AllCustomerOrders(CustomerOrderReportAll model)
        {
            //TODO When All are selected page is empty

            var customerNames = customerService.GetCustomerNames();

            var customerOrderCollection = reportService.AllCustomerOrders(
                model.CustomerName,
                model.CurrentPage, CustomerOrderReportAll.CustomerOrdersPerPage);

            model.CustomerOrderCollection = customerOrderCollection;

            model.CustomerNames = customerNames;


            return View(model);
        }

        public IActionResult CustomerOrderDetails(int id)
        {
            var order = reportService.Details(id);

            return View(order);
        }

        [HttpGet]
        public IActionResult CustomerOrderEdit(int id)
        {

            if (!ModelState.IsValid)
            {
                new CustomerOrderForEdit
                {
                    Currencies = currency.AllCurrency(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductCustomerFormModel>(),
                    Statuses = statusService.GetAllStatus()
                };
            }


            var customerOrderForEdit = reportService.CustomerOrderDetailsForEdit(id);
            customerOrderForEdit.Suppliers = supplierService.GetSuppliers();
            customerOrderForEdit.Currencies = currency.AllCurrency();
            customerOrderForEdit.MyCompanies = myCompanyService.GetAllCompanies();
            customerOrderForEdit.Statuses = statusService.GetAllStatus();            
            customerOrderForEdit.Products = (List<ProductCustomerFormModel>)productService.DetailsPerCustomerOrder(id);
            foreach (var item in customerOrderForEdit.Products)
            {
                item.Descriptions = productService.GetDescriptions();
                item.Grades = productService.GetGrades();
                item.Sizes = productService.GetSizes();

            }

            return View(customerOrderForEdit);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CustomerOrderEdit(int id, CustomerOrderForEdit model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                new CustomerOrderForEdit
                {
                    Currencies = currency.AllCurrency(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductCustomerFormModel>(),
                    Statuses = statusService.GetAllStatus()
                };
            }

            var editOrder = reportService.EditCustomerOrder
                (id,
                model.CustomerPoNumber,
                model.Date,
                model.MyCompanyId,
                model.DeliveryTerms,
                model.LoadingPlace,
                model.DeliveryAddress,
                model.CurrencyId,
                model.StatusId,
                model.FscClaim,
                model.FscCertificate,
                model.PaidAdvance,
                model.PaidAmountStatus,
                (System.Collections.Generic.List<Models.Products.ProductCustomerFormModel>)model.Products);

            if (!editOrder)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CustomerOrdersBySupplier()
        {
            var customersList = customerService.GetCustomerNamesAndId();

            CustomerBySupplierOrdersViewModel cascade = new()
            {
                Customers = customersList,
            };

            ViewData["Selectedsupplier"] = 0;
            cascade.ProductList = null;
            return View(cascade);
        }

        [HttpPost]
        public IActionResult CustomerOrdersBySupplier(FormCollection fc, CustomerBySupplierOrdersViewModel model)
        {
            var customersList = customerService.GetCustomerNamesAndId();
            if (!ModelState.IsValid)
            {
                new CustomerBySupplierOrdersViewModel()
                {
                    Customers = customersList
                };
            };

            var supplierId = fc["Supplier"];
            ViewData["SelectedSupplier"] = supplierId;
            var ordersList = reportService.GetCustomerOrdersBySupplier(model.CustomerId, supplierId);
            var finalListOrders = new CustomerBySupplierOrdersViewModel
            {
                Customers = customersList,
                CustomerId = model.CustomerId,
                ProductList = ordersList
            };


            return View(finalListOrders);
        }

        public JsonResult GetSupplier(int id)
        {
            var selectedSuppliers = supplierService.GetSuppliersNames(id);
            return Json(selectedSuppliers);

        }
    }
}
