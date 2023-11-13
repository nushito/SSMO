//using System.IO;
//using System;
//using System.Web.Mvc;
//using Syncfusion.HtmlConverter;
//using Syncfusion.Pdf;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Web.Helpers;
//using Microsoft.AspNetCore.Http;

//namespace SSMO.Controllers
//{
    
//    public class ExportsController : Controller
//    {
//        private readonly IHttpContextAccessor httpContextAccessor;
//        public ExportsController(IHttpContextAccessor httpContextAccessor)
//        {
//            this.httpContextAccessor = httpContextAccessor;
//        }

//        public ActionResult Pdf()
//        {
//            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, "PrintCustomerOrder", "");
//            string html = GetHtmlFromView(ControllerContext, viewResult, "PrintCustomerOrder", "");
//            string baseUrl = string.Empty;

//            //Convert the HTML string to PDF using WebKit
//            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();

//            //Convert HTML string to PDF
//            var document = htmlConverter.Convert(html, baseUrl);

//            MemoryStream stream = new MemoryStream();

//            //Save and close the PDF document 
//            document.Save(stream);
//            document.Close(true);

//            return File(stream.ToArray(), "application/pdf", "ViewAsPdf.pdf");
//        }

//        private string GetHtmlFromView
//            (ControllerContext controllerContext, ViewEngineResult viewResult, string viewName, object model)
//        {
//            controllerContext.Controller.ViewData.Model = model;
//            using (StringWriter sw = new StringWriter())
//            {
//                // view not found, throw an exception with searched locations
//                if (viewResult.View == null)
//                {
//                    var locations = new StringBuilder();
//                    locations.AppendLine();

//                    foreach (string location in viewResult.SearchedLocations)
//                    {
//                        locations.AppendLine(location);
//                    }
//                    throw new InvalidOperationException(
//                    string.Format(
//                    "The view '{0}' or its master was not found, searched locations: {1}", viewName, locations));
//                }
//                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
//                viewResult.View.Render(viewContext, sw);

//                string html = sw.GetStringBuilder().ToString();     
                
//                string baseUrl = string.Format("{0}://{1}", httpContextAccessor.HttpContext.Request.Scheme, httpContextAccessor.HttpContext.Request.Host);
//                html = Regex.Replace(html, "<head>", string.Format("<head><base href=\"{0}\" />", baseUrl), RegexOptions.IgnoreCase);
//                return html;
//            }
//        }
//    }
//}
