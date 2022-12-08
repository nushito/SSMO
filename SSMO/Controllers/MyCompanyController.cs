﻿
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

namespace SSMO.Controllers
{
    public class MyCompanyController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICurrency icurrency;
        private readonly IBankService bankService;
        private readonly IMycompanyService mycompany;
   
        public MyCompanyController (ApplicationDbContext dbContext
            ,ICurrency icurrency, IBankService bankService, IMycompanyService mycompany)
        {
            this.dbContext = dbContext;
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
                model.Name, model.EIK, model.VAT, model.FSCSertificate, userId, model.City, model.Street, model.Country, model.RepresentativePerson, model.BgName,
                model.BgCity, model.BgStreet, model.BgCountry, model.RepresentativePerson);

            if (!isCompanyRegistered) return View();
           
            return RedirectToAction("Index", "Home"); 
        }
       
        public IActionResult AddBank()       
        {
            return View(new AddBankDetailsFormModel
            { 
                CompanyNames = mycompany.GetCompaniesNames(),
                Currency = icurrency.GetCurrency()
            }); 
        }
              
        [Authorize]
        [HttpPost]
        public IActionResult AddBank(AddBankDetailsFormModel bankmodel)
        {
            if (!ModelState.IsValid)
            {
                bankmodel.Currency = this.icurrency.GetCurrency().ToList();
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
                bankmodel.CurrencyName,
                bankmodel.BankName,
                bankmodel.Iban,
                bankmodel.Swift,
                bankmodel.Address,
                bankmodel.CompanyName,
                bankmodel.CompanyId
                );

            return RedirectToAction("Index", "Home");
        }

       //TODO Edit Mycompany
       

    }
}
