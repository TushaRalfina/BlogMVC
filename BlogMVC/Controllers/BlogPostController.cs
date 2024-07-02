/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: BlogPostController
* Arsyeja: Kjo klase menaxhon funksionalitetet e postimeve te blogut.
* Pershkrimi: Implementon metodat per shtimin e postimeve nga useri, shikimin e nje posti specifik, shtimin/editimin e komenteve dhe reply.
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



        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: Add (GET)
         * Arsyeja: Shfaq VIEW per shtimin e nje  postimi te ri ne  blog nga useri.
         * Pershkrimi: Kontrollon nese perdoruesi eshte i loguar. Nese jo, e ridrejton ne faqen e login-it. Nese eshte i loguar, merr kategorite ne nje instance te AddBlogPostRequest dhe i kalon ato ne VIEW.
         * Para kushti: Perdoruesi duhet te jete i loguar.
         * Post kushti: Kthen nje View me modelin e formes per shtimin e postimit te ri.
         * Parametrat: Nuk ka.
         * Return: ActionResult - VIEW me modelin e formes per shtimin e postimit te ri.
         **/
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



        /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: Add (POST)
         * Arsyeja: Ruajtja e nje postimi te ri ne blog.
         * Pershkrimi: Kontrollon vlefshmerine e modelit. Nese perdoruesi eshte i loguar, krijon dhe ruan postimin e ri se bashku me imazhet dhe filet e bashkangjitur.
         * Para kushti: Perdoruesi duhet te jete i loguar dhe model duhet te jete valid.
         * Post kushti: Ruhet postimi i ri dhe ridrejtohet ne faqen kryesore.
         * Parametrat: model - Modeli i kerkeses per shtimin e postimit; body_images - Lista e imazheve.
         * Return: ActionResult - Ridrejtohet ne faqen kryesore ose rikthen View me gabimet.
         **/
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
                            using (var memoryStream = new MemoryStream())
                            {
                                model.main_imagee.InputStream.CopyTo(memoryStream);
                                var fileBytes = memoryStream.ToArray();
                                model.main_image = Convert.ToBase64String(fileBytes);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"An error occurred while saving the main image: {ex.Message}");
                        }
                    }
                    else
                    {
                        model.main_image = Convert.ToBase64String(System.IO.File.ReadAllBytes(Server.MapPath("~/Content/images/thumbs/masonry/statue-1200.jpg")));
                    }

                    var post = new post
                    {
                        title = model.title,
                        content = model.content,
                        created_at = DateTime.Now,
                        user_id = user_id,
                        main_image = model.main_image,
                        approved = user.role == "admin" ? "yes" : "no",
                        invalidate = 10
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
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", $"An error occurred while saving a file: {ex.Message}");
                            }
                        }
                    }

                    if (body_images != null && body_images.Any())
                    {
                        foreach (var bodyImage in body_images.Where(bi => bi != null && bi.ContentLength > 0))
                        {
                            try
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    bodyImage.InputStream.CopyTo(memoryStream);
                                    var fileBytes = memoryStream.ToArray();

                                    var documentFile = new file
                                    {
                                        post_id = post.id,
                                        file_type = Path.GetExtension(bodyImage.FileName),
                                        file_name = Path.GetFileName(bodyImage.FileName),
                                        file_content = "",
                                        body_images = Convert.ToBase64String(fileBytes),
                                    };

                                    filesRepository.AddFiles(documentFile);
                                }
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", $"An error occurred while saving a body image: {ex.Message}");
                            }
                        }
                    }

                    var categories = categoryRepository.GetCategories();
                    model.categoriess = categories;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }

            return View(model);
        }






        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: Post
        * Arsyeja: Shfaqja e nje posti te vetem.
        * Pershkrimi: Shfaq postimin e caktuar me te gjitha komentet e aprovuara.
        * Para kushti: Nuk ka.
        * Post kushti: Kthen nje View me modelin e postimit dhe komenteve te aprovuara.
        * Parametrat: id - id e postit.
        * Return: ActionResult - VIEW me modelin e postimit dhe komenteve te aprovuara.
        **/
        public ActionResult Post(int id)
        {
            var post = blogPostRepository.GetBlogPostById(id);
            if (post != null)
            {
                var approvedComments = blogPostRepository.GetCommentsByPostId(id).ToList();
                var files = filesRepository.GetFilesByPostId(id).ToList();

                var viewModel = new PostViewModel
                {
                    Post = post,
                    ApprovedComments = approvedComments,
                    Files = files
                };

                var allowedFileTypes = new List<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
                var imageExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
             

                return View(viewModel);
            }

            return HttpNotFound("Post not found.");
        }


         /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: AddComment
         * Arsyeja: Shtimi i nje komenti ne nje postim.
         * Pershkrimi: Kontrollon nese perdoruesi eshte i loguar. Nese eshte i loguar, shton komentin e ri ne databaze dhe kthen nje pergjigje JSON me sukses dhe komentin e shtuar.
         * Para kushti: Perdoruesi duhet te jete i loguar.
         * Post kushti: Kthen nje pergjigje JSON me sukses dhe komentin e shtuar.
         * Parametrat: post_id - id e postit; comment - teksti i komentit.
         * Return: JsonResult - Pergjigje JSON me sukses dhe komentin e shtuar.
         **/
        [HttpPost]
        public JsonResult AddComment(int post_id, string comment)
        {
            if (Session["id"] != null && int.TryParse(Session["id"].ToString(), out int user_id))
            {
                var user = userRepository.GetUserById(user_id);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                if (string.IsNullOrWhiteSpace(comment))
                {
                    return Json(new { success = false, message = "Comment text is required." });
                }

                var newComment = new comment
                {
                    post_id = post_id,
                    user_id = user_id,
                    comment1 = comment,
                    created_at = DateTime.Now,
                    approved = user.role == "admin" ? "yes" : "no",
                    invalidate = 10
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


         /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: AddReply
         * Arsyeja: Shtimi i nje pergjigje te re ne nje koment te blogut.
         * Pershkrimi: Kontrollon nese perdoruesi eshte i loguar dhe krijon nje pergjigje te re per nje koment.
         * Para kushti: Perdoruesi duhet te jete i loguar.
         * Post kushti: Pergjigja e re ruhet dhe kthehet si rezultat JSON.
         * Parametrat: comment_id - id e komentit per t'u pergjigjur; reply_text - Teksti i pergjigjes.
         * Return: JsonResult - Rezultati JSON me detajet e pergjigjes se re ose nje mesazh fail.
         **/
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

                if (string.IsNullOrWhiteSpace(reply_text))
                {
                    return Json(new { success = false, message = "Reply text is required." });
                }

                var newReply = new reply
                {
                    comment_id = comment_id,
                    user_id = user_id,
                    reply_text = reply_text,
                    invalidate = 10
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





         /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: UserPosts
         * Arsyeja: Shfaq te gjitha postimet e blogut te krijuara nga nje perdorues specifik.
         * Pershkrimi: Kontrollon nese perdoruesi eshte i loguar dhe merr te gjitha postimet e krijuara nga perdoruesi specifik me ID-ne e dhene.
         * Para kushti: Perdoruesi duhet te jete i loguar.
         * Post kushti: Kthen nje View me listen e postimeve te krijuara nga perdoruesi specifik.
         * Parametrat: id - ID e perdoruesit per te marre postimet.
         * Return: ActionResult - VIEW me listen e postimeve te krijuara nga perdoruesi specifik.
         **/
        public ActionResult UserPosts(int id)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var posts = blogPostRepository.GetBlogPostsByUserId(id).ToList();
            return View(posts);
        }


         /**
         * Data: 26/06/2024
         * Programuesi: Ralfina Tusha
         * Metoda: EditComment
         * Arsyeja: Editimi i nje komenti ekzistues ne nje postim te blogut.
         * Pershkrimi: Merr komentit nga ID-ja, editon tekstin e komentit dhe ruan ndryshimet.
         * Para kushti: Komenti duhet te ekzistoje.
         * Post kushti: Komenti update-ohet dhe kthehet si rezultat JSON.
         * Parametrat: comment_id - ID e komentit per te redaktuar; comment_text - Teksti i ri i komentit.
         * Return: JsonResult - Rezultati JSON me statusin e suksesit ose nje mesazh fail.
         **/
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






























 

    
