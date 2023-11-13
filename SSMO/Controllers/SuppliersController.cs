
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

        [HttpGet]
             public IActionResult AddSupplier()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddSupplier(AddSupplierModel model)
        {
            string userId = this.User.UserId();
            if (model.Name != null)
            {                
                var listMyCompany = mycompanyService.GetCompaniesUserId();

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
            
            var supplier = supplierService.AddNewSupplier
                (model.Name, model.VAT, model.Eik, model.Email, model.City, model.SupplierAddress, 
                model.Country, model.RepresentativePerson, model.FSCSertificate, userId);
           
            if (supplier == false) return View(model);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditSupplier(EditSupplierViewModel model)
        {
            string userId = this.User.UserId();
            if (model.SupplierName != null)
            {    
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

            var supplierNames = supplierService.GetSupplierNames(userId);
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
  
