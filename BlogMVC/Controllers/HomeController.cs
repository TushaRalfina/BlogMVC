 /**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: HomeController
* Arsyeja: Kjo klasë menaxhon funksionalitetet e login & regjistrim te perdoruesve dhe faqet kryesore te aplikacionit.
* * Pershkrimi: Controlleri per  login & regjistrim te perdoruesve, shfaqjen e faqes kryesore,
* shfaqjen e profilit te perdoruesit, editimin e profilit 
* shkarkimin e fileve,kategorite me postimet perkatese, shfaqjen e faqes about dhe logout
 * Trashegon nga: Controller
* Interfaces: Nuk ka
* Constants: Nuk ka
* Metodat:  SignUp, VerifyEmail, VerifyEmailWithAPI, SendVerificationEmail, GenerateVerificationCode, Login, Index, Profile, DownloadFile, EditProfile, About, Categories, CategoriesPartial
*/


using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using BlogMVC.Interfaces;
using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using BlogMVC.Repositories;
using PagedList;
using System.Web.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;



namespace BlogMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IFilesRepository filesRepository;
        private readonly IAdminRepository adminRepository;
        private readonly BlogEntities db = new BlogEntities();


        public HomeController()
        {
            userRepository = new UserRepository();
            blogPostRepository = new BlogPostRepository();
            categoryRepository = new CategoryRepository();
            filesRepository = new FilesRepository();
            adminRepository = new AdminRepository();
        }
 
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(UserViewModel userViewModel)
        {
             
                if (!ModelState.IsValid)
                {
                    return View(userViewModel);
                }

                // Check if username is taken
                if (userRepository.GetUserByUsername(userViewModel.username) != null)
                {
                    ViewBag.SignupNotification = "This username is already taken";
                    return View(userViewModel);
                }

                 bool isEmailValid = await VerifyEmailWithAPI(userViewModel.email);

                if (!isEmailValid)
                {
                    ViewBag.SignupNotification = "Invalid email address";
                    return View(userViewModel);
                }

                // Generate verification code
                userViewModel.VerificationCode = GenerateVerificationCode();

                // Send verification email
                SendVerificationEmail(userViewModel.email, userViewModel.VerificationCode);

                // Store user in session temporarily (not recommended for production)
                Session["TempUser"] = userViewModel;

                return RedirectToAction("VerifyEmail");
            
            
        }

        private async Task<bool> VerifyEmailWithAPI(string email)
        {
            try
            {
                string apiKey = "jhIvFUjmlgX3E8LdaQFHX";
                string apiUrl = $"https://apps.emaillistverify.com/api/verifyEmail?secret={apiKey}&email={email}";

                WebRequest request = WebRequest.Create(apiUrl);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string responseText = reader.ReadToEnd();
                    Console.WriteLine("API Response: " + responseText); // Debug output
  
                    if (responseText=="ok")
                    {
                        return true; 
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error verifying email: " + ex.Message);
                return false; 
            }
        }





        public ActionResult VerifyEmail()
        {
            return View();
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyEmail(string verificationCode)
        {
            var userViewModel = (UserViewModel)Session["TempUser"];

            if (userViewModel != null && userViewModel.VerificationCode == verificationCode)
            {
                userRepository.AddUser(userViewModel);

                var user = userRepository.GetUserByUsername(userViewModel.username);

                Session["id"] = user.id.ToString();
                Session["username"] = user.username;
                Session["role"] = user.role;

                Session.Remove("TempUser");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Notification = "Invalid verification code";
            return View();
        }

        private void SendVerificationEmail(string email, string verificationCode)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                string emailUsername = WebConfigurationManager.AppSettings["EmailUsername"];
                string emailPassword = WebConfigurationManager.AppSettings["EmailPassword"];

                mail.From = new MailAddress(emailUsername);
                mail.To.Add(email);
                 
                    mail.Subject = "Verification Code";
                    mail.Body = $"Your verification code is: {verificationCode}";
                
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(emailUsername, emailPassword);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }

        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
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
            var usr = userRepository.GetUserByUsername(user.username);
            user.password = userRepository.HashPassword(user.password);
            if (usr != null && usr.password == user.password)
            {
                Session["id"] = usr.id.ToString();
                Session["username"] = usr.username;
                Session["role"] = usr.role;
                Session["password"] = usr.password;
                Session["email"] = usr.email;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.LoginNotification = "Invalid username or password";
                return View("SignUp");
            }
        }
        public ActionResult Index(int? page, string category, string sortBy, DateTime? fromDate, DateTime? toDate)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

             var posts = blogPostRepository.GetBlogPostsApproved();

             if (!string.IsNullOrEmpty(category) && category != "All")
            {
                posts = blogPostRepository.GetBlogPostsByCategory(category);
            }

             if (fromDate != null && toDate != null)
            {
                posts = blogPostRepository.GetBlogPostsByDate(fromDate, toDate);
            }

             switch (sortBy)
            {
                case "date_asc":
                    posts = posts.OrderBy(p => p.created_at);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(p => p.created_at);
                    break;
                default:
                    posts = posts.OrderByDescending(p => p.created_at);  
                    break;
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);

             var categories = db.categories
                .Where(c => c.parent_id == null)
                .OrderBy(c => c.name)
                .Select(c => new SelectListItem
                {
                    Value = c.name,
                    Text = c.name
                })
                .ToList();

            categories.Insert(0, new SelectListItem { Value = "All", Text = "All Categories" });

            ViewBag.Category = category;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.SortBy = sortBy;
            ViewBag.Categories = categories;

            return View(posts.ToPagedList(pageNumber, pageSize));
        }



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

                if (user == null)
                {
                    return HttpNotFound("User not found.");
                }
                var userPosts = userRepository.GetPostsByUserId(id);


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
                    updated_at = user.updated_at,
                    posts = userPosts.ToList()
                };

                return View(userProfileViewModel);
            }
        }

        public ActionResult DownloadFile(int fileId)
        {
            var file = filesRepository.GetFileById(fileId);

            if (file != null)
            {
                byte[] fileBytes = Convert.FromBase64String(file.file_content); // Convert base64 string back to byte array
                return File(fileBytes, "application/octet-stream", file.file_name); // Return file as download
            }

            return HttpNotFound();
        }

         public ActionResult EditProfile(int id)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var user = userRepository.GetUserById(id);
                var usermodel = new EditProfileRequest
                {
                    id = user.id,
                    email = user.email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    bio = user.bio
                };

                return View(usermodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileRequest editProfileRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(editProfileRequest);
            }
            var user = new user
            {
                id = editProfileRequest.id,
                password = Session["password"].ToString(),
                username = Session["username"].ToString(),
                role = Session["role"].ToString(),
                FirstName = editProfileRequest.FirstName,
                LastName = editProfileRequest.LastName,
                email = editProfileRequest.email,
                bio = editProfileRequest.bio
            };

            try
            {
                var updatedUser = userRepository.UpdateUser(user);
                if (updatedUser == null)
                {
                    ViewBag.Notification = "An error occurred while updating the profile";
                    return View(editProfileRequest);
                }

                return RedirectToAction("Profile", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Notification = "An error occurred while updating the profile: " + ex.Message;
                return View(editProfileRequest);
            }
        }
        public ActionResult About()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

     

        public ActionResult Categories(int category_id)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var category = categoryRepository.GetCategoryById(category_id);
            var posts = categoryRepository.GetBlogPostsByCategoryId(category_id).ToList();

            if (category == null)
            {
                return HttpNotFound("Category not found.");
            }

            var viewModel = new CategoryViewModel
            {
                category = category,
                posts = posts,
                subcategories = categoryRepository.GetSubcategoriesByCategoryId(category_id),
                categories = categoryRepository.GetCategories()
            };

            return View(viewModel);
        }

        [ChildActionOnly]
        public ActionResult CategoriesPartial()
        {
             var categories = categoryRepository.GetCategories();

            return PartialView("_Categories", categories);
        }


    }
}
