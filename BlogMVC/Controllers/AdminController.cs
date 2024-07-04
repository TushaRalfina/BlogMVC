/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: AdminController
* Arsyeja: Kjo klasë menaxhon funksionalitetet e adminit për postimet, komentet, kategoritë dhe subkategoritë.
* Pershkrimi: Implementon metodat për menaxhimin e përmbajtjes së blogut nga admini.
* Trashegon nga: Controller
* Interfaces: Nuk ka
* Constants: Nuk ka
* Metodat: ManagePosts,ApprovePost, SendApprovalEmail, DeletePost, ManageComments, ApproveComment, DeleteComment, FshiComment, FshiReply, AddCategory, ShowCategories, AddSubCategory
**/
 
using BlogMVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogMVC.Models;
using System.Net.Mail;
using System.Web.Configuration;


namespace BlogMVC.Controllers
{
    public class AdminController : Controller
    {
        private BlogPostRepository blogPostRepository;
        private CategoryRepository categoryRepository;
        private UserRepository userRepository;
        private FilesRepository filesRepository;
        private AdminRepository adminRepository;

        public AdminController()
        {
            blogPostRepository = new BlogPostRepository();
            categoryRepository = new CategoryRepository();
            userRepository = new UserRepository();
            filesRepository = new FilesRepository();
            adminRepository = new AdminRepository();
        }


        /**
        * Data: 25/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: ManagePosts
        * Arsyeja: Menaxhimi i postimeve te pa aprovuara nga admini.
        * Pershkrimi: Kontrollon nese useri eshte admin dhe kthen listen e postimeve te pa aprovuara.
        * Para kushti: Useri duhet te jete i loguar si admin.
        * Post kushti: Kthen  view me listen e postimeve te pa aprovuara.
        * Parametrat: Nuk ka
        * Return: ActionResult - VIEW  me postimet e pa aprovuara.
        **/

