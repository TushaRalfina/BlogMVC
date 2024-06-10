using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Controllers
{
    public class BlogPostController : Controller
    {

        [HttpGet]
        public ActionResult Add()
        {
             if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
    }
}