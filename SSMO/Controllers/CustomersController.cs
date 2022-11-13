
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

            var customer = new Customer
            {
                ClientAddress = new Address
                {
                    Country = model.Country,
                    City = model.City,
                    Street = model.Street
                },
                Email = model.Email,
                Name = model.Name,
                VAT = model.VAT,
                EIK = model.EIK,
                RepresentativePerson = model.RepresentativePerson,

                // PhoneNumber = model
            };

            this.dbContext.Customers.Add(customer);
            this.dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Edit(EditCustomerViewModel model)
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
            var customerNames = customerService.GetCustomerNames();
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
                model.CustomerForEdit.CustomerAddress.Street, model.CustomerForEdit.Email, model.CustomerForEdit.PhoneNumber);
           
            if (editCustomer == false)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }



    }
}
