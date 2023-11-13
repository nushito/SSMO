using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Services.Images;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace SSMO.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageService imageService;
        public ImagesController(IImageService imageService)
        {
            this.imageService = imageService;
        }


        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IEnumerable<IFormFile> image)
        {
            foreach (var file in image)
            {
               await imageService.NewImage(file);
            }
           
            return View("Index");
        }

       // [HttpPost]
        //public IActionResult RetrieveImage()
        //{
        //    Image img = dbContext.Images.OrderByDescending
        //(i => i.Id).SingleOrDefault();
        //    string imageBase64Data = Convert.ToBase64String(img.ImageData);
        //    string imageDataURL =
        //string.Format("data:image/jpg;base64,{0}",
        //imageBase64Data);
        //    ViewBag.ImageTitle = img.ImageTitle;
        //    ViewBag.ImageDataUrl = imageDataURL;
        //    return View("Index");
        //}}
    }
}
