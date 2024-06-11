using System;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using BlogMVC.Repositories;



namespace BlogMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;

        public HomeController( )
        {
            userRepository = new UserRepository();
         }
       

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(user user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(user);
                }

                if (userRepository.GetUserByUsername(user.username,user.password) != null)
                {
                    ViewBag.Notification = "This username is already taken";
                    return View(user);
                }
                else
                {
                    userRepository.AddUser(user);

                    Session["id"] = user.id.ToString();
                    Session["username"] = user.username;
                    Session["role"] = user.role;

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

                ModelState.AddModelError("", "An error occurred while creating the account.");
                return View(user);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            return View("SignUp");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user user)
        {
            var usr = userRepository.GetUserByUsername(user.username,user.password);
            if (usr != null && usr.password == user.password)
            {
                Session["id"] = usr.id.ToString();
                Session["username"] = usr.username;
                Session["role"] = usr.role;
                Session["password"] = usr.password;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Invalid username or password";
                return View("SignUp");
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


      //GET:   PROFILE

        public ActionResult Profile()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                int id = Convert.ToInt32(Session["id"]);
                var user = userRepository.GetUserById(id);
                var userProfileViewModel = new UserProfileViewModel
                {
                    id = user.id,
                    username = user.username,
                    email = user.email,
                    role = user.role,
                    profile_picture = user.profile_picture,
                    bio = user.bio,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    created_at = user.created_at,
                    updated_at = user.updated_at
                };

                return View(userProfileViewModel);
            }
        }
        //GET: EDIT PROFILE

        public ActionResult EditProfile()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                int id = Convert.ToInt32(Session["id"]);
                var user = userRepository.GetUserById(id);
                var userProfileViewModel = new UserProfileViewModel
                {
                    id = user.id,
                    username = user.username,
                    email = user.email,
                    role = user.role,
                    profile_picture = user.profile_picture,
                    bio = user.bio,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    created_at = user.created_at,
                    updated_at = user.updated_at
                };

                return View(userProfileViewModel);
            }
        }

        //POST: EDIT PROFILE

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserProfileViewModel userProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userProfileViewModel);
            }

            var user = new user
            {
                id = userProfileViewModel.id,
                username = userProfileViewModel.username,
                email = userProfileViewModel.email,
                role = userProfileViewModel.role,
                profile_picture = userProfileViewModel.profile_picture,
                bio = userProfileViewModel.bio,
                FirstName = userProfileViewModel.FirstName,
                LastName = userProfileViewModel.LastName,
                created_at = userProfileViewModel.created_at,
                updated_at = userProfileViewModel.updated_at
            };

            userRepository.UpdateUser(user);

            return RedirectToAction("Index", "Home");
        }
     



        
        

        




        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Post()
        {
            return View();
        }

        public ActionResult Categories()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }
    }
}

