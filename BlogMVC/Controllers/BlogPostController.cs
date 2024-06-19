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
        public ActionResult Add(AddBlogPostRequest model, IEnumerable<HttpPostedFileBase> body_images)
        {
            if (ModelState.IsValid)
            {
                // Ensure user is authenticated
                if (Session["id"] != null && int.TryParse(Session["id"].ToString(), out int user_id))
                {
                    var user = userRepository.GetUserById(user_id);
                    if (user == null)
                    {
                        return RedirectToAction("Login", "Home");
                    }

                    // Handle main image upload
                    if (model.main_imagee != null && model.main_imagee.ContentLength > 0)
                    {
                        try
                        {
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
                            ModelState.AddModelError("", $"An error occurred while saving the main image: {ex.Message}");
                        }
                    }
                    else
                    {
                        model.main_image = "/Content/images/thumbs/masonry/statue-1200.jpg";
                    }

                    // Create a new post object
                    var post = new post
                    {
                        title = model.title,
                        content = model.content,
                        created_at = DateTime.Now,
                        user_id = user_id,
                        main_image = model.main_image
                    };

                    // Add the blog post with categories
                    blogPostRepository.AddBlogPost(post, model.SelectedCategoryIds);



                    if (model.files != null && model.files.Any())
                    {

                        foreach (var file in model.files.Where(f => f != null && f.ContentLength > 0))
                        {
                            try
                            {


                                using (var memoryStream = new MemoryStream())
                                {
                                    file.InputStream.CopyTo(memoryStream);
                                    var fileBytes = memoryStream.ToArray();



                                    foreach (var bodyImage in body_images)
                                    {

                                        var documentFile = new file
                                        {
                                            post_id = post.id,
                                            file_type = Path.GetExtension(file.FileName),
                                            file_name = Path.GetFileName(file.FileName),
                                            file_content = Convert.ToBase64String(fileBytes),
                                            body_images = ""
                                        };
                                        documentFile.body_images += $"/Content/Uploads/{bodyImage.FileName};";
                                        if (!string.IsNullOrEmpty(documentFile.body_images))
                                        {
                                            documentFile.body_images = documentFile.body_images.TrimEnd(';');
                                        }




                                        filesRepository.AddFiles(documentFile);
                                    }



                                }
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", $"An error occurred while saving a document: {ex.Message}");
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
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }

            // If ModelState is not valid, return to the same view with errors
            return View(model);
        }





        public ActionResult Post(int id)
        {
            var post = blogPostRepository.GetBlogPostById(id);
            if (post != null)
            {
                 var approvedComments = post.comments.Where(c => c.approved == "yes").ToList();

                // view model to pass both post and comments
                var viewModel = new PostViewModel
                {
                    Post = post,
                    ApprovedComments = approvedComments
                };

                return View(viewModel);
            }

            return HttpNotFound("Post not found.");
        }



        [HttpPost]
        public JsonResult AddComment(int post_id, string comment)
        {
            if (Session["id"] != null && int.TryParse(Session["id"].ToString(), out int user_id))
            {
                var user = userRepository.GetUserById(user_id);
               
                var newComment = new comment
                {
                    post_id = post_id,
                    user_id = user_id,
                    comment1 = comment,
                    created_at = DateTime.Now,
                    approved = "no"  
                };

                blogPostRepository.AddComment(newComment);

                
                newComment.user = user;

                return Json(new
                {
                    success = true,
                    comment = new
                    {
                        newComment.comment1,
                        user.username,
                        created_at = newComment.created_at
                    }
                });
            }

            return Json(new { success = false, message = "User not authenticated." });
        }
    }
}































        // Add Comment with AJAX
        /*[HttpPost]
        public ActionResult AddComment(int post_id, string comment1)
        {
            if (Session["id"] != null && int.TryParse(Session["id"].ToString(), out int user_id))
            {
                var user = userRepository.GetUserById(user_id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                var newComment = new comment
                {
                    post_id = post_id,
                    user_id = user_id,
                    comment1 = comment1,
                    created_at = DateTime.Now,
                    approved = "no"
                };

                blogPostRepository.AddComment(newComment);

                // Assuming GetCommentsByPostId returns the updated list of comments
                var comments = blogPostRepository.GetCommentsByPostId(post_id);

                // Return JSON response with success flag and updated comments
                return Json(new { success = true, comments = comments });
            }
            else
            {
                return Json(new { success = false, message = "User not logged in." });
            }
        }*/

    
