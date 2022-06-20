using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Models.Sizes;
using SSMO.Services.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Controllers
{
    public class SizesController : Controller
    {
        private readonly IProductService _productservice;

        public SizesController(IProductService service)
        {
            _productservice = service;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(SizeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_productservice.SizeExist(model.Name) == true)
            {
                return View(model);
            }

            _productservice.AddSize(model.Name);


            return RedirectToAction("Index", "Home");
        }
    }
}
