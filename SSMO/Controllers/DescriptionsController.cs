﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data.Models;
using SSMO.Models.Descriptions;
using SSMO.Services.Products;

namespace SSMO.Controllers
{
    public class DescriptionsController : Controller
    {
        private readonly IProductService _productservice;
        public DescriptionsController(IProductService productservice)
        {
            _productservice = productservice;

        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(DescriptionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_productservice.DescriptionExist(model.Name) == true)
            {
                return View(model);
            }

            _productservice.AddDescription(model.Name);
       

            return RedirectToAction("Index","Home"); 
         }

       public IActionResult AddSize()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddSize(DescriptionsViewModel model)
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

        public IActionResult AddGrade()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddGrade(DescriptionsViewModel model)
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
