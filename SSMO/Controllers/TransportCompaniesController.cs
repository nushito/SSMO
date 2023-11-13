using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data.Models;
using SSMO.Infrastructure;
using SSMO.Models.TransportCompany;
using SSMO.Services.MyCompany;
using SSMO.Services.TransportService;

namespace SSMO.Controllers
{
    public class TransportCompaniesController : Controller
    {
        private readonly IMycompanyService mycompanyService;
        private readonly ITransportService transportService;
       
        public TransportCompaniesController(IMycompanyService mycompanyService, ITransportService transportService)
        {
            this.mycompanyService = mycompanyService;
            this.transportService = transportService;   
        }

        [Authorize]
        public IActionResult Add()
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            return View();
        }
        //entity на нова транспортна фирма
        [HttpPost]
        [Authorize]
        public IActionResult Add(TransportCompanyFormModel model)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            var check = transportService.CreateTransportCompany
                (model.Name, model.Eik, model.Vat, model.PhoneNumber, model.Email, model.City,
                model.Country, model.Address, model.ContactPerson, userId);

            if (!check)
            {
                return BadRequest();
            }

            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult Edit(EditTransportCompanyModel model)
        {
            var userId = this.User.UserId();
            var myuserId = mycompanyService.GetCompaniesUserId();
            if (!myuserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }

            model.TransportCompanies = transportService.TransportCompanies();
            model.TransportCompany = transportService.GetTransportCompanyForEdit(model.Id);

            return View(model);
        }

        //Edit на съществуваща транспортна фирма
        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, EditTransportCompanyModel model)
        {
            var userId = this.User.UserId();
            var myuserId = mycompanyService.GetCompaniesUserId();
            if (!myuserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }

            var companyForEdit = transportService.EditTransportCompany(id,model.TransportCompany.Name,model.TransportCompany.Eik,
                model.TransportCompany.Vat, model.TransportCompany.PhoneNumber,model.TransportCompany.Email,
                model.TransportCompany.City, model.TransportCompany.Country,model.TransportCompany.Address,
                model.TransportCompany.ContactPerson);

            if(companyForEdit == false)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
