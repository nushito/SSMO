
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Suppliers;
using SSMO.Services;

namespace SSMO.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICurrency currencyGet;
        public SuppliersController(ApplicationDbContext dbContext, 
            ICurrency currencyGet)
        {
            this.dbContext = dbContext;
            this.currencyGet = currencyGet;
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
        public IActionResult EditSupplier()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditSupplier(string supplierName)
        {
            return View();
        }

    }
 }
  
