using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSMO.Infrastructure;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.ProductsStock;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.Documents.Purchase;
using SSMO.Services.MyCompany;
using SSMO.Services.Products;
using SSMO.Services.Reports;
using SSMO.Services.Status;
using SSMO.Services.SupplierOrders;
using System.Collections.Generic;
using System.Linq;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.Purchase;
using SSMO.Models.Documents.Purchase;
using SSMO.Services.PDF;
using System.Threading.Tasks;
using System;
using System.IO;
using ClosedXML.Excel;
using SSMO.Models.Reports.FSC;
using SSMO.Models.Documents.CreditNote;
using SSMO.Services.Documents.Credit_Note;
using SSMO.Services.Documents.DebitNote;
using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Reports.CreditNote;
using SSMO.Models.Reports.DebitNote;
using SSMO.Services.Documents;
using SSMO.Services.FscTextDocuments;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Reports.ServiceOrders;
using SSMO.Services.TransportService;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Net.Mime;
using SSMO.Models.ServiceOrders;
using SSMO.Data.Models;
using SSMO.Services.FiscalAgent;

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
		private readonly IStatusService statusService;
		private readonly IInvoiceService invoiceService;
		private readonly IPurchaseService purchaseService;
		private readonly ISupplierOrderService supplierOrderService;
		private readonly IViewRenderService viewRenderService;
		private readonly IHtmlToPdfConverter htmlToPdfConverter;
		private readonly ICreditNoteService creditNoteService;
		private readonly IDebitNoteService debitNoteService;
		private readonly IDocumentService documentService;
		private readonly IFscTextService fscTextService;
		private readonly IBankService bankService;
		private readonly ITransportService transportService;

		public ReportsController(IReportsService service,
		   ICustomerService customerService, ISupplierService supplierService,
		   ICurrency currency, IMycompanyService mycompanyService, IProductService productService,
		   ICustomerOrderService customerOrderService, IStatusService statusService, IInvoiceService invoiceService,
		   IPurchaseService purchaseService, ISupplierOrderService supplierOrderService,
		   IViewRenderService viewRenderService, IHtmlToPdfConverter htmlToPdfConverter,
		   ICreditNoteService creditNoteService, IDebitNoteService debitNoteService,
		   IDocumentService documentService, IFscTextService fscTextService, IBankService bankService,
		   ITransportService transportService) //IWebHostEnvironment environment)
		{
			this.reportService = service;
			this.customerService = customerService;
			this.supplierService = supplierService;
			this.statusService = statusService;
			this.currency = currency;
			this.myCompanyService = mycompanyService;
			this.productService = productService;
			this.customerOrderService = customerOrderService;
			this.invoiceService = invoiceService;
			this.purchaseService = purchaseService;
			this.supplierOrderService = supplierOrderService;
			this.viewRenderService = viewRenderService;
			this.htmlToPdfConverter = htmlToPdfConverter;
			this.creditNoteService = creditNoteService;
			this.debitNoteService = debitNoteService;
			this.documentService = documentService;
			this.fscTextService = fscTextService;
			this.bankService = bankService;
			this.transportService = transportService;
		}
		public IActionResult AllCustomerOrders(CustomerOrderReportAll model)
		{
            string userId = this.User.UserId();
            //TODO When All are selected page is empty
            if (model.CustomerName != null)
			{			

				var listMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

				if (!listMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}

			var customerNames = customerService.GetCustomerNames(userId);

			var customerOrderCollection = reportService.AllCustomerOrders(
				model.CustomerName, model.StartDate, model.EndDate,
				model.CurrentPage, CustomerOrderReportAll.CustomerOrdersPerPage);

			model.CustomerOrderCollection = customerOrderCollection.CustomerOrders;
			model.TotalCustomerOrders = customerOrderCollection.TotalCustomerOrders;
			model.CustomerNames = customerNames;
			return View(model);
		}
		public IActionResult CustomerOrderDetailsPrint(int id)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			var order = reportService.CustomerOrderDetails(id);
            var printModel = customerOrderService.GetCustomerOrderPrint(id);
            ClientService.AddCustomerOrderPrint(printModel);

            return View(printModel);
		}

		[HttpGet]
		public IActionResult CustomerOrderEdit(int id)
		{
            string userId = this.User.UserId();
            var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid)
			{
				new CustomerOrderForEdit
				{
					Currencies = currency.AllCurrency(),
					MyCompanies = myCompanyService.GetAllCompanies(),
					Suppliers = supplierService.GetSuppliers(userId),
					Products = new List<ProductCustomerFormModel>(),
					Statuses = statusService.GetAllStatus(),
					SupplierOrdersBySupplier = supplierOrderService.SuppliersAndOrders(),
					BankDetails = customerOrderService.GetBanks(),
					FiscalAgents = documentService.GetFiscalAgents(),
					FscTexts = fscTextService.GetAllFscTexts()
				};
			}			

			var customerOrderForEdit = reportService.CustomerOrderDetailsForEdit(id);

			customerOrderForEdit.Suppliers = supplierService.GetSuppliers(userId);
			customerOrderForEdit.Currencies = currency.AllCurrency();
			customerOrderForEdit.MyCompanies = myCompanyService.GetAllCompanies();
			customerOrderForEdit.Statuses = statusService.GetAllStatus();
			customerOrderForEdit.Products = (List<ProductCustomerFormModel>)productService.DetailsPerCustomerOrder(id);
			customerOrderForEdit.SupplierOrdersBySupplier = supplierOrderService.SuppliersAndOrders();
			customerOrderForEdit.BankDetails = bankService.GetMyBanks(id);
			customerOrderForEdit.FiscalAgents = documentService.GetFiscalAgents();
			customerOrderForEdit.FscTexts = fscTextService.GetAllFscTexts();

			foreach (var item in customerOrderForEdit.Products)
			{
				item.Descriptions = productService.GetDescriptions();
				item.Grades = productService.GetGrades();
				item.Sizes = productService.GetSizes();
				item.Units = productService.GetUnits();
			}
			return View(customerOrderForEdit);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CustomerOrderEdit(int id, 
			CustomerOrderForEdit model, string command)
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
					Suppliers = supplierService.GetSuppliers(userId),
					Products = new List<ProductCustomerFormModel>(),
					Statuses = statusService.GetAllStatus(),
					SupplierOrdersBySupplier = supplierOrderService.SuppliersAndOrders(),
					BankDetails = customerOrderService.GetBanks(),
                    FiscalAgents = documentService.GetFiscalAgents(),
                    FscTexts = fscTextService.GetAllFscTexts()
                };
			}

			var editOrder = await reportService.EditCustomerOrder
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
				model.Products,
				model.ChosenBanks,
				model.FiscalAgentId, 
				model.FscText, model.PaymentTerms,
				model.Eta, model.Etd);

			if (!editOrder)
			{
				return BadRequest();
			}

			if (command == "Add More Products")
			{
				return RedirectToAction("AddOrderProducts", "CustomerOrders", new { customerorderId = id, selectedSupplierOrders = model.SelectedSupplierOrders });
			}

			return RedirectToAction("PrintCustomerOrder", "CustomerOrders", new { customerorderId = id });
		}
				

		[HttpGet]
		public IActionResult GetSupplier(string id)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (id == null)
			{
				id = "0";
			}
			var customerId = int.Parse(id.ToString());
			var selectedSuppliers = supplierService.GetSuppliersIdAndNames(customerId);
			return Json(selectedSuppliers);
		}
		public IActionResult AllSupplierOrders(SupplierOrdersReportAll model)
		{
            string userId = this.User.UserId();
            if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (model.SupplierName != null)
			{	
				var listMyCompany = myCompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

				if (!listMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}

			model.SupplierNames = supplierService.GetSupplierNames(userId);

			var supplierOrdersCollection = reportService.AllSupplierOrders
				(model.SupplierName,model.StartDate,model.EndDate,
				model.CurrentPage, SupplierOrdersReportAll.SupplierOrdersPerPage);

			model.SupplierOrderCollection = supplierOrdersCollection.SupplierOrders;
			model.TotalSupplierOrders = supplierOrdersCollection.TotalSupplierOrders;

			return View(model);
		}
		public IActionResult SupplierOrderDetails(int id)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			var supplierOrderDetails = reportService.SupplierOrderDetail(id);
			return View(supplierOrderDetails);
		}

		[HttpGet]
		public IActionResult SupplierOrderEdit(int id)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid)
			{
				new SupplierOrderForEditModel
				{
					Currencies = currency.AllCurrency(),
					MyCompanies = myCompanyService.GetAllCompanies(),
					Products = new List<ProductsForEditSupplierOrder>(),
					Statuses = statusService.GetAllStatus(),
					NewProducts = new List<NewProductsForSupplierOrderModel>()
				};
			}
			var suppplierOrderForEdit = reportService.SupplierOrderForEditDetails(id);
			suppplierOrderForEdit.Currencies = currency.AllCurrency();
			suppplierOrderForEdit.MyCompanies = myCompanyService.GetAllCompanies();
			suppplierOrderForEdit.Statuses = statusService.GetAllStatus();
			suppplierOrderForEdit.Products = (List<ProductsForEditSupplierOrder>)productService.ProductsDetailsPerSupplierOrder(id);
			suppplierOrderForEdit.CustomerOrderNumber = customerOrderService.CustomerOrderNumber(id);
			suppplierOrderForEdit.NewProducts = new List<NewProductsForSupplierOrderModel>();

			foreach (var item in suppplierOrderForEdit.Products)
			{
				item.Descriptions = productService.DescriptionCollection();
				item.Grades = productService.GradeCollection();
				item.Sizes = productService.SizeCollection();
				item.Units = productService.GetUnits();
			}

			return View(suppplierOrderForEdit);
		}

		[HttpPost]
		public async Task<IActionResult> SupplierOrderEdit(SupplierOrderForEditModel model, int id, IFormCollection collection)
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
				new SupplierOrderForEditModel
				{
					Currencies = currency.AllCurrency(),
					MyCompanies = myCompanyService.GetAllCompanies(),
					Products = new List<ProductsForEditSupplierOrder>(),
					Statuses = statusService.GetAllStatus(),
					NewProducts = new List<NewProductsForSupplierOrderModel>()
				};
			}

			model.NewProducts = new List<NewProductsForSupplierOrderModel>();

			int loopsNum = 0;

			foreach (var key in collection.Keys)
			{
				if (key.StartsWith("Quantity"))
				{
					loopsNum++;
				}
			}

			if (loopsNum > 0)
			{
				for (int i = 1; i <= loopsNum; i++)
				{
					var description = collection["DescriptionId[" + i + "]"];
					var grade = collection["GradeId[" + i + "]"];
					var size = collection["SizeId[" + i + "]"];
					var unit = collection["Unit[" + i + "]"];
					var price = collection["Price[" + i + "]"].ToString();
					var fscClaim = collection["FscClaim[" + i + "]"];
					var fscCertificate = collection["FscCertificate[" + i + "]"];
					var pallets = collection["Pallets[" + i + "]"];
					var sheetsPerPallet = collection["SheetsPerPallet[" + i + "]"];
					var quantity = collection["Quantity[" + i + "]"].ToString();
					var product = new NewProductsForSupplierOrderModel
					{
						DescriptionId = int.Parse(description.ToString()),
						GradeId = int.Parse(grade.ToString()),
						SizeId = int.Parse(size.ToString()),
						Unit = unit,
						PurchasePrice = decimal.Parse(price.ToString()),
						FscClaim = fscClaim,
						SupplierFscCertNumber = fscCertificate,
						Pallets = int.Parse(pallets.ToString()),
						SheetsPerPallet = int.Parse(sheetsPerPallet.ToString()),
						Quantity = decimal.Parse(quantity.ToString()),
						SupplierOrderId = id
					};
					model.NewProducts.Add(product);
				}
			}

			var orderForEdit = await reportService.EditSupplierOrder
				(id, model.Number, model.Date, model.MyCompanyId,
				model.DeliveryTerms, model.LoadingAddress, model.DeliveryAddress, model.GrossWeight, model.NetWeight,
				model.CurrencyId, model.StatusId, model.CustomerOrderNumber, model.FSCClaim, model.FSCSertificate, model.PaidAvance,
				model.PaidStatus, model.VAT, model.Products, model.NewProducts);

			if (orderForEdit == false)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Home");
		}
		public IActionResult InvoicePaymentReport(CustomerInvoicePaymentsReportsViewModel model)
		{
            string userId = this.User.UserId();
            if (model.CustomerName != null)
			{				
				var userIdMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

				if (!userIdMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}
			if (!ModelState.IsValid) return View();
			//TODO When All are selected page is empty

			var customerNames = customerService.GetCustomerNames(userId);

			var customerPaymentCollection = reportService.CustomersInvoicesPaymentDetails(
				model.CustomerName,
				model.CurrentPage, CustomerInvoicePaymentsReportsViewModel.CustomerInvoicesPerPage);

			model.CustomerPaymentCollection = customerPaymentCollection.CustomerInvoices;
			model.CustomerNames = customerNames;
			model.TotalCustomerInvoices = customerPaymentCollection.TotalInvoices;
			ClientService.AddCustomerInvoicePayments(model);
			return View(model);
		}
		[HttpGet]
		public IActionResult EditInvoicePayment(int documentNumber)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var invoiceForEdit = invoiceService.InvoiceForEditByNumber(documentNumber);
			return View(invoiceForEdit);
		}
		[HttpPost]
		[Authorize]
		public IActionResult EditInvoicePayment(EditInvoicePaymentModel model, int documentNumber)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var updatedInvoicePayment = invoiceService.EditInvoicePayment
				(documentNumber, model.PaidStatus, model.PaidAvance, model.DatePaidAmount, model.CustomerOrderId);

			if (updatedInvoicePayment == false)
			{
				return BadRequest();
			}
			return RedirectToAction("Index", "Home");
		}

		
		public IActionResult CustomerOrdersPaymentReport(CustomerOrderPaymentReportViewModel model)
		{
            string userId = this.User.UserId();
            if (model.CustomerName != null)
			{				
				var userIdMyCompany = myCompanyService.MyCompaniesNamePerCustomer
					(model.CustomerName);

				if (!userIdMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}
			if (!ModelState.IsValid) return View();
			//TODO When All are selected page is empty
			var customerNames = customerService.GetCustomerNames(userId);

			var customerOrdersPaymentCollection = reportService.CustomerOrdersPaymentDetails(
				model.CustomerName,
				model.CurrentPage, CustomerOrderPaymentReportViewModel.CustomerOrderPerPage);

			model.CustomerOrdersPaymentCollection = customerOrdersPaymentCollection.CustomerPaymentCollection;
			model.CustomerNames = customerNames;
			model.TotalCustomerOrders = customerOrdersPaymentCollection.TotalCustomerOrders;
			return View(model);
		}
		[HttpGet]
		[Authorize]
		public IActionResult EditCustomerOrderPayment(int orderConfirmationNumber)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var customerOrderForEdit = customerOrderService.GetCustomerOrderPaymentForEdit(orderConfirmationNumber);

			return View(customerOrderForEdit);
		}

		[HttpPost]
		[Authorize]
		public IActionResult EditCustomerOrderPayment(EditCustomerOrderPaymentModel model, int orderConfirmationNumber)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var updatedCustomerOrderPayment = customerOrderService.EditCustomerOrdersPayment
				(orderConfirmationNumber, model.PaidStatus, model.Payment, model.Date);

			if (updatedCustomerOrderPayment == false)
			{
				return BadRequest();
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult SupplierOrdersPaymentReport(SupplierOrdersPaymentReportViewModel model)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid)
			{
				return View();
			}

			var supplierNames = supplierService.GetSupplierNames(userId);
			model.SupplierNames = supplierNames;

			var supplierOrdersPaymentCollection = supplierOrderService.GetSupplierOrders
				(model.SupplierName,model.StartDate,model.EndDate, model.CurrentPage,
				SupplierInvoicePaymentReportViewModel.SupplierInvoicePerPage);

			model.SupplierOrderPaymentCollection = supplierOrdersPaymentCollection.SupplierOrderPaymentCollection;
			model.TotalSupplierOrders = supplierOrdersPaymentCollection.TotalSupplierOrders;
            ClientService.AddPurchasePayments(model);
            return View(model);
		}
		[HttpGet]
		[Authorize]
		public IActionResult DetailsSupplierAndPurchasePayment(int id)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var supplierOrder = supplierOrderService.GetPaymentsPerOrderForEdit(id);
			ViewBag.Count = supplierOrder.PurchasePaymentsCollection.Count;           
            return View(supplierOrder);
		}
		[HttpPost]
		[Authorize]
		public IActionResult DetailsSupplierAndPurchasePayment(int id, EditSupplierOrderPaymentModel model)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var isSupplierOrderPaymentEdit = supplierOrderService.EditSupplierOrderPurchasePayment
                (id, model.NewPaidAmount??0m, model.NewDateOfPayment ?? null,model.ConvertToThatCurrency, model.ActionCalc,
				model.CurrencyExchangeRate ?? 0m, model.PurchasePaymentsCollection);

			if (!isSupplierOrderPaymentEdit) return BadRequest();
			return RedirectToAction("SupplierOrdersPaymentReport");
		}
		public IActionResult ProductsOnStock(ProductAvailabilityViewModel model)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (myCompanyService.GetAllCompanies().Count == 0) return BadRequest();

			var descriptions = productService.DescriptionIdAndNameList();
			var grades = productService.GradeIdAndNameList();
			var sizes = productService.SizeIdAndNameList();

			model.Descriptions = descriptions;
			model.Grades = grades;
			model.Sizes = sizes;

			var productCollection = productService.ProductsOnStock
				(model.DescriptionId, model.GradeId, model.SizeId, model.CurrentPage,
				ProductAvailabilityViewModel.ProductsPerPage);

			model.ProductsDetails = productCollection.Products;
			model.TotalProducts = productCollection.TotalProducts;
			ClientService.AddProductsOnStock(productCollection.Products);

			return View(model);
		}
		public IActionResult FscReport(ProductsFscCollectionViewModel model)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var fscClaims = productService.FscClaimList();		
			
			model.FscClaims = fscClaims;
			model.MyCompanies = myCompanyService.GetCompaniesNameAndId();
			model.MyCompany = myCompanyService.GetCompanyName(model.MyCompanyId);

			if(model.PurchaseOrSell == "Purchase")
			{
                var purchaseProductsCollection = productService.PurchaseProductFscCollection
                (model.MyCompanyId, model.StartDate, model.EndDate, model.FSCClaim);

                model.PurchaseProducts = purchaseProductsCollection;
                model.TotalProducts = purchaseProductsCollection.Count();
                model.SoldProducts = new List<SoldProductsFscCollectionViewModel>();
            }
			else if(model.PurchaseOrSell == "Sell")
			{
                var soldProductsCollection = productService.SoldProductFscCollection
                                (model.MyCompanyId, model.StartDate, model.EndDate, model.FSCClaim);

                model.SoldProducts = soldProductsCollection;
                model.TotalProducts = soldProductsCollection.Count();
				
				model.PurchaseProducts = new List<PurchaseProductFscCollectionViewModel>();
            }
			
			ClientService.AddFscReport(model);
            return View(model);
		}
		public IActionResult AllInvoices([FromQuery] InvoicesViewModel model)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			var myCompanyNames = myCompanyService.GetCompaniesNames();
			model.MyCompanyNames = myCompanyNames;

			var invoiceCollection = reportService.InvoiceCollection
				(model.MyCompanyName, model.StartDate, model.EndDate,
                 model.CurrentPage,
				 InvoicesViewModel.InvoicesPerPage);

			model.InvoiceCollection = invoiceCollection.InvoiceCollection;
			model.TotalInvoices = invoiceCollection.TotalInvoices;

			return View(model);
		}

		[HttpGet]
		public IActionResult EditInvoice(int id, string documentType)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditInvoiceViewModel
				{
					DocumentType = documentType,
					Products = new List<EditProductForCompanyInvoicesViewModel>(),
					Customers = invoiceService.CustomersForeEditInvoice(),
                    CompanyBankDetails = customerOrderService.GetBanks(),
                    FiscalAgents = documentService.GetFiscalAgents(),
					FscTexts = fscTextService.GetAllFscTexts()
                };
			}
			var invoiceToEdit = invoiceService.ViewEditInvoice(id);
			return View(invoiceToEdit);
		}

		[HttpPost]
		public async Task<IActionResult> EditInvoice(int id, EditInvoiceViewModel model,
			string documentType, string command)
		{
			//TODO Can i make this global
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditInvoiceViewModel
				{
					DocumentType = documentType,
					Products = new List<EditProductForCompanyInvoicesViewModel>(),
					Customers = invoiceService.CustomersForeEditInvoice(),
					CompanyBankDetails = customerOrderService.GetBanks(),
                    FiscalAgents = documentService.GetFiscalAgents(),
					FscTexts = fscTextService.GetAllFscTexts()
                };
			}

			var checkEditableInvoice = await invoiceService.EditInvoice
				(id, model.CurrencyExchangeRate, model.Date, model.GrossWeight, model.NetWeight,
				model.DeliveryCost, model.OrderConfirmationNumber, model.TruckNumber,
			   model.Products, model.Incoterms, model.Comment, model.ChosenBanks, 
			   model.FiscalAgentId, model.FscTextEng, model.PaymentTerms,model.DeliveryAddress,
			   model.LoadingAddress);

			if (!checkEditableInvoice) return BadRequest();

			if (command == "Add New Products")
			{
				return RedirectToAction("NewProductsForEditInvoice", new { Id = id, selectedOrders = model.SelectedCustomerOrders });
			}

			return RedirectToAction("InvoiceDetails", new { Id = id });
		}

		[HttpGet]
		public IActionResult NewProductsForEditInvoice(int id, List<int> selectedOrders)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			var newProducts = productService.ProductsForInvoice(selectedOrders);

			return View(newProducts);
		}

		[HttpPost]
		public IActionResult NewProductsForEditInvoice(int id, List<int> selectedOrders,
			List<ProductsForInvoiceViewModel> products)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			var addNewProductsToInvoiceForEdit = productService.AddNewProductsToEditedInvoice(id, products);

			if (addNewProductsToInvoiceForEdit == false)
			{
				return BadRequest();
			}

			return RedirectToAction("InvoiceDetails", new { Id = id });
		}
		public IActionResult EditCreditNote(int id, string documentType)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditCreditNoteViewModel
				{
					DocumentType = documentType,
					Products = new List<EditProductForCreditNoteViewModel>(),
					InvoiceNumbers = new List<InvoiceNumbersForEditedCreditNoteViewModel>()
				};
			}
			var invoiceToEdit = creditNoteService.ViewCreditNoteForEdit(id);
			invoiceToEdit.InvoiceNumbers = creditNoteService.InvoiceNumbers();

			return View(invoiceToEdit);
		}

		[HttpPost]
		public IActionResult EditCreditNote(int id, string documentType, string command, EditCreditNoteViewModel model)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditCreditNoteViewModel
				{
					DocumentType = documentType,
					Products = new List<EditProductForCreditNoteViewModel>(),
					InvoiceNumbers = new List<InvoiceNumbersForEditedCreditNoteViewModel>()
				};
			}

			var editcreditnote = creditNoteService.EditCreditNote
				(id, model.Date, model.Incoterms, model.TruckNumber, model.NetWeight,
				model.GrossWeight, model.DeliveryCost, model.CurrencyExchangeRate, model.Comment, 
				model.Products, model.PaymentTerms);

			if (editcreditnote == false)
			{
				return BadRequest();
			}

			if (command == "Add New/More Products")
			{
				return RedirectToAction("AddMoreProductsToCreditNote", new { id = id, invoiceNumberId = model.InvoiceNumberId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult AddMoreProductsToCreditNote
			(int id, int invoiceNumberId)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();

			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditedCreditNoteNewProductViewModel()
				{
					CreditNoteId = id,
					InvoiceNumberId = invoiceNumberId,
					NewProducts = new List<NewProductsForCreditNoteViewModel>(),
					Products = new List<ProductForCreditNoteViewModelPerInvoice>()
				};
			}

			var editedModel = new EditedCreditNoteNewProductViewModel();

			var products = productService.ProductsForCreditNotePerInvoice(invoiceNumberId);

			editedModel.Products = products;
			editedModel.NewProducts = new List<NewProductsForCreditNoteViewModel>();
			editedModel.CreditNoteId = id;
			editedModel.InvoiceNumberId = invoiceNumberId;

			return View(editedModel);
		}

		[HttpPost]
		public IActionResult AddMoreProductsToCreditNote
			(int id, EditedCreditNoteNewProductViewModel model, int invoiceNumberId, IFormCollection collection)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();

			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditedCreditNoteNewProductViewModel()
				{
					CreditNoteId = id,
					InvoiceNumberId = invoiceNumberId,
					NewProducts = new List<NewProductsForCreditNoteViewModel>(),
					Products = new List<ProductForCreditNoteViewModelPerInvoice>()
				};
			}

			model.NewProducts = new List<NewProductsForCreditNoteViewModel>();
			int loopsNum = 0;

			foreach (var key in collection.Keys)
			{
				if (key.StartsWith("CreditNoteQuantity"))
				{
					loopsNum++;
				}
			}

			if (loopsNum > 0)
			{
				for (int i = 1; i <= loopsNum; i++)
				{
					var description = collection["DescriptionId[" + i + "]"];
					var grade = collection["GradeId[" + i + "]"];
					var size = collection["SizeId[" + i + "]"];
					var unit = collection["Unit[" + i + "]"];
					var price = collection["CreditNotePrice[" + i + "]"].ToString();
					var fscClaim = collection["FscClaim[" + i + "]"];
					var fscCertificate = collection["FscCertificate[" + i + "]"];
					var pallets = collection["CreditNotePallets[" + i + "]"];
					var sheetsPerPallet = collection["CreditNoteSheetsPerPallet[" + i + "]"];
					var quantity = collection["CreditNoteQuantity[" + i + "]"].ToString();
					var product = new NewProductsForCreditNoteViewModel
					{
						DescriptionId = int.Parse(description.ToString()),
						GradeId = int.Parse(grade.ToString()),
						SizeId = int.Parse(size.ToString()),
						Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), unit, true),
						CreditNotePrice = decimal.Parse(price.ToString()),
						FscClaim = fscClaim,
						FscCertificate = fscCertificate,
						CreditNotePallets = int.Parse(pallets.ToString()),
						CreditNoteSheetsPerPallet = int.Parse(sheetsPerPallet.ToString()),
						CreditNoteQuantity = decimal.Parse(quantity.ToString()),
						CreditNoteId = id
					};

					model.NewProducts.Add(product);
				}
			}

			var addProductsToEditCredtNote = creditNoteService.AddNewProductsToCreditNoteWhenEdit
				(id, invoiceNumberId, model.Products, model.NewProducts);

			if (addProductsToEditCredtNote == false) return null;

			return RedirectToAction("Index", "Home");
			; }
		public IActionResult EditDebitNote(int id, string documentType)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditDebitNoteViewModel
				{
					DocumentType = documentType,
					Products = new List<EditProductForDebitNoteViewModel>(),
					DebitNoteInvoicenumbers = new List<InvoiceNumbersForEditedDebitNoteViewModel>()
				};
			}
			var invoiceToEdit = debitNoteService.ViewDebitNoteForEdit(id);
			invoiceToEdit.DebitNoteInvoicenumbers = debitNoteService.GetInvoiceNumbers();
			return View(invoiceToEdit);
		}

		[HttpPost]
		public IActionResult EditDebitNote
			(EditDebitNoteViewModel model, string documentType, string command, int id)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditDebitNoteViewModel
				{
					DocumentType = documentType,
					Products = new List<EditProductForDebitNoteViewModel>(),
					DebitNoteInvoicenumbers = new List<InvoiceNumbersForEditedDebitNoteViewModel>()
				};
			}

			var editDebitNote = debitNoteService.EditDebitNote
				(id, model.Date, model.Incoterms, model.Comment, model.Products, model.PaymentTerms);

			if (editDebitNote == false)
			{
				return BadRequest();
			}

			if (command == "Add New/More Products")
			{
				return RedirectToAction("AddMoreProductsToDebitNote", new { id = id, invoiceNumberId = model.DebitToInvoiceNumberId });
			}

			return View();
		}

		public IActionResult AddMoreProductsToDebitNote(int id, int invoiceNumberId)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditedDebitNoteNewProductsViewModel
				{
					DebitNoteId = id,
					InvoiceId = invoiceNumberId,
					NewProducts = new List<NewProductsForEditedDebitNoteFormModel>(),
					Products = new List<NewProductsFromOrderEditedDebitNoteViewModel>(),
					PurchaseProducts = purchaseService.PurchaseProducts()
				};
			}

			var productsForDebitNote = new EditedDebitNoteNewProductsViewModel
			{
				DebitNoteId = id,
				InvoiceId = invoiceNumberId,
				NewProducts = new List<NewProductsForEditedDebitNoteFormModel>(),
				Products = new List<NewProductsFromOrderEditedDebitNoteViewModel>()
			};

			var products = productService.ProductsForDebitNotePerInvoice(invoiceNumberId);

			productsForDebitNote.Products = products;
			productsForDebitNote.PurchaseProducts = purchaseService.PurchaseProducts();

			return View(productsForDebitNote);
		}

		[HttpPost]
		public IActionResult AddMoreProductsToDebitNote(EditedDebitNoteNewProductsViewModel model, int id, int invoiceNumberId,
			IFormCollection collection)
		{
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				new EditedDebitNoteNewProductsViewModel
				{
					DebitNoteId = id,
					InvoiceId = invoiceNumberId,
					NewProducts = new List<NewProductsForEditedDebitNoteFormModel>(),
					Products = new List<NewProductsFromOrderEditedDebitNoteViewModel>(),
					PurchaseProducts = purchaseService.PurchaseProducts()
				};
			}

			model.NewProducts = new List<NewProductsForEditedDebitNoteFormModel>();

			int loopsNum = 0;

			foreach (var key in collection.Keys)
			{
				if (key.StartsWith("DebitNoteQuantity"))
				{
					loopsNum++;
				}
			}

			if (loopsNum > 0)
			{
				for (int i = 1; i <= loopsNum; i++)
				{
					var description = collection["DescriptionId[" + i + "]"];
					var grade = collection["GradeId[" + i + "]"];
					var size = collection["SizeId[" + i + "]"];
					var unit = collection["Unit[" + i + "]"];
					var price = collection["DebitNotePrice[" + i + "]"].ToString();
					var fscClaim = collection["FscClaim[" + i + "]"];
					var fscCertificate = collection["FscCertificate[" + i + "]"];
					var pallets = collection["DebitNotePallets[" + i + "]"];
					var sheetsPerPallet = collection["DebitNoteSheetsPerPallet[" + i + "]"];
					var quantity = collection["DebitNoteQuantity[" + i + "]"].ToString();
					var product = new NewProductsForEditedDebitNoteFormModel
					{
						DescriptionId = int.Parse(description.ToString()),
						GradeId = int.Parse(grade.ToString()),
						SizeId = int.Parse(size.ToString()),
						Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), unit, true),
						DebitNotePrice = decimal.Parse(price.ToString()),
						FscClaim = fscClaim,
						FscCertificate = fscCertificate,
						DebitNotePallets = int.Parse(pallets.ToString()),
						DebitNoteSheetsPerPallet = int.Parse(sheetsPerPallet.ToString()),
						DebitNoteQuantity = decimal.Parse(quantity.ToString()),
						DebitNoteId = id
					};

					model.NewProducts.Add(product);
				}
			}

			var addProductsToEditDebitNote = debitNoteService.AddNewProductsToDebitNoteWhenEdit
				(id, invoiceNumberId, model.Products, model.NewProducts, model.PurchaseProducts);

			if (addProductsToEditDebitNote == false) return null;

			return RedirectToAction("Index", "Home");
		}

		public IActionResult InvoiceDetails(int id)
		{
			string userId = this.User.UserId();
			var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
			if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

			var invoiceDetails = reportService.InvoiceDetails(id);
			ClientService.AddClient(invoiceDetails);

			// return RedirectToAction("ExportInvoiceToPdf", new { Model = invoiceDetails })
			return View(invoiceDetails);
		}

		public IActionResult AllPurchases(PurchaseInvoisecBySupplierModel model)
		{
			var userId = this.User.UserId();
			var myuserId = myCompanyService.GetCompaniesUserId();
			if (!myuserId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid) { return BadRequest(); }

			model.Suppliers = supplierService.GetSupplierNames(userId);

			var purchaseCollection = reportService.PurchaseInvoices
				(model.Supplier, model.StartDate.Date, model.EndDate.Date, model.CurrentPage,
				PurchaseInvoisecBySupplierModel.PurchaseInvoicesPerPage);

			model.InvoiceCollection = purchaseCollection.PurchaseInvoiceCollection;

			model.TotalPurchaseInvoices = purchaseCollection.TotalPurchaseInvoices;

			return View(model);
		}
		public IActionResult PurchaseInvoiceDetails(int id)
		{
			var userId = this.User.UserId();
			var myuserId = myCompanyService.GetCompaniesUserId();
			if (!myuserId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid) { return BadRequest(); }

			var purchaseForEdit = purchaseService.PurchaseDetails(id);

			return View(purchaseForEdit);
		}

		[HttpGet]
		public IActionResult EditPurchase(int id)
		{
			var userId = this.User.UserId();
			var myuserId = myCompanyService.GetCompaniesUserId();
			if (!myuserId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid) { return BadRequest(); }
			var purchaseForEdit = purchaseService.PurchaseDetailsForEdit(id);

			return View(purchaseForEdit);

		}

		[HttpPost]
		public async Task<IActionResult> EditPurchase(EditPurchaseViewModel model, int id)
		{
			var userId = this.User.UserId();
			var myUserId = myCompanyService.GetCompaniesUserId();

			if (!myUserId.Contains(userId)) { return BadRequest(); }

			if (!ModelState.IsValid) { return BadRequest(); }

			var purchaseForEdit = await purchaseService.EditPurchaseInvoice(id, model.PurchaseNumber, model.Date,
				model.SupplierOrderId, model.Vat, model.NetWeight, model.GrossWeight, model.TruckNumber, model.Swb,
				model.PurchaseTransportCost, model.BankExpenses, model.Duty, model.CustomsExpenses,
				model.Factoring, model.FiscalAgentExpenses, model.ProcentComission, model.OtherExpenses,
				model.PurchaseProducts, model.DeliveryAddress, model.ShippingLine,model.Eta);

			if (purchaseForEdit == false) { return BadRequest(); }

			return RedirectToAction("AllPurchases");
		}

		public IActionResult AllServiceOrders(AllServiceOrdersCollectionViewModel model) 
		{
            string userId = this.User.UserId();
            var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
            if (!mycompaniesUserId.Contains(userId))
            {
                return BadRequest();
            }

			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			model.MyCompanies = myCompanyService.GetCompaniesForTransportOrder();
			
			var ordersCollection = reportService.ServiceOrdersCollection
				(model.MyCompanyId,model.StartDate,model.EndDate,model.CurrentPage,
				AllServiceOrdersCollectionViewModel.serviceOrdersPerPage);
			
			
			model.ServiceOrders = ordersCollection.ServiceOrders;
			model.TotalServiceOrders = ordersCollection.TotalServiceOrders;

            return View(model);		
		}

		public IActionResult ServiceOrderDetail(int id)
		{
            var userId = this.User.UserId();
            var myuserId = myCompanyService.GetCompaniesUserId();
            if (!myuserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }

			var serviceOrderForPrint = transportService.ServiceOrderPrintDetails(id);

            ClientService.AddServiceOrder(serviceOrderForPrint);

            return View(serviceOrderForPrint);	
		}

		[HttpGet]
		public IActionResult EditServiceOrder(int id)
		{
            var userId = this.User.UserId();
            var myuserId = myCompanyService.GetCompaniesUserId();
            if (!myuserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }
			//vzimame zaqwkata za redaktirane
			var getOrder = transportService.GetForEditTransportOrder(id);
			getOrder.Customers = customerService.CustomersListForService(userId);//spisyk klienti
			getOrder.Suppliers = supplierService.GetSuppliers(userId);
            return View(getOrder);
		}

		[HttpPost]
		public IActionResult EditServiceOrder(int id, ServiceOrderForEditViewModel model)
        {
            var userId = this.User.UserId();
            var myuserId = myCompanyService.GetCompaniesUserId();
            if (!myuserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }

			var checkEdit = transportService.EditTransportOrder(id, model.Date, model.TransportCompanyId,
				model.LoadingAddress, model.Eta, model.Etd, model.DeliveryAddress, model.TruckNumber,
				model.Cost, model.MyCompanyId, model.SupplierOrderId, model.CustomerOrderNumberId, 
				model.FiscalAgentId, model.CurrencyId, model.Vat, model.DriverName,
				model.DriverPhone, model.Comment, model.GrossWeight, model.PaymentMethod, 
				model.PaymentTerms, model.Payer); 

			if(checkEdit == false) { return BadRequest(); }	

            return RedirectToAction("Index", "Home");
		}
		
		[HttpGet]
		//public async Task<IActionResult> ExportInvoiceToPdf()
		//{
		//	InvoiceDetailsViewModel invoice = ClientService.GetClient();
		//	await Export();
		//	return View(invoice);
		//}
		//public async Task<string> Export()
		//{
		//	System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
		//	var pdfModel = ClientService.GetClient();
		//	var filename = "doc" + pdfModel.DocumentNumber + "_" + DateTime.Now.ToString("ddMMyyyy");
		//	var stringForPrint = await viewRenderService.RenderToStringAsync("~/Views/Reports/ExportInvoiceToPdf.cshtml", pdfModel);
		//	iTextSharp.text.Document document = new iTextSharp.text.Document();
		//	XMLWorkerFontProvider fontProvider = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
		//	var path = Directory.GetCurrentDirectory() + filename + ".pdf";
		//	PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
		//	//foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()+"/"+"wwwroot/fonts"))
		//	//{
		//	//	FontFactory.FontImp.Register(file);
		//	//}

		//	document.Open();
		//	document.Add(new Chunk(""));

		//	using (var strReader = new StringReader(stringForPrint))
		//	{
		//		HtmlPipelineContext htmlcontext = new HtmlPipelineContext(null);
		//		htmlcontext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
		//		ICSSResolver cSSResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
		//		//cSSResolver.AddCssFile(Directory.GetCurrentDirectory() + "/wwwroot/css/site.css", true);
		//		IPipeline pipeline = new CssResolverPipeline(cSSResolver, new HtmlPipeline(htmlcontext, new PdfWriterPipeline(document, writer)));
		//		var worker = new XMLWorker(pipeline, true);
		//		var xmlParse = new XMLParser(true, worker);
		//		xmlParse.Parse(strReader);
		//		xmlParse.Flush();
		//	}
		//	document.Close();
		//	return path;
		//}

		//public async Task<ActionResult> ExportToPdf2()
		//      {
		//          InvoiceDetailsViewModel invoice = ClientService.GetClient();
		//          using (StringWriter sw = new StringWriter())
		//          {
		//              using (HtmlTextWriter hw = new HtmlTextWriter(sw))
		//              {
		//                  //To Export all pages
		//                  GridView gridview = new GridView();
		//                  gridview.DataSource = _empList;
		//                  gridview.DataBind();

		//                  gridview.RenderControl(hw);
		//                  StringReader sr = new StringReader(sw.ToString());
		//                  Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
		//                  HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
		//                  PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
		//                  pdfDoc.Open();
		//                  htmlparser.Parse(sr);
		//                  pdfDoc.Close();
		//                  Response.ContentType = "application/pdf";
		//                  Response.AddHeader("content-disposition", "attachment;filename=Export.pdf");
		//                  Response.Cache.SetCacheability(HttpCacheability.NoCache);
		//                  Response.Write(pdfDoc);
		//                  Response.End();
		//              }
		//              return View();
		//} 
		[HttpGet]
		public async Task<IActionResult> GetPdf()
		{
			var model = ClientService.GetClient();
			var htmlData = await this.viewRenderService.RenderToStringAsync("~/Views/Reports/ExportInvoiceToPdf.cshtml", model);

			//TODO Convert is not working						
			byte[] fileContents = this.htmlToPdfConverter.Convert(htmlData);

			return this.File(fileContents, "application/pdf");
		}
		//public FileResult ExportPdf2(string exportData)
		//{
		//	using (MemoryStream stream = new System.IO.MemoryStream())
		//	{
		//		StringReader reader = new StringReader(exportData);
		//		iTextSharp.text.Document PdfFile = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
		//		PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
		//		PdfFile.Open();
		//		XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
		//		PdfFile.Close();
		//		return File(stream.ToArray(), "application/pdf", "exportData.pdf");
		//	}
		//}

		public FileResult ExportToExcel()
		{
			InvoiceDetailsViewModel invoice = ClientService.GetClient();

			using (var excelDocument = new XLWorkbook())
			{
				IXLWorksheet worksheet = excelDocument.Worksheets.Add("Invoice");
				worksheet.Cell(1, 1).Value = "Invoice No";
				worksheet.Cell(1, 2).Value = invoice.DocumentNumber;
				worksheet.Cell(2, 1).Value = "Date";
				worksheet.Cell(2, 2).Value = invoice.Date;

                if (invoice.DocumentType == Data.Enums.DocumentTypes.CreditNote.ToString())
                {				

                    worksheet.Cell(3, 1).Value = "Към фактура";
                    worksheet.Cell(3, 2).Value = invoice.CreditToInvoice;
                    worksheet.Cell(3, 3).Value = "Дата";
                    worksheet.Cell(3, 4).Value = invoice.CreditToInvoiceDate;
                }
                else if (invoice.DocumentType == Data.Enums.DocumentTypes.DebitNote.ToString())
                {
                    worksheet.Cell(3, 1).Value = "Към фактура";
                    worksheet.Cell(3, 2).Value = invoice.DebitToInvoice;
                    worksheet.Cell(3, 3).Value = "Дата";
                    worksheet.Cell(3, 4).Value = invoice.DebitToInvoiceDate;
                }

                worksheet.Cell(4, 1).Value = "Customer";
				worksheet.Cell(4, 2).Value = invoice.Customer.Name;
				worksheet.Cell(4, 7).Value = "Seller";
				worksheet.Cell(4, 8).Value = invoice.Seller.Name;

				worksheet.Cell(5, 1).Value = "EIK";
				worksheet.Cell(5, 2).Value = invoice.Customer.EIK;
				worksheet.Cell(5, 7).Value = "EIK";
				worksheet.Cell(5, 8).Value = invoice.Seller.EIK;

				worksheet.Cell(6, 1).Value = "VAT";
				worksheet.Cell(6, 2).Value = invoice.Customer.VAT;
				worksheet.Cell(6, 7).Value = "VAT";
				worksheet.Cell(6, 8).Value = invoice.Seller.VAT;

				worksheet.Cell(7, 1).Value = "Address";
				worksheet.Cell(7, 2).Value = invoice.Customer.Country;
				worksheet.Cell(7, 3).Value = invoice.Customer.City;
				worksheet.Cell(7, 4).Value = invoice.Customer.Street;
				worksheet.Cell(7, 7).Value = "Address";
				worksheet.Cell(7, 8).Value = invoice.Seller.Country;
				worksheet.Cell(7, 9).Value = invoice.Seller.City;
				worksheet.Cell(7, 10).Value = invoice.Seller.Street;

				int i = 2;
				worksheet.Cell(9, 1).Value = "Order confirmation No";
				foreach (var product in invoice.OrderConfirmationNumber)
				{
					worksheet.Cell(9, i).Value = product;
					i++;
				}

				worksheet.Cell(9, 1).Value = "Order confirmation No";
				worksheet.Cell(9, 2).Value = invoice.OrderConfirmationNumber.ToString();

				worksheet.Cell(10, 1).Value = "Delivery Terms";
				worksheet.Cell(10, 2).Value = invoice.Incoterms;

				worksheet.Cell(11, 1).Value = "Truck No";
				worksheet.Cell(11, 2).Value = invoice.TruckNumber;

				worksheet.Cell(12, 1).Value = "Net Weight:";
				worksheet.Cell(12, 2).Value = invoice.NetWeight;
				worksheet.Cell(12, 3).Value = "Gross Weight:";
				worksheet.Cell(12, 4).Value = invoice.GrossWeight;

				worksheet.Cell(14, 1).Value = "No";
				worksheet.Cell(14, 2).Value = "Description";
				worksheet.Cell(14, 3).Value = "Grade";
				worksheet.Cell(14, 4).Value = "Size";
				worksheet.Cell(14, 5).Value = "FSC Claim";
				worksheet.Cell(14, 6).Value = "FSC Certificate";
				worksheet.Cell(14, 7).Value = "Unit";
				worksheet.Cell(14, 8).Value = "Quantity";
				worksheet.Cell(14, 9).Value = "Unit Price";
				worksheet.Cell(14, 10).Value = "Amount";

				IXLRange range = worksheet.Range(worksheet.Cell(14, 1).Address, worksheet.Cell(14, 10).Address);
				range.Style.Fill.SetBackgroundColor(XLColor.TurquoiseGreen);

				int row = 15;
				i = 1;

				foreach (var product in invoice.Products)
				{
					worksheet.Cell(row, 1).Value = i;
					worksheet.Cell(row, 2).Value = product.Description;
					worksheet.Cell(row, 3).Value = product.Grade;
					worksheet.Cell(row, 4).Value = product.Size;
					worksheet.Cell(row, 5).Value = product.FscClaim;
					worksheet.Cell(row, 6).Value = product.FscCertificate;
					worksheet.Cell(row, 7).Value = product.Unit.ToString();
					worksheet.Cell(row, 8).Value = product.InvoicedQuantity;
					worksheet.Cell(row, 9).Value = product.SellPrice;
					worksheet.Cell(row, 10).Value = product.Amount;

					row++;
					i++;
				}

				row++;
				worksheet.Cell(row, 1).Value = "Amount";
				worksheet.Cell(row, 2).Value = invoice.Amount;
				row++;
				worksheet.Cell(row, 1).Value = "VAT %";
				worksheet.Cell(row, 2).Value = invoice.VatAmount;
				row++;
				worksheet.Cell(row, 1).Value = "Total Amount";
				worksheet.Cell(row, 2).Value = invoice.TotalAmount;
				row++;

				worksheet.Cell(row, 1).Value = "Deal Type";
				worksheet.Cell(row, 2).Value = invoice.DealTypeEng;
				row++;
				worksheet.Cell(row, 1).Value = "Description of the deal";
				worksheet.Cell(row, 2).Value = invoice.DealDescriptionEng;
				row++;
				worksheet.Cell(row, 1).Value = "Event date";
				worksheet.Cell(row,2).Value = invoice.Date.ToShortTimeString();
				row++;

                worksheet.Cell(row,1).Value = "Payment Terms";
				worksheet.Cell(row, 2).Value = invoice.PaymentTerms;

                foreach (var bank in invoice.CompanyBankDetails)
				{
					row++;
					worksheet.Cell(row, 1).Value = "Currency";
					worksheet.Cell(row, 2).Value = bank.CurrencyName;
					worksheet.Cell(row, 3).Value = "Bank";
					worksheet.Cell(row, 4).Value = bank.BankName;
					worksheet.Cell(row, 5).Value = "IBAN";
					worksheet.Cell(row, 6).Value = bank.Iban;
					worksheet.Cell(row, 7).Value = "Swift";
					worksheet.Cell(row, 8).Value = bank.Swift;
				}

				using (var stream = new MemoryStream())
				{
					excelDocument.SaveAs(stream);
					var content = stream.ToArray();
					string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

					var strDate = DateTime.Now.ToString("yyyyMMdd");
					string filename = string.Format($"Invoice_{strDate}.xlsx");

					return File(content, contentType, filename);
				}
			}
		}
        public FileResult ExportToExcelBg()
        {
            BgInvoiceViewModel invoice = ClientService.GetBgInvoice();

            using (var excelDocument = new XLWorkbook())
            {
                IXLWorksheet worksheet = excelDocument.Worksheets.Add("Фактура");
                worksheet.Cell(1, 1).Value = "Фактура No";
                worksheet.Cell(1, 2).Value = invoice.DocumentNumber;
                worksheet.Cell(2, 1).Value = "Дата";
                worksheet.Cell(2, 2).Value = invoice.Date;

				if(invoice.DocumentType == Data.Enums.DocumentTypes.CreditNote.ToString())
				{
					worksheet.Cell(3, 1).Value = "Към фактура";
					worksheet.Cell(3, 2).Value = invoice.CreditToInvoiceNumber;
					worksheet.Cell(3, 3).Value = "Дата";
					worksheet.Cell(3, 4).Value = invoice.CreditToInvoiceDate;
				}
                else if (invoice.DocumentType ==  Data.Enums.DocumentTypes.DebitNote.ToString())
                {
                    worksheet.Cell(3, 1).Value = "Към фактура";
                    worksheet.Cell(3, 2).Value = invoice.DebitToInvoiceNumber;
                    worksheet.Cell(3, 3).Value = "Дата";
                    worksheet.Cell(3, 4).Value = invoice.DebitToInvoiceDate;
                }

                worksheet.Cell(4, 1).Value = "Клиент";
                worksheet.Cell(4, 2).Value = invoice.BgCustomer.BgName;
                worksheet.Cell(4, 7).Value = "Доставчик";
                worksheet.Cell(4, 8).Value = invoice.BgMyCompany.BgName;

                worksheet.Cell(5, 1).Value = "EИК No";
                worksheet.Cell(5, 2).Value = invoice.BgCustomer.EIK;
                worksheet.Cell(5, 7).Value = "ЕИК No";
                worksheet.Cell(5, 8).Value = invoice.BgMyCompany.EIK;

                worksheet.Cell(6, 1).Value = "ДДС No";
                worksheet.Cell(6, 2).Value = invoice.BgCustomer.VAT;
                worksheet.Cell(6, 7).Value = "ДДС No";
                worksheet.Cell(6, 8).Value = invoice.BgMyCompany.VAT;

                worksheet.Cell(7, 1).Value = "Адрес";
                worksheet.Cell(7, 2).Value = invoice.BgCustomer.ClientAddress.BgCountry;
                worksheet.Cell(7, 3).Value = invoice.BgCustomer.ClientAddress.BgCity;
                worksheet.Cell(7, 4).Value = invoice.BgCustomer.ClientAddress.BgStreet;
                worksheet.Cell(7, 7).Value = "Адрес";
                worksheet.Cell(7, 8).Value = invoice.BgMyCompany.BgCountry;
                worksheet.Cell(7, 9).Value = invoice.BgMyCompany.BgCity;
                worksheet.Cell(7, 10).Value = invoice.BgMyCompany.BgStreet;
             
                worksheet.Cell(9, 1).Value = "No";
                worksheet.Cell(9, 2).Value = "Oписание";
                worksheet.Cell(9, 3).Value = "Качество";
                worksheet.Cell(9, 4).Value = "Размер";
                worksheet.Cell(9, 5).Value = "FSC Claim";
                worksheet.Cell(9, 6).Value = "FSC Certificate";
                worksheet.Cell(9, 7).Value = "Мер.ед.";
                worksheet.Cell(9, 8).Value = "Количество";
                worksheet.Cell(9, 9).Value = "Цена лв.";
                worksheet.Cell(9, 10).Value = "Сума лв.";

                IXLRange range = worksheet.Range(worksheet.Cell(9, 1).Address, worksheet.Cell(9, 10).Address);
                range.Style.Fill.SetBackgroundColor(XLColor.TurquoiseGreen);

                int row = 10;
               int i = 1;

                foreach (var product in invoice.BgProducts)
                {
                    worksheet.Cell(row, 1).Value = i;
                    worksheet.Cell(row, 2).Value = product.BgDescription;
                    worksheet.Cell(row, 3).Value = product.Grade;
                    worksheet.Cell(row, 4).Value = product.Size;
                    worksheet.Cell(row, 5).Value = product.FSCClaim;
                    worksheet.Cell(row, 6).Value = product.FSCSertificate;
                    worksheet.Cell(row, 7).Value = product.Unit.ToString();
                    worksheet.Cell(row, 8).Value = product.InvoicedQuantity;
                    worksheet.Cell(row, 9).Value = product.BgPrice;
                    worksheet.Cell(row, 10).Value = product.BgAmount;

                    row++;
                    i++;
                }

                row++;
                worksheet.Cell(row, 1).Value = "Сума без ДДС";
                worksheet.Cell(row, 2).Value = invoice.Amount;
                row++;
                worksheet.Cell(row, 1).Value = "ДДС %";
                worksheet.Cell(row, 2).Value = invoice.VatAmount;
                row++;
                worksheet.Cell(row, 1).Value = "Обща сума";
                worksheet.Cell(row, 2).Value = invoice.TotalAmount;
                row++;

                worksheet.Cell(row, 1).Value = "Основание на сделката";
                worksheet.Cell(row, 2).Value = invoice.DealTypeBg;
                row++;
                worksheet.Cell(row, 1).Value = "Описание на сделката";
                worksheet.Cell(row, 2).Value = invoice.DealDescriptionBg;
                row++;
                worksheet.Cell(row, 1).Value = "Дата на данъчно събитие";
                worksheet.Cell(row, 2).Value = invoice.Date.ToShortTimeString();

                foreach (var bank in invoice.CompanyBankDetails)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = "Валута";
                    worksheet.Cell(row, 2).Value = bank.Currency;
                    worksheet.Cell(row, 3).Value = "Банка";
                    worksheet.Cell(row, 4).Value = bank.BankName;
                    worksheet.Cell(row, 5).Value = "IBAN";
                    worksheet.Cell(row, 6).Value = bank.Iban;
                    worksheet.Cell(row, 7).Value = "Swift";
                    worksheet.Cell(row, 8).Value = bank.Swift;
                }

                using (var stream = new MemoryStream())
                {
                    excelDocument.SaveAs(stream);
                    var content = stream.ToArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var strDate = DateTime.Now.ToString("yyyyMMdd");
                    string filename = string.Format($"Invoice_{strDate}.xlsx");

                    return File(content, contentType, filename);
                }
            }
        }
        public FileResult ExportProductsToExcel()
		{
			IEnumerable<ProductAvailabilityDetailsViewModel> products = ClientService.GetProductOnStock();

			using (var excelDocument = new XLWorkbook())
			{
				IXLWorksheet worksheet = excelDocument.Worksheets.Add("Products");

				worksheet.Cell(1, 1).Value = "No";
				worksheet.Cell(1, 2).Value = "Description";
				worksheet.Cell(1, 3).Value = "Quantity";
				worksheet.Cell(1, 4).Value = "Supplier Name";
				int i = 2;
				int j = 2;
				foreach (var product in products)
				{
					worksheet.Cell(j, 1).Value = i;
					worksheet.Cell(j, 2).Value = product.Description;
					worksheet.Cell(j, 3).Value = product.QuantityOnStock;
					worksheet.Cell(j, 4).Value = product.SupplierName;
					++j;
					worksheet.Cell(j, 2).Value = "Purchase Details";
					++j;
					worksheet.Cell(j, 2).Value = "Purchase Number";
					worksheet.Cell(j, 3).Value = "Purchase Date";
					worksheet.Cell(j, 4).Value = "Delivery Address";
					worksheet.Cell(j, 5).Value = "FSC";
					worksheet.Cell(j, 6).Value = "Loaded Quantity";
					worksheet.Cell(j, 7).Value = "Unit";
					worksheet.Cell(j, 8).Value = "Pallets";
                    worksheet.Cell(j, 9).Value = "Sheets";
                    worksheet.Cell(j, 10).Value = "Purchase Price";
					worksheet.Cell(j, 11).Value = "Cost Price";
                    IXLRange rangeOne = worksheet.Range(worksheet.Cell(j, 2).Address, worksheet.Cell(j, 11).Address);
                    rangeOne.Style.Fill.SetBackgroundColor(XLColor.Alizarin);
                    ++j;
					foreach (var purchase in product.PurchaseProductDetails)
					{
						worksheet.Cell(j, 2).Value = purchase.PurchaseNumber;
                        worksheet.Cell(j, 3).Value = purchase.PurchaseDate;
                        worksheet.Cell(j, 4).Value = purchase.DeliveryAddress;
                        worksheet.Cell(j, 5).Value = purchase.FSCClaim;
                        worksheet.Cell(j, 6).Value = purchase.LoadedQuantity;
                        worksheet.Cell(j, 7).Value = purchase.Unit;
                        worksheet.Cell(j, 8).Value = purchase.Pallets;
						worksheet.Cell(j, 9).Value = purchase.SheetsPerPallet;
						worksheet.Cell(j, 10).Value = purchase.PurchasePrice;
						worksheet.Cell(j, 11).Value = purchase.CostPrice;
                        ++j;
					}

					worksheet.Cell(j, 2).Value = "Customer Orders";
                    
                    ++j;
					worksheet.Cell(j, 2).Value = "Name";
                    worksheet.Cell(j, 3).Value = "Order Confirmation No";
                    worksheet.Cell(j, 4).Value = "Customer PO No";
                    worksheet.Cell(j, 5).Value = "Delivery Address";
					worksheet.Cell(j, 6).Value = "Price";
                    IXLRange rangeTwo = worksheet.Range(worksheet.Cell(j, 2).Address, worksheet.Cell(j, 6).Address);
                    rangeTwo.Style.Fill.SetBackgroundColor(XLColor.Alizarin);

                    foreach (var order in product.CustomerProductsDetails)
					{
						++j;
						worksheet.Cell(j, 2).Value = order.CustomerName;
						worksheet.Cell(j, 3).Value = order.CustomerOrderNumber;
						worksheet.Cell(j, 4).Value = order.CustomerPoNumber;
						worksheet.Cell(j, 5).Value = order.DeliveryAddress;
						worksheet.Cell(j, 6).Value = order.Price;
					}
					i++;					
				}

				IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 4).Address);
				range.Style.Fill.SetBackgroundColor(XLColor.TurquoiseGreen);

				using (var stream = new MemoryStream())
				{
					excelDocument.SaveAs(stream);
					var content = stream.ToArray();
					string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

					var strDate = DateTime.Now.ToString("yyyyMMdd");
					string filename = string.Format($"ProductsOnStock_{strDate}.xlsx");

					return File(content, contentType, filename);
				}

			}

		}
		public FileResult ExportFscToExcel()
		{
            ProductsFscCollectionViewModel fscReport = ClientService.GetFscReport();

            using (var excelDocument = new XLWorkbook())
            {
                IXLWorksheet worksheet = excelDocument.Worksheets.Add("FscReport");

                worksheet.Cell(1, 1).Value = "Company Name";
                worksheet.Cell(1, 2).Value = "From";
                worksheet.Cell(1, 3).Value = "To";
                worksheet.Cell(1, 4).Value = "Fsc claim";
				worksheet.Cell(1, 5).Value = "Subject";
                worksheet.Cell(2, 1).Value = fscReport.MyCompany;
                worksheet.Cell(2, 2).Value = fscReport.StartDate;
                worksheet.Cell(2, 3).Value = fscReport.EndDate;
                worksheet.Cell(2, 4).Value = fscReport.FSCClaim;
                worksheet.Cell(2, 5).Value = fscReport.PurchaseOrSell;
                int i = 4;
				
				if(fscReport.PurchaseOrSell == "Purchase")
				{
					worksheet.Cell(i, 1).Value = "Supplier Name";
					worksheet.Cell(i, 2).Value = "Invoice No";
					worksheet.Cell(i, 3).Value = "Date";
					worksheet.Cell(i, 4).Value = "Fsc Claim";
					worksheet.Cell(i, 5).Value = "Description";
					worksheet.Cell(i, 6).Value = "Purchase Quantity";
					worksheet.Cell(i, 7).Value = "Unit";
					worksheet.Cell(i, 8).Value = "Trasnport";
                    
                    foreach (var fsc in fscReport.PurchaseProducts)
					{
                        i++;
                        worksheet.Cell(i, 1).Value = fsc.SupplierName;
                        worksheet.Cell(i, 2).Value = fsc.PurchaseInvoice;
                        worksheet.Cell(i, 3).Value = fsc.PurchaseDate;
                        worksheet.Cell(i, 4).Value = fsc.FscClaim;
                        worksheet.Cell(i, 5).Value = fsc.Description;
                        worksheet.Cell(i, 6).Value = fsc.Quantity;
                        worksheet.Cell(i, 7).Value = fsc.Unit;
                        worksheet.Cell(i, 8).Value = fsc.Transport;

                    }
					
				}
				else if (fscReport.PurchaseOrSell == "Sell")
				{
                   
                    worksheet.Cell(i, 1).Value = "Customer Name";
                    worksheet.Cell(i, 2).Value = "Invoice No";
                    worksheet.Cell(i, 3).Value = "Date";
                    worksheet.Cell(i, 4).Value = "Fsc Claim";
                    worksheet.Cell(i, 5).Value = "Description";
                    worksheet.Cell(i, 6).Value = "Quantity";
                    worksheet.Cell(i, 7).Value = "Unit";
                    worksheet.Cell(i, 8).Value = "Trasnport";
                    worksheet.Cell(i, 9).Value = "Supplier";
                    worksheet.Cell(i, 10).Value = "Purchase Invoice";

                    foreach (var fsc in fscReport.SoldProducts)
                    {
                        i++;
                        worksheet.Cell(i, 1).Value = fsc.CustomerName;
                        worksheet.Cell(i, 2).Value = fsc.InvoiceNumber;
                        worksheet.Cell(i, 3).Value = fsc.Date;
                        worksheet.Cell(i, 4).Value = fsc.FscClaim;
                        worksheet.Cell(i, 5).Value = fsc.Description;
                        worksheet.Cell(i, 6).Value = fsc.Quantity;
                        worksheet.Cell(i, 7).Value = fsc.Unit;
                        worksheet.Cell(i, 8).Value = fsc.Transport;
						worksheet.Cell(i, 9).Value = fsc.Supplier;
						worksheet.Cell(i, 10).Value = fsc.PurchaseInvoice;
                    }
                }
               
                IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 5).Address);
                range.Style.Fill.SetBackgroundColor(XLColor.TurquoiseGreen);

                IXLRange rangeOne = worksheet.Range(worksheet.Cell(4, 1).Address, worksheet.Cell(4, 10).Address);
                rangeOne.Style.Fill.SetBackgroundColor(XLColor.Alizarin);

                using (var stream = new MemoryStream())
                {
                    excelDocument.SaveAs(stream);
                    var content = stream.ToArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var strDate = DateTime.Now.ToString("yyyyMMdd");
                    string filename = string.Format($"FscReport_{strDate}.xlsx");

                    return File(content, contentType, filename);
                }
            }
        }
        public FileResult ExportPurchasePaymentToExcel()
        {
            SupplierOrdersPaymentReportViewModel payments = ClientService.GetPurchasePayments();

            using (var excelDocument = new XLWorkbook())
            {
                IXLWorksheet worksheet = excelDocument.Worksheets.Add("PaymentsToSuppliers");
                int row = 1;
                for (int i = 0; i < payments.SupplierOrderPaymentCollection.Count(); i++)
				{
                    worksheet.Cell(row, 1).Value = "Supplier Order";
                    worksheet.Cell(row, 2).Value = "Date";
                    worksheet.Cell(row, 3).Value = "Payment status";
                    worksheet.Cell(row, 4).Value = "Amount";
                    worksheet.Cell(row, 5).Value = "Currency";
                    worksheet.Cell(row, 6).Value = "Balance";
                    worksheet.Cell(row, 7).Value = "Advance Payment";
                    worksheet.Cell(row, 8).Value = "Date Paid advance";
					row++;
					worksheet.Cell(row, 1).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).SupplierOrderNumber;
                    worksheet.Cell(row, 2).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).Date;
                    worksheet.Cell(row, 3).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).PaidStatus;
                    worksheet.Cell(row, 4).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).TotalAmount;
                    worksheet.Cell(row, 5).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).PurchaseCurrency;
                    worksheet.Cell(row, 6).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).Balance;
                    worksheet.Cell(row, 7).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).PaidAvance;
                    worksheet.Cell(row, 8).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).DatePaidAmount;
					row++;
					worksheet.Cell(row, 1).Value = "Payments";
                    row++;
                    for (int j = 0; j < payments.SupplierOrderPaymentCollection.ElementAt(i).Payments.Count(); j++)
					{
                        worksheet.Cell(row, 1).Value = "Paid amount";
                        worksheet.Cell(row, 2).Value = "Date";
                        row++;
                        worksheet.Cell(row, 1).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).Payments.ElementAt(j).PaidAmount;
                        worksheet.Cell(row, 2).Value = payments.SupplierOrderPaymentCollection.ElementAt(i).Payments.ElementAt(j).Date;
						row++;
                    }
                    worksheet.Cell(row,1).Value = "Purchase payments";
                    foreach (var payment in payments.SupplierOrderPaymentCollection.ElementAt(i).PurchasePaymentsCollection)
                    {
                        row++;
                        worksheet.Cell(row, 1).Value = "Purchase invoice";
                        worksheet.Cell(row, 2).Value = "Balance";
                        worksheet.Cell(row, 3).Value = "Advance payment";
                        worksheet.Cell(row, 4).Value = "Date";
                        row++;
                        worksheet.Cell(row, 1).Value = payment.PurchaseNumber;
                        worksheet.Cell(row, 2).Value = payment.Balance;
                        worksheet.Cell(row, 3).Value = payment.PaidAdvance;
                        worksheet.Cell(row, 4).Value = payment.DatePaidAmount;

                        foreach (var item in payment.PurchasePaymentsDetails)
                        {
                            row++;
                            worksheet.Cell(row, 1).Value = "Paid amount";
                            worksheet.Cell(row, 2).Value = "Date";
                            row++;
                            worksheet.Cell(row, 1).Value = item.PaidAmount;
                            worksheet.Cell(row, 2).Value = item.Date;
                        }
                    }
                }

                IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 8).Address);
                range.Style.Fill.SetBackgroundColor(XLColor.BlueGreen);
                
                using (var stream = new MemoryStream())
                {
                    excelDocument.SaveAs(stream);
                    var content = stream.ToArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var strDate = DateTime.Now.ToString("yyyyMMdd");
                    string filename = string.Format($"PaymentsToSuppliers_{strDate}.xlsx");

                    return File(content, contentType, filename);
                }

            }

        }
        public FileResult ExportCustomerInvoicePaymentsToExcel()
        {
            CustomerInvoicePaymentsReportsViewModel payments = ClientService.GetCustomerInvoicePayments();

            using (var excelDocument = new XLWorkbook())
            {
                IXLWorksheet worksheet = excelDocument.Worksheets.Add("PaymentsFromCustomers");

                worksheet.Cell(1, 1).Value = "Customer Name";
				worksheet.Cell(1, 2).Value = payments.CustomerName;
                int row = 2;

                for (int i = 0; i < payments.CustomerPaymentCollection.Count(); i++)
                {
                    worksheet.Cell(row, 1).Value = "Invoice No";
                    worksheet.Cell(row, 2).Value = "Date";
                    worksheet.Cell(row, 3).Value = "Payment status";
                    worksheet.Cell(row, 4).Value = "Total amount";
                    worksheet.Cell(row, 5).Value = "Paid advance";
                    worksheet.Cell(row, 6).Value = "Date paid amount";
                    worksheet.Cell(row, 7).Value = "Balance";
                    
                    row++;
                    worksheet.Cell(row, 1).Value = payments.CustomerPaymentCollection.ElementAt(i).DocumentNumber;
                    worksheet.Cell(row, 2).Value = payments.CustomerPaymentCollection.ElementAt(i).Date;
                    worksheet.Cell(row, 3).Value = payments.CustomerPaymentCollection.ElementAt(i).PaidStatus;
					worksheet.Cell(row, 4).Value = payments.CustomerPaymentCollection.ElementAt(i).TotalAmount;
                    worksheet.Cell(row, 5).Value = payments.CustomerPaymentCollection.ElementAt(i).PaidAvance;
                    worksheet.Cell(row, 6).Value = payments.CustomerPaymentCollection.ElementAt(i).DatePaidAmount;
                    worksheet.Cell(row, 7).Value = payments.CustomerPaymentCollection.ElementAt(i).Balance;
                    
                    row++;
                    worksheet.Cell(row, 1).Value = "Payments";
                    row++;
                    for (int j = 0; j < payments.CustomerPaymentCollection.ElementAt(i).Payments.Count(); j++)
                    {
                        worksheet.Cell(row, 1).Value = "Paid amount";
                        worksheet.Cell(row, 2).Value = "Date";
                        row++;
                        worksheet.Cell(row, 1).Value = payments.CustomerPaymentCollection.ElementAt(i).Payments.ElementAt(j).PaidAmount;
                        worksheet.Cell(row, 2).Value = payments.CustomerPaymentCollection.ElementAt(i).Payments.ElementAt(j).Date;
                        row++;
                    }
                    worksheet.Cell(row, 1).Value = "Customer order payments Details";
                    foreach (var payment in payments.CustomerPaymentCollection.ElementAt(i).CustomerOrders)
                    {
                        row++;
                        worksheet.Cell(row, 1).Value = "Customer Order N";
                        worksheet.Cell(row, 2).Value = "Balance";
                        worksheet.Cell(row, 3).Value = "Advance payment";
                        worksheet.Cell(row, 4).Value = "Date";
                        row++;
                        worksheet.Cell(row, 1).Value = payment.OrderConfirmationNumber;
                        worksheet.Cell(row, 2).Value = payment.Balance;
                        worksheet.Cell(row, 3).Value = payment.PaidAvance;
                        worksheet.Cell(row, 4).Value = payment.DateAdvancePayment;

                        foreach (var item in payment.Payments)
                        {
                            row++;
                            worksheet.Cell(row, 1).Value = "Paid amount";
                            worksheet.Cell(row, 2).Value = "Date";
                            row++;
                            worksheet.Cell(row, 1).Value = item.PaidAmount;
                            worksheet.Cell(row, 2).Value = item.Date;
                        }
                    }
                }

                IXLRange range = worksheet.Range(worksheet.Cell(2, 1).Address, worksheet.Cell(2, 7).Address);
                range.Style.Fill.SetBackgroundColor(XLColor.BlueGreen);

                using (var stream = new MemoryStream())
                {
                    excelDocument.SaveAs(stream);
                    var content = stream.ToArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var strDate = DateTime.Now.ToString("yyyyMMdd");
                    string filename = string.Format($"PaymentsFromCustomers_{strDate}.xlsx");

                    return File(content, contentType, filename);
                }

            }

        }
		public FileResult ExportCoToExcel()
		{
		  CustomerOrderPrintViewModel order = ClientService.GetCustomerOrderPrint();
            using (var excelDocument = new XLWorkbook())
            {
                IXLWorksheet worksheet = excelDocument.Worksheets.Add("Invoice");
                worksheet.Cell(1, 1).Value = order.Type;
                worksheet.Cell(1, 2).Value = order.OrderConfirmationNumber;
                worksheet.Cell(2, 1).Value = "Date";
                worksheet.Cell(2, 2).Value = order.Date;
				worksheet.Cell(3, 1).Value = "Customer PO No:";
				worksheet.Cell(3, 2).Value = order.CustomerPoNumber;
				               
                worksheet.Cell(4, 1).Value = "Customer";
                worksheet.Cell(4, 2).Value = order.Customer.Name;
                worksheet.Cell(4, 7).Value = "Seller";
                worksheet.Cell(4, 8).Value = order.MyCompany.MyCompanyName;

                worksheet.Cell(5, 1).Value = "EIK";
                worksheet.Cell(5, 2).Value = order.Customer.EIK;
                worksheet.Cell(5, 7).Value = "EIK";
                worksheet.Cell(5, 8).Value = order.MyCompany.Eik;

                worksheet.Cell(6, 1).Value = "VAT";
                worksheet.Cell(6, 2).Value = order.Customer.VAT;
                worksheet.Cell(6, 7).Value = "VAT";
                worksheet.Cell(6, 8).Value = order.MyCompany.Vat;

                worksheet.Cell(7, 1).Value = "Address";
                worksheet.Cell(7, 2).Value = order.Customer.Country;
                worksheet.Cell(7, 3).Value = order.Customer.City;
                worksheet.Cell(7, 4).Value = order.Customer.Street;
                worksheet.Cell(7, 7).Value = "Address";
                worksheet.Cell(7, 8).Value = order.MyCompany.Country;
                worksheet.Cell(7, 9).Value = order.MyCompany.City;
                worksheet.Cell(7, 10).Value = order.MyCompany.Street;

				if(order.FiscalAgent != null)
				{
					worksheet.Cell(8, 1).Value = "Fiscal Agent";
					worksheet.Cell(8, 2).Value = order.FiscalAgent.Name;
					worksheet.Cell(9, 1).Value = "Address:";
					worksheet.Cell(9, 2).Value = order.FiscalAgent.Details;
				}
               
                worksheet.Cell(10, 1).Value = "Delivery Terms";
                worksheet.Cell(10, 2).Value = order.DeliveryTerms;

				worksheet.Cell(10, 4).Value = "Delivery Address";
				worksheet.Cell(10, 5).Value = order.DeliveryAddress;

                worksheet.Cell(11, 1).Value = "Truck No";
                worksheet.Cell(11, 2).Value = "-";

                worksheet.Cell(12, 1).Value = "Net Weight:";
                worksheet.Cell(12, 2).Value = "-";
                worksheet.Cell(12, 3).Value = "Gross Weight:";
                worksheet.Cell(12, 4).Value = "-";
				worksheet.Cell(13, 1).Value = "Comment";
				worksheet.Cell(13, 2).Value = order.Comment;	

                worksheet.Cell(14, 1).Value = "No";
                worksheet.Cell(14, 2).Value = "Description";
                worksheet.Cell(14, 3).Value = "Grade";
                worksheet.Cell(14, 4).Value = "Size";
                worksheet.Cell(14, 5).Value = "FSC Claim";
                worksheet.Cell(14, 6).Value = "FSC Certificate";
                worksheet.Cell(14, 7).Value = "Unit";
                worksheet.Cell(14, 8).Value = "Quantity";
                worksheet.Cell(14, 9).Value = "Unit Price";
                worksheet.Cell(14, 10).Value = "Amount";

                IXLRange range = worksheet.Range(worksheet.Cell(14, 1).Address, worksheet.Cell(14, 10).Address);
                range.Style.Fill.SetBackgroundColor(XLColor.TurquoiseGreen);

                int row = 15;
                int i = 1;

                foreach (var product in order.Products)
                {
                    worksheet.Cell(row, 1).Value = i;
                    worksheet.Cell(row, 2).Value = product.Description;
                    worksheet.Cell(row, 3).Value = product.Grade;
                    worksheet.Cell(row, 4).Value = product.Size;
                    worksheet.Cell(row, 5).Value = product.FSCClaim;
                    worksheet.Cell(row, 6).Value = product.FscCertificate;
                    worksheet.Cell(row, 7).Value = product.Unit.ToString();
                    worksheet.Cell(row, 8).Value = product.Quantity;
                    worksheet.Cell(row, 9).Value = product.SellPrice;
                    worksheet.Cell(row, 10).Value = product.Amount;

                    row++;
                    i++;
                }

                row++;
                worksheet.Cell(row, 1).Value = "Amount";
                worksheet.Cell(row, 2).Value = order.Amount;
                row++;
                worksheet.Cell(row, 1).Value = "VAT %";
                worksheet.Cell(row, 2).Value = order.Vat;
                row++;
                worksheet.Cell(row, 1).Value = "Total Amount";
                worksheet.Cell(row, 2).Value = order.TotalAmount;
                row++;

				if(order.FscText != null)
				{
					worksheet.Cell(row,1).Value = order.FscText;
					worksheet.Cell(row++, 1).Value = "Licence No:";
					worksheet.Cell(row,2).Value = order.MyCompany.FscCertificate;
				}
				row++;
                worksheet.Cell(row, 1).Value = "Deal Type";
                worksheet.Cell(row, 2).Value = order.DealType;
                row++;
                worksheet.Cell(row, 1).Value = "Description of the deal";
                worksheet.Cell(row, 2).Value = order.DealDescription;
                row++;
                worksheet.Cell(row, 1).Value = "Event date";
                worksheet.Cell(row, 2).Value = order.Date.ToShortTimeString();

				worksheet.Cell(row, 4).Value = "Payment Terms";
				worksheet.Cell(row, 5).Value = order.PaymentTerms;

                foreach (var bank in order.BankDetails)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = "Currency";
                    worksheet.Cell(row, 2).Value = bank.CurrencyName;
                    worksheet.Cell(row, 3).Value = "Bank";
                    worksheet.Cell(row, 4).Value = bank.BankName;
                    worksheet.Cell(row, 5).Value = "IBAN";
                    worksheet.Cell(row, 6).Value = bank.Iban;
                    worksheet.Cell(row, 7).Value = "Swift";
                    worksheet.Cell(row, 8).Value = bank.Swift;
                }

                using (var stream = new MemoryStream())
                {
                    excelDocument.SaveAs(stream);
                    var content = stream.ToArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var strDate = DateTime.Now.ToString("yyyyMMdd");
                    string filename = string.Format($"Invoice_{strDate}.xlsx");

                    return File(content, contentType, filename);
                }
            }
        }
		public FileResult ExportServiceOrderExcel()
		{
			ServiceOrderDetailsPrintViewModel order = ClientService.GetServiceOrder();
           
			using (var excelDocument = new XLWorkbook())
            {
                IXLWorksheet worksheet = excelDocument.Worksheets.Add("Invoice");
				worksheet.Cell(1, 1).Value = "Заявка/Order Nо:";
                worksheet.Cell(1, 2).Value = order.Number;				
                worksheet.Cell(2, 1).Value = "Date";
                worksheet.Cell(2, 2).Value = order.Date;
                worksheet.Cell(3, 1).Value = "Заявител/Applicant:";
                worksheet.Cell(3, 2).Value = "Превозвач/Transport Company:";
                worksheet.Cell(4, 1).Value = order.MyCompany.Name;
                worksheet.Cell(4, 2).Value = order.TransportCompany.Name;
                worksheet.Cell(5, 1).Value = order.MyCompany.Vat;
                worksheet.Cell(5, 2).Value = order.TransportCompany.Vat;
                worksheet.Cell(6, 1).Value = order.MyCompany.Address;
                worksheet.Cell(6, 2).Value = order.TransportCompany.Address;

                worksheet.Cell(7, 1).Value = "Адрес за кореспонденция/Correspondence address:";
                worksheet.Cell(7, 2).Value = "Данни за товара/Goods:";
                worksheet.Cell(8, 1).Value = order.MyCompany.CorrespondAddress;
                worksheet.Cell(8, 2).Value = order.ProductType;
                worksheet.Cell(9, 2).Value = order.GrossWeight;

                worksheet.Cell(10, 1).Value = "Товарен адрес/Loading address:";
                worksheet.Cell(10, 2).Value = "Дата на товарене/Date of loading:";
                worksheet.Cell(11, 1).Value = order.LoadingAddress;
                worksheet.Cell(11, 2).Value = order.Etd;


                worksheet.Cell(12, 1).Value = "Разтоварен пункт/Discharged address:";
                worksheet.Cell(12, 2).Value = "Детайли митница/Customs details:";
                worksheet.Cell(13, 1).Value = order.DeliveryAddress;
                
                if (order.FiscalAgent != null)
                {
                    worksheet.Cell(13, 2).Value = order.FiscalAgent.FiscalAgentName;
                    worksheet.Cell(14, 2).Value = order.FiscalAgent.FiscalAgentDetails;
                }

                
                worksheet.Cell(15, 1).Value = "Плащане/Payment:";
                worksheet.Cell(15, 3).Value = "Камион No/Truck No:";
				worksheet.Cell(15, 4).Value = order.TruckNumber;

                worksheet.Cell(16, 1).Value = "Навло/Freight:";
                worksheet.Cell(16, 2).Value = order.Cost;
                worksheet.Cell(16, 3).Value = "Шофьор/Driver:";
                worksheet.Cell(16, 4).Value = order.DriverName;

                worksheet.Cell(17, 1).Value = "Валута/Currency:";
                worksheet.Cell(17, 2).Value = order.Currency;
                worksheet.Cell(17, 3).Value = "Теl: ";
                worksheet.Cell(17, 4).Value = order.DriverPhone;
                worksheet.Cell(18, 1).Value = "Метод на плащане/Payment method:";
                worksheet.Cell(18, 2).Value = order.PaymentMethod;
                worksheet.Cell(19, 1).Value = "Срок на плащане/Payment Terms:";
                worksheet.Cell(19, 2).Value = order.PaymentTerms;
                worksheet.Cell(20, 1).Value = "Платец/Paid by:";
                worksheet.Cell(20, 2).Value = order.Payer;

				worksheet.Cell(21, 1).Value = "Коментар/Comment:";
				worksheet.Cell(21, 2).Value = order.Comment;

                worksheet.Cell(22, 1).Value = "С настоящето потвърждаваме следното:";
                worksheet.Cell(22, 2).Value = "We hereby confirm the following:";
				worksheet.Cell(23,1).Value = "1.Тази заявка представлява договор за транспорт.";
				worksheet.Cell(23, 2).Value = "1. This application is a transport contract.";
				worksheet.Cell(24, 1).Value = "2. Превозвачът се задължава да представи валидна застрахова за отговорността " +
					"си за нанесени щети на пратката до\r\n стойности, определени в чл.71, т.3 от Закона за автомобилни превози и чл.23," +
					" т3 и т.4 от Конвенцията за договора за\r\n  международен автомобилен превоз на стоки, покриваща до 12 USD на " +
					"килограм, но не повече от фактурната стойност\r\n  на пратката.";
				worksheet.Cell(24, 2).Value = "2. The carrier shall present a valid insurance for liability for damage to the consignment to the values specified in Article 71, item 3 of\r\n                the Law on Road Transport and Article 23, T3 and 4 of the Convention on the Contract for International carriage of goods by covering\r\n                up to 12 USD per kilogram but not exceeding the invoice value of the consignment.";
				worksheet.Cell(25, 1).Value = "3. При неявяване на автомобила на уговорената дата на товарене, ПРЕВОЗВАЧА заплаща 100 eur неустойка.";
				worksheet.Cell(25, 2).Value = "3. If the car is missing on the agreed loading date, the CARRIER pays a penalty of 100 eur.";
				worksheet.Cell(26, 1).Value = "4. При неспазване на горепосочените условия, възниквалите спорове ще се решават в арбитражния съд в гр.Варна.";
				worksheet.Cell(26, 2).Value = "4. Failure to comply with the above conditions arising disputes will be resolved in the arbitration court in Varna, Bulgaria.";

                using var stream = new MemoryStream();
                excelDocument.SaveAs(stream);
                var content = stream.ToArray();
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var strDate = DateTime.Now.ToString("yyyyMMdd");
                string filename = string.Format($"Invoice_{strDate}.xlsx");

                return File(content, contentType, filename);
            }            
        }
    }
}
