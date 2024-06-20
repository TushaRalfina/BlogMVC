using BlogMVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogMVC.Models;


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
        public ActionResult ApprovePost(int id)
        {
            adminRepository.ApproveBlogPost(id);
            return RedirectToAction("ManagePosts");
        }

        //DeletePost
        public ActionResult DeletePost(int id)
        {
            adminRepository.DeleteBlogPosts(id);
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

        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["role"] == null || Session["role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "Home");
            }

 
             return View( );  
        }

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

            var categoriesandsubcategories = categoryRepository.GetCategoriesWithSubCategories();
             
            return View(categoriesandsubcategories);
        }

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






