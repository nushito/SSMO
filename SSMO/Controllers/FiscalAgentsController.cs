﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Models.Descriptions;
using SSMO.Models.FiscalAgent;
using SSMO.Services.Documents;

namespace SSMO.Controllers
{
    public class FiscalAgentsController : Controller
    {
        private readonly IDocumentService _documentService;
        public FiscalAgentsController(IDocumentService documentService)
        {
            this._documentService = documentService;
        }
    
        public IActionResult AddFiscalAgent()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddFiscalAgent(AddFiscalAgentFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _documentService.AddFiscalAgent(model.Name, model.BgName, model.Details, model.BgDetails);

            return RedirectToAction("Index", "Home");
        }
    }
}