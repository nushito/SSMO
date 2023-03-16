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
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;

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


		public ReportsController(IReportsService service,
		   ICustomerService customerService, ISupplierService supplierService,
		   ICurrency currency, IMycompanyService mycompanyService, IProductService productService,
		   ICustomerOrderService customerOrderService, IStatusService statusService, IInvoiceService invoiceService,
		   IPurchaseService purchaseService, ISupplierOrderService supplierOrderService,
		   IViewRenderService viewRenderService, IHtmlToPdfConverter htmlToPdfConverter) //IWebHostEnvironment environment)
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

		}
		public IActionResult AllCustomerOrders(CustomerOrderReportAll model)
		{
			//TODO When All are selected page is empty
			if (model.CustomerName != null)
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

			model.CustomerOrderCollection = customerOrderCollection.CustomerOrders;
			model.TotalCustomerOrders= customerOrderCollection.TotalCustomerOrders;
			model.CustomerNames = customerNames;
			return View(model);
		}
		public IActionResult CustomerOrderDetails(int id)
		{
			var order = reportService.CustomerOrderDetails(id);

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
				item.Units = productService.GetUnits();
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

			CustomerBySupplierOrdersViewModel cascadeCustomerOrders = new()
			{
				Customers = customersList
			};

			ViewData["Selectedsupplier"] = 0;
			cascadeCustomerOrders.ProductList = null;
			return View(cascadeCustomerOrders);
		}

		[HttpPost]
		[Authorize]
		public IActionResult CustomerOrdersBySupplier(CustomerBySupplierOrdersViewModel model, IFormCollection fc)
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

			var supplierId = fc["SupplierId"];
			ViewData["SelectedSupplier"] = supplierId;
			var ordersList = reportService.GetCustomerOrdersBySupplier(model.CustomerId, supplierId);

			var finalListOrders = new CustomerBySupplierOrdersViewModel
			{
				Customers = customersList,
				CustomerId = int.Parse(model.CustomerId.ToString()),
				ProductList = ordersList,
			};
			return View(finalListOrders);
		}

		[HttpGet]
		public IActionResult GetSupplier(string id)
		{
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
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (model.SupplierName != null)
			{
				string userId = this.User.UserId();

				var listMyCompany = myCompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

				if (!listMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}

			model.SupplierNames = supplierService.GetSupplierNames();

			var supplierOrdersCollection = reportService.AllSupplierOrders
				(model.SupplierName, model.CurrentPage, SupplierOrdersReportAll.SupplierOrdersPerPage);

			model.SupplierOrderCollection = supplierOrdersCollection.SupplierOrders;
			model.TotalSupplierOrders = supplierOrdersCollection.TotalSupplierOrders;

			return View(model);
		}
		public IActionResult SupplierOrderDetails(int id)
		{
			var supplierOrderDetails = reportService.SupplierOrderDetail(id);
			return View(supplierOrderDetails);
		}

		[HttpGet]
		public IActionResult SupplierOrderEdit(int id)
		{
			if (!ModelState.IsValid)
			{
				new SupplierOrderForEditModel
				{
					Currencies = currency.AllCurrency(),
					MyCompanies = myCompanyService.GetAllCompanies(),
					Products = new List<ProductsForEditSupplierOrder>(),
					Statuses = statusService.GetAllStatus()
				};
			}
			var suppplierOrderForEdit = reportService.SupplierOrderForEditDetails(id);
			suppplierOrderForEdit.Currencies = currency.AllCurrency();
			suppplierOrderForEdit.MyCompanies = myCompanyService.GetAllCompanies();
			suppplierOrderForEdit.Statuses = statusService.GetAllStatus();
			suppplierOrderForEdit.Products = (List<ProductsForEditSupplierOrder>)productService.ProductsDetailsPerSupplierOrder(id);
			suppplierOrderForEdit.CustomerOrderNumber = customerOrderService.CustomerOrderNumber(id);

			foreach (var item in suppplierOrderForEdit.Products)
			{
				item.Descriptions = productService.GetDescriptions();
				item.Grades = productService.GetGrades();
				item.Sizes = productService.GetSizes();
				item.Units = productService.GetUnits();
			}

			return View(suppplierOrderForEdit);
		}

		[HttpPost]
		public IActionResult SupplierOrderEdit(SupplierOrderForEditModel model, int id)
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
					Statuses = statusService.GetAllStatus()
				};
			}

			var orderForEdit = reportService.EditSupplierOrder
				(id, model.Number, model.Date, model.MyCompanyId,
				model.DeliveryTerms, model.LoadingAddress, model.DeliveryAddress, model.GrossWeight, model.NetWeight,
				model.CurrencyId, model.StatusId, model.CustomerOrderNumber, model.FSCClaim, model.FSCSertificate, model.PaidAvance,
				model.PaidStatus, model.VAT, model.Products);

			if (orderForEdit == false)
			{
				return BadRequest();
			}

			return RedirectToAction("Index", "Home");
		}
		public IActionResult InvoicePaymentReport(CustomerInvoicePaymentsReportsViewModel model)
		{
			if (model.CustomerName != null)
			{
				string userId = this.User.UserId();
				var userIdMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

				if (!userIdMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}
			if (!ModelState.IsValid) return View();
			//TODO When All are selected page is empty

			var customerNames = customerService.GetCustomerNames();

			var customerPaymentCollection = reportService.CustomersInvoicesPaymentDetails(
				model.CustomerName,
				model.CurrentPage, CustomerInvoicePaymentsReportsViewModel.CustomerInvoicesPerPage);

			model.CustomerPaymentCollection = customerPaymentCollection.CustomerInvoices;
			model.CustomerNames = customerNames;
			model.TotalCustomerInvoices = customerPaymentCollection.TotalInvoices;
			return View(model);
		}
		[HttpGet]
		public IActionResult EditInvoicePayment(int documentNumber)
		{
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
			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var updatedInvoicePayment = invoiceService.EditInvoicePayment
				(documentNumber, model.PaidStatus, model.PaidAvance, model.DatePaidAmount);

			if (updatedInvoicePayment == false)
			{
				return BadRequest();
			}
			return RedirectToAction("Index", "Home");
		}
		public IActionResult PurchasePaymentReport(SupplierInvoicePaymentReportViewModel model)
		{
			if (!ModelState.IsValid) return View();
			if (model.SupplierName != null)
			{
				string userId = this.User.UserId();
				var userIdMyCompany = myCompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

				if (!userIdMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}

			var supplierNames = supplierService.GetSupplierNames();
			model.SupplierNames = supplierNames;

			var supplierPaymentCollection = reportService.SuppliersInvoicesPaymentDetails(
				model.SupplierName,
				model.CurrentPage, SupplierInvoicePaymentReportViewModel.SupplierInvoicePerPage);

			model.SupplierInvoicesPaymentCollection = supplierPaymentCollection.PurchaseInvoices;

			model.TotalSupplierInvoices = supplierPaymentCollection.TotalPurchaseInvoices;

			return View(model);
		}
		[HttpGet]
		[Authorize]
		public IActionResult EditPurchasePayment(string number)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var purchaseForpurchaseDetails = purchaseService.GetPurchaseForPaymentEdit(number);

			return View(purchaseForpurchaseDetails);
		}
		[HttpPost]
		[Authorize]
		public IActionResult EditPurchasePayment(EditPurchasePaymentDetails model, string number)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var updatedPurchasePayment = purchaseService.EditPurchasePayment
				(number, model.PaidStatus, model.PaidAvance, model.DatePaidAmount);


			if (updatedPurchasePayment == false)
			{
				return BadRequest();
			}
			return RedirectToAction("Index", "Home");
		}
		public IActionResult CustomerOrdersPaymentReport(CustomerOrderPaymentReportViewModel model)
		{
			if (model.CustomerName != null)
			{
				string userId = this.User.UserId();
				var userIdMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

				if (!userIdMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}
			if (!ModelState.IsValid) return View();
			//TODO When All are selected page is empty
			var customerNames = customerService.GetCustomerNames();

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
			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var updatedCustomerOrderPayment = customerOrderService.EditCustomerOrdersPayment
				(orderConfirmationNumber, model.PaidStatus, model.PaidAvance);

			if (updatedCustomerOrderPayment == false)
			{
				return BadRequest();
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult SupplierOrdersPaymentReport(SupplierOrdersPaymentReportViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			if (model.SupplierName != null)
			{
				string userId = this.User.UserId();
				var userIdMyCompany = myCompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

				if (!userIdMyCompany.Contains(userId))
				{
					return BadRequest();
				}
			}

			var supplierNames = supplierService.GetSupplierNames();
			model.SupplierNames = supplierNames;

			var supplierOrdersPaymentCollection = supplierOrderService.GetSupplierOrders(model.SupplierName, model.CurrentPage,
				SupplierInvoicePaymentReportViewModel.SupplierInvoicePerPage);

			model.SupplierOrderPaymentCollection = supplierOrdersPaymentCollection.SupplierOrderPaymentCollection;
			model.TotalSupplierOrders = supplierOrdersPaymentCollection.TotalSupplierOrders;

			return View(model);
		}
		[HttpGet]
		[Authorize]
		public IActionResult EditSupplierOrderPayment(string supplierOrderNumber)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var supplierOrder = supplierOrderService.GetSupplierOrderForEdit(supplierOrderNumber);
			return View(supplierOrder);
		}
		[HttpPost]
		[Authorize]
		public IActionResult EditSupplierOrderPayment(string supplierOrderNumber, EditSupplierOrderPaymentModel model)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return BadRequest();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var isSupplierOrderPaymentEdit = supplierOrderService.EditSupplierOrderPayment
				(supplierOrderNumber, model.PaidAvance, model.DatePaidAmount, model.PaidStatus);

			if (!isSupplierOrderPaymentEdit) return BadRequest();

			return View();
		}
		public IActionResult ProductsOnStock(ProductAvailabilityViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (myCompanyService.GetAllCompanies().Count() == 0) return BadRequest();

			var descriptions = productService.DescriptionIdAndNameList();
			var grades = productService.GradeIdAndNameList();
			var sizes = productService.SizeIdAndNameList();

			model.Descriptions = descriptions;
			model.Grades = grades;
			model.Sizes = sizes;

			var productCollection = productService.ProductsOnStock
				(model.DescriptionId, model.GradeId, model.SizeId, model.CurrentPage, ProductAvailabilityViewModel.ProductsPerPage);

			model.ProductsDetails = productCollection;
			model.TotalProducts = productCollection.Count();

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
				(model.MyCompanyName, 
				 model.CurrentPage, 
				 InvoicesViewModel.InvoicesPerPage);

			model.InvoiceCollection = invoiceCollection.InvoiceCollection;
			model.TotalInvoices = invoiceCollection.TotalInvoices;
			
			return View(model);
        }

        [HttpGet]
		public IActionResult EditInvoice(int id)
        {
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}

			var invoiceToEdit = invoiceService.ViewEditInvoice(id);
			return View(invoiceToEdit);
		}

		[HttpPost]
		public IActionResult EditInvoice(int id, EditInvoiceViewModel model)
		{
			//TODO Can i make this global
			string userId = this.User.UserId();
			var mycompaniesUserId = myCompanyService.GetCompaniesUserId();
			if (!mycompaniesUserId.Contains(userId))
			{
				return BadRequest();
			}
			var checkEditableInvoice = invoiceService.EditInvoice
				(id, model.CurrencyExchangeRate, model.Date, model.GrossWeight, model.NetWeight,
				model.DeliveryCost, model.OrderConfirmationNumber, model.TruckNumber, 
				model.CreditToInvoiceNumber,model.DebitToInvoiceNumber, model.Products);

			if(!checkEditableInvoice) return BadRequest();

            return RedirectToAction("InvoiceDetails", new { Id = id });
		}

		public IActionResult InvoiceDetails(int id)
		{
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

			if(!ModelState.IsValid) { return BadRequest(); }

			model.Suppliers = supplierService.GetSupplierNames();

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
			//TODO change model and finish the action
			var purchaseForEdit = purchaseService.PurchaseDetailsForEdit(id);

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
		public  IActionResult EditPurchase(EditPurchaseViewModel model, int id)
		{
            var userId = this.User.UserId();
            var myUserId = myCompanyService.GetCompaniesUserId();

            if (!myUserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }

			var purchaseForEdit = purchaseService.EditPurchaseInvoice(id, model.Number, model.Date, model.SupplierOrderId, model.Vat,
				model.NetWeight, model.GrossWeight, model.TruckNumber, model.Swb, model.PurchaseTransportCost, model.BankExpenses, model.Duty,
				model.CustomsExpenses, model.Factoring, model.FiscalAgentExpenses, model.ProcentComission, model.OtherExpenses);

			if(purchaseForEdit == false) { return BadRequest(); }

            return RedirectToAction("AllPurchases");
		}

        [HttpGet]
        public async Task<IActionResult> ExportInvoiceToPdf()
        {
           InvoiceDetailsViewModel invoice = ClientService.GetClient();
			await Export();
            return View(invoice);
       } 
        public async Task<string> Export()
		{
           System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
           var pdfModel = ClientService.GetClient();
			var filename = "doc"+ pdfModel.DocumentNumber +"_" + DateTime.Now.ToString("ddMMyyyy");
            var stringForPrint = await viewRenderService.RenderToStringAsync("~/Views/Reports/ExportInvoiceToPdf.cshtml", pdfModel);
            Document document = new Document();
			XMLWorkerFontProvider fontProvider= new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
			var path = Directory.GetCurrentDirectory() + filename + ".pdf";
			PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path,FileMode.Create));
			//foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()+"/"+"wwwroot/fonts"))
			//{
			//	FontFactory.FontImp.Register(file);
			//}

			document.Open();
            document.Add(new Chunk(""));

            using (var strReader = new StringReader(stringForPrint))
			{ 
				HtmlPipelineContext htmlcontext = new HtmlPipelineContext(null);
				htmlcontext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
				ICSSResolver cSSResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
				//cSSResolver.AddCssFile(Directory.GetCurrentDirectory() + "/wwwroot/css/site.css", true);
				IPipeline pipeline = new CssResolverPipeline(cSSResolver, new HtmlPipeline(htmlcontext, new PdfWriterPipeline(document,writer)));
				var worker = new XMLWorker(pipeline, true);
				var xmlParse = new XMLParser(true, worker);
				xmlParse.Parse(strReader);
				xmlParse.Flush();
            }
            document.Close();
			return path;
        }

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
        public FileResult ExportPdf2(string exportData)
		{
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader reader = new StringReader(exportData);
                Document PdfFile = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                PdfFile.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
                PdfFile.Close();
                return File(stream.ToArray(), "application/pdf", "exportData.pdf");
            }
        }

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

                worksheet.Cell(9, 1).Value = "Order confirmation No"; 
				worksheet.Cell(9, 2).Value = invoice.OrderConfirmationNumber;

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

				IXLRange range = worksheet.Range(worksheet.Cell(14,1).Address, worksheet.Cell(14,10).Address);
				range.Style.Fill.SetBackgroundColor(XLColor.TurquoiseGreen);

				int row = 15;
				int i = 1;

                foreach (var product in invoice.Products)
				{
					worksheet.Cell(row, 1).Value = i;
                    worksheet.Cell(row, 2).Value = product.Description;
                    worksheet.Cell(row, 3).Value = product.Grade;
                    worksheet.Cell(row, 4).Value = product.Size;
                    worksheet.Cell(row, 5).Value = product.FSCClaim;
                    worksheet.Cell(row, 6).Value = product.FSCSertificate;
                    worksheet.Cell(row, 7).Value = product.Unit;
                    worksheet.Cell(row, 8).Value = product.OrderedQuantity;
                    worksheet.Cell(row, 9).Value = product.Price;
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

	}
}
