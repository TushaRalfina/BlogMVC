using BlogMVC.Models.ViewModels;
using BlogMVC.Models;
using BlogMVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace BlogMVC.Controllers
{
    public class BlogPostController : Controller
    {
        private BlogPostRepository blogPostRepository;
        private CategoryRepository categoryRepository;
        private UserRepository userRepository;
        private FilesRepository filesRepository;
        public BlogPostController()
        {
            blogPostRepository = new BlogPostRepository();
            categoryRepository = new CategoryRepository();
            userRepository= new UserRepository();
           filesRepository = new FilesRepository();
        }


        [HttpGet]
        public ActionResult Add()
        {
             if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
             //get all categories
             var categories = categoryRepository.GetCategories();

            var model = new AddBlogPostRequest
            {
                categories = categories.Select(c => new SelectListItem
                {
                    Text = c.name,
                    Value = c.id.ToString()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(AddBlogPostRequest model,IEnumerable<HttpPostedFileBase>files)
        {
            if (ModelState.IsValid)
            {

                if (Session["id"] != null && int.TryParse(Session["id"].ToString(), out int user_id))
                {
                    var user = userRepository.GetUserById(user_id);
                    if (user == null)
                    {
                        return RedirectToAction("Login", "Home");
                    }
                    var post = new post
                    {
                        title = model.title,
                        content = model.content,
                        created_at = DateTime.Now,
                        user_id = user_id
                    };

                    blogPostRepository.AddBlogPost(post, model.SelectedCategoryIds);

                    foreach (var file in files)
                    {
                        if (file != null & file.ContentLength>0)
                        {
                            var fileEntity = new file
                            {
                                post_id = post.id,
                                file_type = file.ContentType,
                                file_name = file.FileName,
                                file_content = ConvertToBase64(file)
                            };

                            filesRepository.AddFiles(fileEntity);
                            
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(model);
                }
            }
            var categories = categoryRepository.GetCategories();
            model.categories = categories.Select(c => new SelectListItem
            {
                Text = c.name,
                Value = c.id.ToString()
            }).ToList();

            return View(model);
        }
        private string ConvertToBase64(HttpPostedFileBase file)
        {
            using (var reader = new System.IO.BinaryReader(file.InputStream))
            {
                var fileBytes = reader.ReadBytes(file.ContentLength);
                return Convert.ToBase64String(fileBytes);
            }
        }



    }
}