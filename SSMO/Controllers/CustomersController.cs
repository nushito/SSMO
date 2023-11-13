
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Infrastructure;
using SSMO.Models;
using SSMO.Models.Customers;
using SSMO.Services.Customer;
using SSMO.Services.MyCompany;

namespace SSMO.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerService customerService;
        public CustomersController(ApplicationDbContext dbContex, IMycompanyService mycompanyService, ICustomerService customerService)
        {
            this.dbContext = dbContex;
            myCompanyService = mycompanyService;
            this.customerService = customerService;
        }

        public IActionResult Add()
        {
            return View();

        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(AddCustomerFormModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            string userId = this.User.UserId();
            var isCustomerCreated = customerService.CreateCustomer
                (model.Name, model.VAT, model.EIK, model.RepresentativePerson, model.Country, 
                model.City, model.Street, model.Email, model.PhoneNumber,
                model.BgCustomerName, model.BgStreet, model.BgCity, model.BgCountry, 
                model.BgRepresentativePerson, userId);

            if(!isCustomerCreated) return View();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Edit(EditCustomerViewModel model)
        {
            string userId = this.User.UserId();
            if (model.CustomerName != null)
            {
                var listMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

                if (!listMyCompany.Contains(userId))
                {
                    return BadRequest();
                }
            }
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var customerNames = customerService.GetCustomerNames(userId);
            model.CustomerNames = customerNames;
            model.CustomerForEdit = customerService.GetCustomerForEdit(model.CustomerName);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(string customerName, EditCustomerViewModel model)
        {
            if (model.CustomerName != null)
            {
                string userId = this.User.UserId();

                var listMyCompany = myCompanyService.MyCompaniesNamePerCustomer(model.CustomerName);

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
                return BadRequest();
            }

            var editCustomer = customerService.EditCustomer
                (customerName, model.CustomerForEdit.VAT, model.CustomerForEdit.EIK, model.CustomerForEdit.RepresentativePerson,
                model.CustomerForEdit.CustomerAddress.Country, model.CustomerForEdit.CustomerAddress.City, 
                model.CustomerForEdit.CustomerAddress.Street, model.CustomerForEdit.Email, model.CustomerForEdit.PhoneNumber,
                model.CustomerForEdit.BgName, model.CustomerForEdit.CustomerAddress.BgStreet, model.CustomerForEdit.CustomerAddress.BgCity,
                model.CustomerForEdit.CustomerAddress.BgCountry, model.CustomerForEdit.BgRepresentativePerson);
           
            if (editCustomer == false)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }



    }
}
