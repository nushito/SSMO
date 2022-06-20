using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Models.Grades;
using SSMO.Services.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Controllers
{
    public class GradesController : Controller
    {
        private readonly IProductService _productservice;
        public GradesController(IProductService service)
        {
            _productservice = service;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(GradeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_productservice.GradeExist(model.Name) == true)
            {
                return View(model);
            }

            _productservice.AddGrade(model.Name);


            return RedirectToAction("Index", "Home");
        }

    }
}
