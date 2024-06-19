﻿using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using BlogMVC.Interfaces;
using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using BlogMVC.Repositories;
using PagedList;



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


        public HomeController( )
        {
            userRepository = new UserRepository();
            blogPostRepository=new BlogPostRepository();
            categoryRepository=new CategoryRepository();
            filesRepository = new FilesRepository();
            adminRepository = new AdminRepository();
         }
       

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(UserViewModel userViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(userViewModel);
                }

                if (userRepository.GetUserByUsername(userViewModel.username) != null)
                {
                    ViewBag.Notification = "This username is already taken";
                    return View(userViewModel);
                }
                else
                {
                     userRepository.AddUser(userViewModel);
                    
                    var user = userRepository.GetUserByUsername(userViewModel.username);

                    Session["id"] = user.id.ToString();
                    Session["username"] = user.username;
                    Session["role"] = user.role;

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                 return View("Error");
            }
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
                ViewBag.Notification = "Invalid username or password";
                return View("SignUp");
            }
        }






 
     public ActionResult Index(int? page)
    {
        if (Session["id"] == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var posts = blogPostRepository.GetBlogPostsApproved().ToList();
        int pageSize = 8;
        int pageNumber = (page ?? 1);

        return View(posts.ToPagedList(pageNumber, pageSize));
    }



    //GET:PROFILE
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
            //file nga repository
            var file = filesRepository.GetFileById(fileId);

            if (file != null)
            {
                byte[] fileBytes = Convert.FromBase64String(file.file_content); // Convert base64 string back to byte array
                return File(fileBytes, "application/octet-stream", file.file_name); // Return file as download
            }

            return HttpNotFound();
        }

        //GET:EDIT PROFILE
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
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

       
        public ActionResult Categories()
        {
            return View();
        }
      


    }
}