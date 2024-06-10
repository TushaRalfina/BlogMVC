using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogMVC.Models;
using BCrypt.Net;
using System.Data.Entity.Validation;


 

namespace BlogMVC.Controllers

{


    /*public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            // Hash the password using BCrypt with a random salt and a work factor of 10
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(10));
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify the password against the hashed password
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }*/

    public class HomeController : Controller
    {

        public HomeController()
        {
        }

 
        BlogEntities db = new BlogEntities();

         public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(user user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                     return View(user);
                }

                if (db.users.Any(x => x.username == user.username))
                {
                    ViewBag.Notification = "This username is already taken";
                    return View(user);
                }
                else
                {
                    //user.created_at = DateTime.Now;
                    var password = BCrypt.Net.BCrypt.HashPassword(user.password, BCrypt.Net.BCrypt.GenerateSalt(10));

                    user.password = password;
                        
                    db.users.Add(user);
                    db.SaveChanges();

                    //save the user in the session
                    Session["id"] = user.id.ToString();
                    Session["username"] = user.username.ToString();
                    Session["role"] = user.role.ToString();

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (DbEntityValidationException ex)
            {
                 foreach (var entityValidationError in ex.EntityValidationErrors)
                {
                     string entityName = entityValidationError.Entry.Entity.GetType().Name;

                     foreach (var validationError in entityValidationError.ValidationErrors)
                    {
                         string propertyName = validationError.PropertyName;
                        string errorMessage = validationError.ErrorMessage;

                         Console.WriteLine($"Validation error for {entityName}.{propertyName}: {errorMessage}");
                    }
                }
            }
            return View();
        }

         public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user user)
        {
            var usr = db.users.Where(u => u.username == user.username && u.password == user.password).FirstOrDefault();
            if (usr != null)
            {
                Session["id"] = usr.id.ToString();
                Session["username"] = usr.username.ToString();
                Session["role"] = usr.role.ToString();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Invalid username or password";
                return View();
            }
        }

        //this controller will be used to display and update the user profile

        [HttpGet]
        public ActionResult UserProfile()
        {

            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                int id = Convert.ToInt32(Session["id"]);
                user user = db.users.Find(id);
                return View(user);
            }   
        }





        public ActionResult Index()
        {
            if (Session["id"] == null)
                {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
         }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact() {
            return View();
        }

        public ActionResult Post() {
            return View();
        }

        public ActionResult Categories() {
            return View();
        }


     

        public ActionResult Add()
        {
            return View();
        }
    }
}