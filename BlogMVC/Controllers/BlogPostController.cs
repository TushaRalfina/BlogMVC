/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: BlogPostController
* Arsyeja: Kjo klasë menaxhon funksionalitetet e postimeve të blogut.
* Pershkrimi: Implementon metodat për shtimin e postimeve nga useri, shikimin e nje posti specifik, shtimin/editimin e komenteve dhe reply.
* Trashegon nga: Controller
* Interfaces: Nuk ka
* Constants: Nuk ka
* Metodat: Add, Post, AddComment, AddReply, UserPosts, EditComment
*/

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
            var categories = categoryRepository.GetCategories();

            var model = new AddBlogPostRequest
            {
                categoriess = categories
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult Add(AddBlogPostRequest model, IEnumerable<HttpPostedFileBase> body_images)
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
                            string fileName = Path.GetFileName(model.main_imagee.FileName);
                            string uploadsDir = Server.MapPath("~/Content/Uploads");
                            string filePath = Path.Combine(uploadsDir, fileName);

                            if (!Directory.Exists(uploadsDir))
                            {
                                Directory.CreateDirectory(uploadsDir);
                            }

                             model.main_imagee.SaveAs(filePath);

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

                     var post = new post
                    {
                        title = model.title,
                        content = model.content,
                        created_at = DateTime.Now,
                        user_id = user_id,
                        main_image = model.main_image
                    };

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

                                     if (body_images == null || !body_images.Any())
                                    {
                                        var documentFile = new file
                                        {
                                            post_id = post.id,
                                            file_type = Path.GetExtension(file.FileName),
                                            file_name = Path.GetFileName(file.FileName),
                                            file_content = Convert.ToBase64String(fileBytes),
                                            body_images = ""
                                        };

                                        filesRepository.AddFiles(documentFile);
                                    }

                                    else
                                    {
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
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", $"An error occurred while saving a document: {ex.Message}");
                            }
                        }
                    }


                    var categories = categoryRepository.GetCategories();
                    model.categoriess = categories;
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return RedirectToAction("Login","Home");
                }
            }

            return View(model);
        }





        public ActionResult Post(int id)
        {
            var post = blogPostRepository.GetBlogPostById(id);
            if (post != null)
            {
                var approvedComments = blogPostRepository.GetCommentsByPostId(id).ToList();

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

 
        [HttpPost]
        public JsonResult AddReply(int comment_id, string reply_text)
        {
            if (Session["id"] != null && int.TryParse(Session["id"].ToString(), out int user_id))
            {
                var user = userRepository.GetUserById(user_id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                var newReply = new reply
                {
                    comment_id = comment_id,
                    user_id = user_id,
                    reply_text = reply_text
                };

                blogPostRepository.AddReply(newReply);

                newReply.user = user;

                return Json(new
                {
                    success = true,
                    reply = new
                    {
                        newReply.reply_text,
                        user.username
                    }
                });
            }
            return Json(new { success = false, message = "User not authenticated." });
         }


         public ActionResult UserPosts(int id)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var posts = blogPostRepository.GetBlogPostsByUserId(id).ToList();
            return View(posts);
        }


        [HttpPost]
        public JsonResult EditComment(int comment_id, string comment_text)
        {
            var comment = blogPostRepository.GetCommentById(comment_id);
            if (comment == null)
            {
                return Json(new { success = false, message = "Comment not found." });
            }
            comment.comment1 = comment_text;
            blogPostRepository.UpdateComment(comment);
            return Json(new { success = true });
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

    
