using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index", (object)"Index1");
        }
        public ActionResult Index2()
        {
            return View("Index", (object)"Index2");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);
                file.SaveAs(path);
            }

            return RedirectToAction("Download", new {fileName = file.FileName, contentType = file.ContentType});
        }

        public ActionResult Download(string fileName, string contentType)
        {
            return File(Path.Combine(Server.MapPath("~/App_Data/"), fileName), contentType);
        }


    }
}
