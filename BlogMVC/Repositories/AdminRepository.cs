/**
* Versioni: V 1.0.0
* Data: 25/06/2024  
* Programuesi: Ralfina Tusha
* Pershkrimi: Repository qe permban metodat qe mund te perdoren nga AdminController 
(c) Copyright by Soft & Solution 
**/





using BlogMVC.Interfaces;
using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
 
namespace BlogMVC.Repositories
{
    public class AdminRepository : IAdminRepository

    {

         public void AddCategory(category category)
        { 
             using (var db = new BlogEntities())
            {
                db.categories.Add(category);
                db.SaveChanges();
            }
        }

        public void AddSubCategory(string name,int category_id)
        {
            using (var db = new BlogEntities())
            {
                var category = db.categories.Find(category_id);
                if (category == null)
                {
                    throw new Exception("Category not found");
                }
                var subcategory = new category
                {
                    name = name,
                    parent_id = category_id

                };
                db.categories.Add(subcategory);
                db.SaveChanges();    
            }
        }
      


        public void ApproveBlogPost(int id)
        {
            using (var db = new BlogEntities())
            {
                var post = db.posts.Find(id);
                post.approved = "yes";
                db.SaveChanges();
            }
        }

        public void ApproveComment(int id)
        {
            using (var db = new BlogEntities())
            {
                var comment = db.comments.Find(id);
                comment.approved = "yes";
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

        public void DeleteComment(int id)
        {

            using (var db = new BlogEntities())
            {
                var comment = db.comments.Find(id);
                db.replies.RemoveRange(comment.replies);
                db.comments.Remove(comment);
                db.SaveChanges();
            }
        }

        public void FshiReply(int id)
        {
            using (var db = new BlogEntities())
            {
                var reply = db.replies.Find(id);
                db.replies.Remove(reply);
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

        public int GetCommentIdByReplyId(int id)
        {
            using (var db = new BlogEntities())
            {
                var reply = db.replies.Find(id);
                return reply.comment_id;
            }
         }

        public IEnumerable<comment> GetCommentsNotApproved()
        {
            using (var db = new BlogEntities())
            {
                return db.comments.Include(c => c.user).Where(x => x.approved == "no").ToList();
            }
        }

 
        public int GetPostIdByCommentId(int id)
        {
            using (var db = new BlogEntities())
            {
                var comment = db.comments.Find(id);
                return comment.post_id;
            }
        }
 
    }
}