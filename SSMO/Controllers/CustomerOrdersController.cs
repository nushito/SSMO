using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.Products;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.MyCompany;
using SSMO.Services.Product;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SSMO.Data;
using SSMO.Services.CustomerOrderService;

namespace SSMO.Controllers
{
    public class CustomerOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly ICurrency currency;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerService customerService;
        private readonly IProductService productService;
        private readonly ICustomerOrderService cusomerOrderService;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;
        public CustomerOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService, IMapper  mapper,
 ApplicationDbContext dbContext, ICustomerOrderService cusomerOrderService)
        {
            this.supplierService = supplierService;
            this.currency = currency;
            this.myCompanyService = myCompanyService;
            this.customerService = customerService;
            this.productService = productService;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.cusomerOrderService = cusomerOrderService;
            
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddCustomerOrder()
        {


            return View(
                new CustomerOrderViewModel
                {
                    Currencies = currency.AllCurrency(),
                    Customers = customerService.CustomersData(),
                    MyCompanies =myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductViewModel>(),
                    
                 }

                ) ;
        }



        [HttpPost]
        [Authorize]
        public IActionResult AddCustomerOrder(CustomerOrderViewModel customermodel)

        {
            if (!ModelState.IsValid)
            {
                new CustomerOrderViewModel
                {
                    Id = customermodel.Id,  
                    Currencies = currency.AllCurrency(),
                    Customers = customerService.CustomersData(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductViewModel>(),
                    
                };

                new ProductViewModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()
                };
            }

         

            var customerorderId = cusomerOrderService.CreateOrder
                (customermodel.Number, 
                customermodel.Date,
                customermodel.ClientId,
                customermodel.MyCompanyId,
                customermodel.DeliveryTerms,
                customermodel.LoadingPlace,
                customermodel.DeliveryAddress,
                customermodel.CurrencyId);

           
            return RedirectToAction("AddOrderProducts", new { CustomerOrderId = customerorderId });
        }


        [Authorize]
        public IActionResult AddPurchaseProducts()
        {
            return View(
                 new ProductViewModel
                 {
                     Descriptions = productService.GetDescriptions(),
                     Grades = productService.GetGrades(),
                     Sizes = productService.GetSizes(),
                     

                 });
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddPurchaseProducts(int purchaseId,
           List<ProductViewModel> model)
        {


            if (!ModelState.IsValid)
            {
                new ProductViewModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()

                };
            }


         
            var thisPurchase = dbContext.CustomerOrders.Find(purchaseId);
         

            //for (int i = 0; i < model.ProductSpecificationFormModels.Count; i++)
            //{


            //    var productDescription = Request.Form["Description[" + i + "]"];
            //    var size = Request.Form["Size[" + i + "]"];
            //    var grade = Request.Form["Grade[" + i + "]"];
            //    var pieces = Request.Form["Pieces[" + i + "]"];
            //    var cubic = Request.Form["Cubic[" + i + "]"];
            //    var purchasePrice = Request.Form["Price[" + i + "]"];
            //    var transportCost = Request.Form["TransportCost[" + i + "]"];
            //    var terminalCharges = Request.Form["TerminalCharges[" + i + "]"];
            //    var duty = Request.Form["Duty[" + i + "]"];
            //    var customsExpenses = Request.Form["CustomsExpenses[" + i + "]"];
            //    var bankExpenses = Request.Form["BankExpenses[" + i + "]"];

            //    if ((productDescription.ToString() != null) && (size.ToString() != null)
            //        && (grade.ToString() != null) && (pieces != 0) && (cubic != 0) &&
            //        (purchasePrice != 0) && (transportCost != 0) && (terminalCharges != 0) && (duty != 0) &&
            //        (customsExpenses != 0) && (bankExpenses != 0))
            //    {
            //        var product = this.dbContext.Products
            //            .Where(a => a.Description.ToLower() == productDescription.ToString().ToLower()
            //        && a.Size.ToLower() == size.ToString().ToLower()
            //        && a.Grade.ToLower() == grade.ToString().ToLower()
            //       )
            //            .FirstOrDefault();

            //        var productDetails = new ProductSpecification
            //        {

            //            BankExpenses = Math.Round(decimal.Parse(bankExpenses.ToString()), 4),
            //            Cubic = Math.Round(decimal.Parse(cubic.ToString()), 4),
            //            CustomsExpenses = Math.Round(decimal.Parse(customsExpenses.ToString()), 4),
            //            Duty = Math.Round(decimal.Parse(duty.ToString()), 4),
            //            Pieces = int.Parse(pieces.ToString()),
            //            Price = Math.Round(decimal.Parse(purchasePrice.ToString()), 4),
            //            TerminalCharges = Math.Round(decimal.Parse(terminalCharges.ToString()), 4),
            //            TransportCost = Math.Round(decimal.Parse(transportCost.ToString()), 4),

            //        };

            //        var costPrice = (
            //           decimal.Parse(bankExpenses.ToString()) +
            //           decimal.Parse(customsExpenses.ToString()) +
            //           decimal.Parse(duty.ToString()) +
            //           decimal.Parse(terminalCharges.ToString()) +
            //           decimal.Parse(transportCost.ToString()) +
            //           decimal.Parse(cubic.ToString()) * decimal.Parse(purchasePrice.ToString())
            //           ) / decimal.Parse(cubic.ToString());

            //        productDetails.CostPrice = costPrice;

            //        //  var thisSupplier = dbContext.Suppliers.Find(model.SupplierId);
            //        //if (product.Supplier.Name != (model.SupplierName))
            //        //{

            //        //    product.Supplier = (thisSupplier);//new Supplier { Id = model.SupplierId, Name = model.SupplierName });
            //        //}

            //        product.ProductSpecifications.Add(productDetails);
            //        var x = new ProductPurchase
            //        {
            //            ProductId = product.Id,
            //            PurchaseId = purchaseId

            //        };

            //        listProducts.Add(x);
            //        dbContext.SaveChanges();
            //    }
            //}



            //thisPurchase.Products = listProducts;
            //dbContext.SaveChanges();

            return RedirectToAction("Home");
        }

    }
}
