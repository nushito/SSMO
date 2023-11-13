
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.MyCompany;
using SSMO.Services;
using SSMO.Services.MyCompany;
using SSMO.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SSMO.Controllers
{
    public class MyCompanyController : Controller
    {
        
        private readonly ICurrency icurrency;
        private readonly IBankService bankService;
        private readonly IMycompanyService mycompany;
       
   
        public MyCompanyController (ICurrency icurrency, IBankService bankService, IMycompanyService mycompany)
        {           
            this.icurrency = icurrency;
            this.bankService = bankService;
            this.mycompany = mycompany;
        }
        public IActionResult Register()
        {
            return  View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Register(MyCompanyFormModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            string userId = this.User.UserId();//this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isCompanyRegistered = mycompany.RegisterMyCompany(
                model.Name, model.EIK, model.VAT, model.FscSertificate, userId, model.City, 
                model.Street, model.Country, model.RepresentativePerson, model.BgName,
                model.BgCity, model.BgStreet, model.BgCountry, model.RepresentativePerson,
                model.CorrespondCountry, model.CorrespondCity,model.CorrespondStreet,
                model.CorrespondBgCountry, model.CorrespondBgCity, model.CorrespondBgStreet);

            if (!isCompanyRegistered) return View();
           
            return RedirectToAction("Index", "Home"); 
        }
       
        public IActionResult AddBank()       
        {
            return View(new AddBankDetailsFormModel
            { 
                CompanyNames = mycompany.GetCompaniesNames(),
                Currency = this.icurrency.AllCurrency().ToList()
        }); 
        }
              
        [Authorize]
        [HttpPost]
        public IActionResult AddBank(AddBankDetailsFormModel bankmodel)
        {
            if (!ModelState.IsValid)
            {
                bankmodel.Currency = this.icurrency.AllCurrency().ToList();
                bankmodel.CompanyNames = this.mycompany.GetCompaniesNames();
            }
                
            var userCompanyId = mycompany.GetUserIdMyCompanyByName(bankmodel.CompanyName);
            string userId = this.User.UserId();

            if (userCompanyId != userId)
            {
                return BadRequest();
            }

                if (mycompany.GetCompaniesNames() == null)
            {
                return RedirectToAction("Register", "MyCompany");
            }

            bankService.Create(
                bankmodel.CurrencyId,              
                bankmodel.BankName,
                bankmodel.Iban,
                bankmodel.Swift,
                bankmodel.Address,
                bankmodel.CompanyName,
                bankmodel.CompanyId
                );

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
       public IActionResult EditCompany(EditCompanyViewModel model)
        {
            var userId = this.User.UserId();
            var myuserId = mycompany.GetCompaniesUserId();
            if (!myuserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }

            model.MyCompanies = mycompany.GetCompaniesNameAndId();
            model.Company = mycompany.CompanyForEditById(model.Id);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCompany(int id, EditCompanyViewModel model)
        {
            var userId = this.User.UserId();
            var myuserId = mycompany.GetCompaniesUserId();
            if (!myuserId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid) { return BadRequest(); }

            var isEdited = mycompany.EditCompany(id, model.Company.Name, model.Company.BgName, model.Company.EIK, model.Company.VAT,
                model.Company.FscClaim, model.Company.FscSertificate, model.Company.RepresentativePerson, 
                model.Company.BgRepresentativePerson, model.Company.Street, model.Company.BgStreet, 
                model.Company.City, model.Company.BgCity, model.Company.Country, model.Company.BgCountry,
                model.Company.CorrespondCountry, model.Company.CorrespondCity, model.Company.CorrespondStreet,
                model.Company.CorrespondBgCountry, model.Company.CorrespondBgCity, model.Company.CorrespondBgStreet);

            if(!isEdited) { return BadRequest(); }

            return RedirectToAction("Index", "Home");

        }

    }
}
