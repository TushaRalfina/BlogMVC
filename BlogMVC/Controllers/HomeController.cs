 /**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: HomeController
* Arsyeja: Kjo klase menaxhon funksionalitetet e login & regjistrim te perdoruesve dhe faqet kryesore te aplikacionit.
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

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: SignUp (GET)
         * Pershkrimi: Kthen nje View qe permban formen per regjistrimin e nje perdoruesi te ri.
         * Return: ActionResult - VIEW me formen e regjistrimit.
         **/

        public ActionResult SignUp()
        {
            return View();
        }

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: SignUp (POST)
         * Arsyeja: Ruajtja e te dhenave te nje perdoruesi te ri.
         * Pershkrimi: Kontrollon vlefshmerine e modelit. Verifikon nese username i perdoruesit eshte i zene dhe nese emaili eshte valid. Gjeneron kodin e verifikimit dhe dergon emailin e verifikimit.
         * Para kushti: Modeli duhet te jete valid.
         * Post kushti: Ruhet perdoruesi i perkohshem ne sesion dhe dergohet emaili i verifikimit.
         * Parametrat: userViewModel - Modeli i perdoruesit per regjistrim.
         * Return: ActionResult - Ridrejtohet ne faqen e verifikimit te emailit ose rikthen pamjen e formes me gabimet perkatese.
         **/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(UserViewModel userViewModel)
        {
             
                if (!ModelState.IsValid)
                {
                    return View(userViewModel);
                }

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

                 userViewModel.VerificationCode = GenerateVerificationCode();

                 SendVerificationEmail(userViewModel.email, userViewModel.VerificationCode);

                 Session["TempUser"] = userViewModel;

                return RedirectToAction("VerifyEmail");
            
            
        }

        /**
         * Data: 26/06/2024
         * Programuesi:Ralfina Tusha
         * Metoda: VerifyEmailWithAPI
         * Arsyeja: Verifikimi i vlefshmerise se nje adrese emaili duke perdorur API (EMAILLISTVERIFY).
         * Pershkrimi: Ben nje kerkese HTTP GET ne nje API e per te verifikuar vlefshmerine e emailit.
         * Para kushti: Nuk ka.
         * Post kushti: Kthen nje rezultat boolean qe tregon nese emaili eshte valid( api kthen statusin si ok ose fail) .
         * Parametrat: email - Adresa e emailit per t'u verifikuar.
         * Return: Task<bool> - Rezultati i vlefshmerise se emailit.
         **/

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
                    Console.WriteLine("API Response: " + responseText);  
  
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



        /**
             * Data: 26/06/2024
             * Programuesi:Ralifna Tusha
             * Metoda: VerifyEmail (GET)
             * Pershkrimi: Kthen nje View qe permban formen per verifikimin e emailit.
             * Return: ActionResult - VIEW me formen e verifikimit te emailit.
             **/

        public ActionResult VerifyEmail()
        {
            return View();
        }

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: VerifyEmail (POST)
         * Arsyeja: Validimi i kodit te verifikimit te derguar me email.
         * Pershkrimi: Kontrollon vlefshmerine e kodit te verifikimit. Nese kodi eshte i sakte, shton perdoruesin e ri ne bazen e te dhenave dhe e logon ate.
         * Para kushti: Modeli duhet te jete valid dhe kodi i verifikimit duhet te jete i sakte.
         * Post kushti: Perdoruesi i ri shtohet ne bazen e te dhenave dhe e logon ate.
         * Parametrat: verificationCode - Kodi i verifikimit i vendosur nga perdoruesi.
         * Return: ActionResult - Ridrejtohet ne faqen kryesore ose rikthen pamjen e formes me mesazhin "Invalid verification code" .
         **/


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
        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: SendVerificationEmail
         * Arsyeja: Dergimi i nje emaili verifikimi me kodin e verifikimit.
         * Para kushti: Nuk ka.
         * Post kushti: Emaili i verifikimit dergohet.
         * Parametrat: email - Adresa e emailit te marresit; verificationCode - Kodi i verifikimit.
         * Return: void
         **/

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
        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: GenerateVerificationCode
         * Pershkrimi: Gjeneron nje kod verifikimi te rastesishem me 6 shifra.
         * Return: string - Kodi i verifikimit.
         **/


        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: Logout
          * Pershkrimi: Fshin te gjitha te dhenat e sesionit dhe ridrejton perdoruesin ne faqen kryesore.
         * Para kushti: Perdoruesi duhet te jete i loguar.
         * Post kushti: Perdoruesi del nga accounti dhe ridrejtohet ne faqen kryesore.
         * Parametrat: Nuk ka.
         * Return: ActionResult - Ridrejtim ne faqen kryesore.
         **/


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: Login (GET)
          * Pershkrimi: Kthen nje View qe permban formen per logimin e perdoruesit.
         * Return: ActionResult - VIEW me formen e logimit.
         **/
        public ActionResult Login()
        {
            return View("SignUp");
        }

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: Login (POST)
         * Arsyeja: Autentifikimi i userit.
         * Pershkrimi: Verifikon kredencialet e perdoruesit dhe e logon ate nese kredencialet jane te sakta.
         * Para kushti: Modeli duhet te jete valid.
         * Post kushti: Perdoruesi logon dhe ridrejtohet ne faqen kryesore ose rikthen pamjen e formes me nje mesazh qe te dhenat nuk jane te sakta.
         * Parametrat: user - Modeli i perdoruesit qe permban kredencialet.
         * Return: ActionResult - Ridrejtim ne faqen kryesore .
         **/

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
        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: Index
         * Arsyeja: Shfaq te gjitha postimet e miratuara te blogut, me mundesi filtrimi.
         * Pershkrimi: Merr dhe shfaq te gjitha postimet e miratuara te blogut. Mund te filtroje sipas kategorive, datave dhe mund te rendite postimet sipas dates se postuar.
         * Para kushti: Perdoruesi duhet te jete i loguar.
         * Post kushti: Kthen nje View me listen e postimeve te miratuara.
         * Parametrat: page - Numri i faqes per paginim; category - Filtrimi sipas kategorise; sortBy - Renditja e postimeve; fromDate - Filtrimi sipas dates fillimit; toDate - Filtrimi sipas dates mbarimit.
         * Return: ActionResult - VIEW me listen e postimeve te miratuara.
         **/
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


        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: Profile
         * Arsyeja: Shfaq profilin e perdoruesit te loguar.
         * Pershkrimi: Merr dhe shfaq te dhenat e profilit te perdoruesit te loguar.
         * Para kushti: Perdoruesi duhet te jete i loguar.
         * Post kushti: Kthen nje View me te dhenat e profilit te perdoruesit te loguar.
          * Return: ActionResult - VIEW me te dhenat e profilit te perdoruesit te loguar.
         **/
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
        /**
         * Data: 26/06/2024
         * Programuesi:Ralfina Tusha
         * Metoda: DownloadFile
         * Arsyeja: Shkarkimi i fileve qe ndodhen te bashkangjituara postit.
          * Para kushti: Skedari duhet te ekzistoje ne bazen e te dhenave.
         * Post kushti: Skedari shkarkohet nga perdoruesi.
         * Parametrat: fileId - ID e skedarit per t'u shkarkuar.
         * Return: ActionResult - FileResult qe permban skedarin per t'u shkarkuar ose HttpNotFound nese skedari nuk ekziston.
         **/
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


        /**
            * Data: 26/06/2024
            * Programuesi: Ralfina Tusha
            * Metoda: EditProfile (GET)
            * Arsyeja: Shfaq view per editimin e profilit te perdoruesit.
            * Para kushti: Perdoruesi duhet te jete i loguar.
            * Post kushti: Kthen nje View me formen per editimin e profilit te perdoruesit.
            * Parametrat: id - ID e perdoruesit.
            * Return: ActionResult - VIEW me formen per editimin e profilit te perdoruesit.
            **/

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
        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: EditProfile (POST)
         * Arsyeja: Ruajtja e te dhenave te edituara te profilit te perdoruesit.
          * Para kushti: Modeli duhet te jete valid.
         * Post kushti: Te dhenat e profilit te perdoruesit perditesohen ne db dhe ridrejtohet ne profilin e perdoruesit.
         * Parametrat: editProfileRequest - Modeli qe permban te dhenat qe mund te editohen.
         * Return: ActionResult - Ridrejtim ne profilin e perdoruesit ose rikthen pamjen e formes me nje mesazh gabimi.
         **/

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

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: About
         * Arsyeja: Shfaq faqen "About".
          * Para kushti: Perdoruesi duhet te jete i loguar.
          * Return: ActionResult - VIEW me informacionin "Rreth Nesh".
         **/
        public ActionResult About()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: Categories
        * Arsyeja: Shfaq postimet e blogut sipas kategorise.
        * Pershkrimi: Merr dhe shfaq postimet e blogut sipas kategorise dhe subkategorive te tyre.
        * Para kushti: Perdoruesi duhet te jete i loguar.
        * Post kushti: Kthen nje View me listen e postimeve sipas kategorise te caktuar.
        * Parametrat: category_id - ID e kategorise per te shfaqur postimet.
        * Return: ActionResult - VIEW me listen e postimeve sipas kategorise.
        **/

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

        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: CategoriesPartial
         * Arsyeja: Kthen kategorite si PartialView per ti perfshire ne layout.
         * Return: ActionResult - PartialView me listen e kategorive.
         **/

        [ChildActionOnly]
        public ActionResult CategoriesPartial()
        {
             var categories = categoryRepository.GetCategories();

            return PartialView("_Categories", categories);
        }


    }
}
