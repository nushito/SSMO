using Microsoft.AspNetCore.Mvc;
using SSMO.Models.FscTexts;
using SSMO.Services.FscTextDocuments;

namespace SSMO.Controllers
{
    public class FscTextsController : Controller
    {
        private readonly IFscTextService fscTextService;

        public FscTextsController(IFscTextService fscTextService)
        {
            this.fscTextService = fscTextService;
        }

        public IActionResult AddFscText()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddFscText(FscTextFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            fscTextService.AddFscText(model.FscTextEng, model.FscTextBg);

            return RedirectToAction("Index","Home");
        }
    }
}