        [HttpGet]
        public ActionResult ManagePosts()
        {
            try
            {
                if (Session["role"] == null || Session["role"].ToString() != "admin")
                {
                    return RedirectToAction("Login", "Home");
                }

                var posts = adminRepository.GetBlogPostsNotApproved();

                return View(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting posts: " + ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }



         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: ApprovePost
         * Arsyeja: Aprovim i një postimi të blogut nga admini.
         * Pershkrimi: Aprovon postimin e specifikuar nga ID-ja, ndryshon statusin në "approved", dhe dergon email njoftimi.
         * Parametrat: id - ID e postimit për tu aprovuar.
         * Return: ActionResult - Kthen ne ManagePosts pas aprovimit.
         **/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovePost(int id)
        {
            try
            {
                var post = blogPostRepository.GetBlogPostById(id);
                {
                    adminRepository.ApproveBlogPost(id);
                    post.approved = "yes";
                    SendApprovalEmail(post.user.email, post.title, post.user.username, post.approved);
                }
                return RedirectToAction("ManagePosts");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error approving post: " + ex.Message);
                return RedirectToAction("ManagePosts");
            }
        }

        private void SendApprovalEmail(string toEmail, string postTitle,string username,string approved)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                string emailUsername = WebConfigurationManager.AppSettings["EmailUsername"];
                string emailPassword = WebConfigurationManager.AppSettings["EmailPassword"];

                mail.From = new MailAddress(emailUsername);
                mail.To.Add(toEmail);
                if (approved == "yes")
                {
                    mail.Subject = "Your blog post has been approved";
                    mail.Body = $"Dear {username},\n\n Your blog post titled '{postTitle}' has been approved and is now live on our site.\n\nBest regards,\nRadianceBlog Team";
                }
                else
                {
                    mail.Subject = "Your blog post has been rejected";
                    mail.Body = $"Dear {username},\n\n Your blog post titled '{postTitle}' has been rejected.\n\nBest regards,\nRadianceBlog Team";
                }
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
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: DeletePost
         * Arsyeja: Refuzimi(fshirja nga db) e nje posti te blogut nga admini.
         * Pershkrimi: Fshin postimin e specifikuar nga ID-ja dhe dergon email njoftimi per refuzimin.
         * Parametrat: id - ID e postimit.
         * Return: ActionResult - Kthen ne ManagePosts pas fshirjes.
         **/

        public ActionResult DeletePost(int id)
        {
            try
            {
                var post = blogPostRepository.GetBlogPostById(id);
                adminRepository.DeleteBlogPosts(id);
                SendApprovalEmail(post.user.email, post.title, post.user.username, post.approved);
                return RedirectToAction("ManagePosts");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting post: " + ex.Message);
                return RedirectToAction("ManagePosts");
             }
        }

         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: ManageComments
         * Arsyeja: Menaxhimi i komenteve te pa aprovuara nga admini.
         * Pershkrimi: Kontrollon nëse useri eshte i loguar si admin dhe kthen listen e komenteve te pa aprovuara.
         * Para kushti: Useri duhet te jetë i loguar si admin.
         * Post kushti: Kthen view me listen e komenteve te pa aprovuara.
         * Parametrat: Nuk ka
         * Return: ActionResult - VIEW me komentet e pa aprovuara.
         **/

        [HttpGet]
        public ActionResult ManageComments()
        {
            try
            {
                if (Session["role"] == null || Session["role"].ToString() != "admin")
                {
                    return RedirectToAction("Login", "Home");
                }

                var comments = adminRepository.GetCommentsNotApproved();

                return View(comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting comments: " + ex.Message);
                return RedirectToAction("ManageComments", "Admin");
             }
        }


         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: ApproveComment
         * Arsyeja: Aprovim i nje komenti nga admini.
         * Pershkrimi: Aprovon komentin e specifikuar nga ID-ja dhe kthen ne ManageComments.
         * Parametrat: id - ID e komentit.
         * Return: ActionResult - Kthen ne ManageComments pas aprovimit.
         **/

        public ActionResult ApproveComment(int id)
        {
            try
            {
                adminRepository.ApproveComment(id);
                return RedirectToAction("ManageComments");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error approving comment: " + ex.Message);
                return RedirectToAction("ManageComments");
            }
        }

         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: DeleteComment
         * Arsyeja: Refuzimi e nje komenti nga admini.
         * Pershkrimi: Fshin komentin e specifikuar nga ID-ja dhe kthen ne ManageComments.
         * Parametrat: id - ID e komentit për tu fshirë.
         * Return: ActionResult - Kthen ne ManageComments pas refuzimit.
         **/

        public ActionResult DeleteComment(int id)
        {
            try
            {
                adminRepository.DeleteComment(id);
                return RedirectToAction("ManageComments");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting comment: " + ex.Message);
                return RedirectToAction("ManageComments");
            }
        }


         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: FshiComment
         * Arsyeja: Fshirja e nje komenti  dhe kthimi ne postin ku ndodhej komenti.
         * Pershkrimi: Fshin komentin e specifikuar dhe kthen ne postin ku ndodhej komenti.
         * Parametrat: id - ID e komentit.
         * Return: ActionResult - Kthen ne postimin e blogut pas fshirjes se komentit.
         **/
        public ActionResult FshiComment(int id)
        {
            int postId = adminRepository.GetPostIdByCommentId(id);
            try
            {
                adminRepository.DeleteComment(id);
                return RedirectToAction("Post", "BlogPost", new { id = postId });
            }
            catch (Exception ex)
            {
                 Console.WriteLine("Error deleting comment: " + ex.Message);
                return RedirectToAction("Post", "BlogPost", new { id = postId });
            }

        }

         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: FshiReply
         * Arsyeja: Fshirja e një përgjigjeje në një koment të përshtatur.
         * Pershkrimi: Fshin përgjigjen e specifikuar në komentin e përshtatur dhe kthen në postimin e blogut përkatës.
         * Parametrat: id - ID e përgjigjes për tu fshirë.
         * Return: ActionResult - Kthen në postimin e blogut pas fshirjes së përgjigjes.
         **/

        public ActionResult FshiReply(int id)
        {               
            int postId = adminRepository.GetPostIdByCommentId(id);
            try 
            { 
                adminRepository.FshiReply(id);
                return RedirectToAction("Post", "BlogPost", new { id = postId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting reply: " + ex.Message);
                return RedirectToAction("Post", "BlogPost", new { id = postId });
            }

        }

         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: AddCategory
         * Arsyeja: Shtimi i nje kategori te re nga admini.
         * Pershkrimi: Kthen view ne  formen e shtimit te nje kategorie te re nese useri eshte i loguar si admin.
         * Para kushti: Userr duhet te jeet i loguar si admin.
         * Parametrat: Nuk ka
         * Return: ActionResult - VIEW te forma e shtimit te kategorive.
         **/


        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["role"] == null || Session["role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "Home");
            }
             return View( );  
        }

         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: AddCategory (Post)
         * Arsyeja: Shtimi i nje kategorie te re nga admini.
         * Pershkrimi: Shton kategorine e re nese te dhenat jane valide dhe kthen ne formen per shtimin e kategorive.
         * Parametrat: category - Objekti i kategorise per tu shtuar.
         * Return: ActionResult - Kthen ne AddCategory pas shtimit te kategorise.
         **/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    category.created_at = DateTime.Now;
                    category.updated_at = DateTime.Now;

                    adminRepository.AddCategory(category);

                    return RedirectToAction("AddCategory");
                }
                return View();
            }
            catch (Exception ex) {
                Console.WriteLine("Error adding category: " + ex.Message);
                return RedirectToAction("AddCategory");
            }

        }

         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: ShowCategories
         * Arsyeja: Shfaqja e kategorive te krijuara nga admini.
         * Pershkrimi: Kontrollon nese useri eshte i loguar si admin dhe kthen listen e kategorive dhe subkategorive perkatese.
         * Para kushti: useri duhet te jete i loguar si admin.
         * Return: ActionResult - VIEW me kategorite dhe subkategorite perkatese.
         **/

        public ActionResult ShowCategories()
        {
            try
            {
                if (Session["role"] == null || Session["role"].ToString() != "admin")
                {
                    return RedirectToAction("Login", "Home");
                }

                var categoriesandsubcategories = categoryRepository.GetCategories();

                return View(categoriesandsubcategories);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting categories: " + ex.Message);
                return RedirectToAction("ShowCategories", "Admin");
            }
        }


         /**
         * Data: 25/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: AddSubCategory
         * Arsyeja: Shtimi i n je sub-kategori nga admini.
         * Pershkrimi: Shton nje nsub-kategori nese te dhenat(ermi i subkategorise dhe id e kategorise) jane valide dhe kthen nje response JSON  sukses ose fail.
         * Parametrat: name - Emri i sub-kategorise  , category_id - ID e kategorise qe do jete parent category e kesaj subkategorie qe po shtoj.
         * Return: JsonResult - responce JSON per sukses ose fail.
         **/

        [HttpPost]
        public JsonResult AddSubCategory(string name, int category_id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || category_id <= 0)
                {
                    return Json(new { success = false, message = "Invalid input." });
                }
                adminRepository.AddSubCategory(name, category_id);
                return Json(new { success = true });
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error adding subcategory: " + ex.Message);
                return Json(new { success = false, message = "Error adding subcategory." });
            }
        } 

    }

}