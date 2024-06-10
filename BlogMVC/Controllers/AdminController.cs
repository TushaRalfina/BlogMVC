using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Controllers
{
    public class AdminController : Controller
    {


        [HttpGet]
        public ActionResult ManagePosts()
        {
            return View();
        }
         
                    
        public ActionResult List()
        {
            return View();
        }
    }
}