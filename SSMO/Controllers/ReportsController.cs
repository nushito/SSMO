using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Infrastructure;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.Documents.Invoice;
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
		private readonly IInvoiceService invoiceService;

		public ReportsController(IReportsService service,
		   ICustomerService customerService, ISupplierService supplierService,
		   ICurrency currency, IMycompanyService mycompanyService, IProductService productService,
		   ICustomerOrderService customerOrderService, IStatusService statusService, IMapper mapper, IInvoiceService invoiceService)
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
			this.invoiceService = invoiceService;
		}


		public IActionResult AllCustomerOrders(CustomerOrderReportAll model)
		{
			//TODO When All are selected page is empty
			if(model.CustomerName != null)
			{
				string userId = this.User.UserId();

				var listMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

				if (!listMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}
		   
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
			string userId = this.User.UserId();
			string userIdMyCompany = myCompanyService.GetUserIdMyCompanyById(model.MyCompanyId);

			if (userIdMyCompany != userId)
			{
				return BadRequest();
			}

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
				model.Products);

			if (!editOrder)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
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
		[Authorize]
		public IActionResult CustomerOrdersBySupplier(string supplierId, CustomerBySupplierOrdersViewModel model)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			var customersList = customerService.GetCustomerNamesAndId();
			if (!ModelState.IsValid)
			{
				new CustomerBySupplierOrdersViewModel()
				{
					Customers = customersList
				};
			};

		   // var supplierId = fc["Supplier"];
			ViewData["SelectedSupplier"] = supplierId;
			var ordersList = reportService.GetCustomerOrdersBySupplier(model.CustomerId, supplierId);
			var finalListOrders = new CustomerBySupplierOrdersViewModel
			{
				Customers = customersList,
				CustomerId = int.Parse(model.CustomerId.ToString()),
				ProductList = ordersList
			};
			return View(finalListOrders);
		}
		[HttpGet]
		public JsonResult GetSupplier(string id)
		{
			if(id == null)
			{
				id = "0";
			}
			var customerId = int.Parse(id.ToString());
			var selectedSuppliers = supplierService.GetSuppliersIdAndNames(customerId);
			return Json(selectedSuppliers);

		}

		public IActionResult InvoicePaymentReport(CustomerInvoicePaymentsReportsViewModel model)
		{
			if(model.CustomerName != null)
			{
				string userId = this.User.UserId();
				var userIdMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

				if (!userIdMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}
			if(!ModelState.IsValid) return View();
			//TODO When All are selected page is empty
		  
			var customerNames = customerService.GetCustomerNames();

			var customerPaymentCollection = reportService.CustomersInvoicesPaymentDetails(
				model.CustomerName,
				model.CurrentPage, CustomerInvoicePaymentsReportsViewModel.CustomerOrdersPerPage);

			model.CustomerPaymentCollection = customerPaymentCollection;

			model.CustomerNames = customerNames;


			return View(model);
		}
		[HttpGet]
		public IActionResult EditInvoicePayment(int id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var invoiceForEdit = invoiceService.InvoiceForEditById(id);
			return View(invoiceForEdit);
		}
		[HttpPost]
		[Authorize]
		public IActionResult EditInvoicePayment(EditInvoicePaymentModel model, int id)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var updatedInvoicePayment = reportService.EditInvoicePayment
				(id,model.DocumentNumber,model.Date, model.PaidStatus, model.PaidAvance, model.DatePaidAmount);

			if(updatedInvoicePayment == false)
            {
				return BadRequest();
            }
			return RedirectToAction("Index", "Home");
		}
		public IActionResult PurchasePaymentReport()
		{
			return View();
		}
		public IActionResult CustomerOrderPaymentReport()
		{
			return View();
		}
		public IActionResult SupplierOrderPaymentReport()
		{
			return View();
		}
	}
}
