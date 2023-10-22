using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Infrastructure;
using SSMO.Models.TransportCompany;
using SSMO.Services.MyCompany;

namespace SSMO.Controllers
{
    public class TransportCompaniesController : Controller
    {
        private readonly IMycompanyService mycompanyService;

        public TransportCompaniesController(IMycompanyService mycompanyService)
        {
            this.mycompanyService = mycompanyService;
        }

        [Authorize]
        public IActionResult Add()
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(TransportCompanyFormModel model)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = mycompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            return View();
        }
    }
}
