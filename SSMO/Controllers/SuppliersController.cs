
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Infrastructure;
using SSMO.Models.Suppliers;
using SSMO.Services;
using SSMO.Services.MyCompany;

namespace SSMO.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICurrency currencyGet;
        private readonly IMycompanyService mycompanyService;
        private readonly ISupplierService supplierService;
        public SuppliersController(ApplicationDbContext dbContext, 
            ICurrency currencyGet,
            IMycompanyService mycompanyService, ISupplierService supplierService)
        {
            this.dbContext = dbContext;
            this.currencyGet = currencyGet;
            this.mycompanyService = mycompanyService;
            this.supplierService = supplierService;
        }

        [Authorize]
             public IActionResult AddSupplier()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddSupplier(
          AddSupplierModel model
            )
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
            var supplier = new Supplier
            { 
                Name = model.Name,
                VAT = model.VAT,
                Eik = model.Eik,
                Email = model.Email,
                Address = new Address
                {
                    City = model.City,
                    Street = model.SupplierAddress,
                    Country = model.Country
                },
                RepresentativePerson = model.RepresentativePerson,
                FSCSertificate = model.FSCSertificate
            };

            //var bankDetail = new BankDetails
            //{
            //    BankName = model.BankName,
            //    Iban = model.Iban,
            //    Address = model.BankAddress,
            //    Swift = model.Swift,
            //    Currency = new InvoiceAndStockModels.Currency { AccountCurrency = (AccountCurrency)Enum.Parse(typeof(AccountCurrency), model.Currency) }  //(AccountCurrency)Enum.Parse(typeof(AccountCurrency),model.Currency)              
            //};

            //supplier.BankDetails.Add(bankDetail);

            this.dbContext.Suppliers.Add(supplier);
            this.dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditSupplier(EditSupplierViewModel model)
        {
            if (model.SupplierName != null)
            {
                string userId = this.User.UserId();

                var listMyCompany = mycompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

                if (!listMyCompany.Contains(userId))
                {
                    return BadRequest();
                }
            }
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var supplierNames = supplierService.GetSupplierNames();
            model.SupplierNames = supplierNames;
            model.SupplierForEdit = supplierService.GetSupplierForEdit(model.SupplierName); 
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditSupplier(string supplierName, EditSupplierViewModel model)
        {
            if (model.SupplierName != null)
            {
                string userId = this.User.UserId();

                var listMyCompany = mycompanyService.MyCompaniesNamePerSupplier(model.SupplierName);

                if (!listMyCompany.Contains(userId))
                {
                    return BadRequest();
                }
            }
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            var supplierEdit = supplierService.EditSupplier
                (model.SupplierName, model.SupplierForEdit.VAT, model.SupplierForEdit.Eik, model.SupplierForEdit.RepresentativePerson,
                model.SupplierForEdit.SupplierAddress.Country, model.SupplierForEdit.SupplierAddress.City, model.SupplierForEdit.SupplierAddress.Street, 
                model.SupplierForEdit.Email,model.SupplierForEdit.FSCSertificate);

            if (supplierEdit == false)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

    }
 }
  
