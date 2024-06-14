using BlogMVC.Models.ViewModels;
using BlogMVC.Models;
using BlogMVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

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
            userRepository = new UserRepository();
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
        public ActionResult Add(AddBlogPostRequest model)
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

                    if (model.main_imagee != null && model.main_imagee.ContentLength > 0)
                    {
                        try
                        {
                            // Generate a unique file name to avoid overwriting
                            string fileName = Path.GetFileName(model.main_imagee.FileName);
                            string uploadsDir = Server.MapPath("~/Content/Uploads");
                            string filePath = Path.Combine(uploadsDir, fileName);

                            // Ensure the directory exists
                            if (!Directory.Exists(uploadsDir))
                            {
                                Directory.CreateDirectory(uploadsDir);
                            }

                            // Save the file
                            model.main_imagee.SaveAs(filePath);

                            // Update the model with the saved file path
                            model.main_image = "/Content/Uploads/" + fileName;

                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"An error occurred while saving the file: {ex.Message}");
                        }
                    }
                    else
                    {
                        model.main_image = "/Content/images/thumbs/masonry/statue-1200.jpg";
                    }

                    var post = new post
                    {
                        title = model.title,
                        content = model.content,
                        created_at = DateTime.Now,
                        user_id = user_id,
                        main_image = model.main_image // Use the updated main_image path
                    };

                    // Add the blog post with categories
                    blogPostRepository.AddBlogPost(post, model.SelectedCategoryIds);
                    // Handle additional files
                    if (model.files != null && model.files.Count > 0)
                    {
                        foreach (var file in model.files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    file.InputStream.CopyTo(memoryStream);
                                    var fileBytes = memoryStream.ToArray();

                                    // Save file to the database
                                    var fileData = new file
                                    {
                                        post_id = post.id,
                                        file_type = Path.GetExtension(file.FileName), // Get file extension
                                        file_name = Path.GetFileName(file.FileName), // Get file name
                                        file_content = Convert.ToBase64String(fileBytes) // Convert to base64
                                    };

                                    filesRepository.AddFiles(fileData);
                                }
                            }
                        }



                        // Get updated categories list
                        var categories = categoryRepository.GetCategories();
                        model.categories = categories.Select(c => new SelectListItem
                        {
                            Text = c.name,
                            Value = c.id.ToString()
                        }).ToList();

                        return RedirectToAction("Index", "Home");
                    }

                    return RedirectToAction("Login", "Home");
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }




        public ActionResult Post()
        {
            return View();
        }

    }
}