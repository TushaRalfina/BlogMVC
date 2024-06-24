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


        [HttpGet]
        public ActionResult ManagePosts()
        {
            if (Session["role"] == null || Session["role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "Home");
            }
 
             var posts = adminRepository.GetBlogPostsNotApproved();

             return View(posts);
        }
         
                    
        public ActionResult List()
        {
            return View();
        }

        //ApprovePost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovePost(int id)
        {
            var post = blogPostRepository.GetBlogPostById(id);
            {
                adminRepository.ApproveBlogPost(id);
                post.approved = "yes";
                SendApprovalEmail(post.user.email, post.title,post.user.username,post.approved);
            }
            return RedirectToAction("ManagePosts");
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


        //DeletePost
        public ActionResult DeletePost(int id)
        {
            var post = blogPostRepository.GetBlogPostById(id);
            {
                adminRepository.DeleteBlogPosts(id);
                SendApprovalEmail(post.user.email, post.title, post.user.username, post.approved);
            }
            return RedirectToAction("ManagePosts");
        }

        //manageComments
        [HttpGet]
        public ActionResult ManageComments()
        {
            if (Session["role"] == null || Session["role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "Home");
            }

            var comments = adminRepository.GetCommentsNotApproved();

            return View(comments);
        }

        //ApproveComment
        public ActionResult ApproveComment(int id)
        {
            adminRepository.ApproveComment(id);
            return RedirectToAction("ManageComments");
        }

        //DeleteComment
        public ActionResult DeleteComment(int id)
        {
            adminRepository.DeleteComment(id);
            return RedirectToAction("ManageComments");
        }
        //fshi koment
        public ActionResult FshiComment(int id)
        {
            int postId = adminRepository.GetPostIdByCommentId(id);

            adminRepository.DeleteComment(id);
            return RedirectToAction("Post","BlogPost", new { id = postId });

        }

        //fshi reply
        public ActionResult FshiReply(int id)
        {

            int commentId = adminRepository.GetCommentIdByReplyId(id);

            int postId = adminRepository.GetPostIdByCommentId(commentId);
            adminRepository.FshiReply(id);
            return RedirectToAction("Post", "BlogPost", new { id = postId });

        }


        //add category
        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["role"] == null || Session["role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "Home");
            }

 
             return View( );  
        }

        //add category
         [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(category category)
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

        //show categories and subcategories
        public ActionResult ShowCategories()
        {
            if (Session["role"] == null || Session["role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "Home");
            }

            var categoriesandsubcategories = categoryRepository.GetCategories();
             
            return View(categoriesandsubcategories);
        }

        //add subcategory
       [HttpPost]
        public JsonResult AddSubCategory(string name, int category_id)
        {
            if (string.IsNullOrWhiteSpace(name) || category_id <= 0)
            {
                return Json(new { success = false, message = "Invalid input." });
            }
            adminRepository.AddSubCategory(name, category_id);
            return Json(new { success = true });
        } 

    }

}






