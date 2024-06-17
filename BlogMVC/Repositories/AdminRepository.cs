﻿using BlogMVC.Interfaces;
using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace BlogMVC.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        public void ApproveBlogPost(int id)
        {
             using (var db = new BlogEntities())
            {
                var post = db.posts.Find(id);
                post.approved = "yes";
                db.SaveChanges();
            }
        }

        public void DeleteBlogPosts(int id)
        {
            using (var db = new BlogEntities())
            {
                var post = db.posts.Find(id);
                db.PostCategories.RemoveRange(post.PostCategories);
                db.files.RemoveRange(post.files);
                db.posts.Remove(post);
                db.SaveChanges();
            }
           
        }

        public IEnumerable<post> GetBlogPostsNotApproved()
        {

             using (var db = new BlogEntities())
            {
                return db.posts.Include(p => p.user).Where(x => x.approved == "no").ToList();
            }
            
        }
    }
}