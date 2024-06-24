using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        BlogEntities db = new BlogEntities();

        public void AddBlogPost(post post, List<int> categoryIds)
        {
            db.posts.Add(post);
            db.SaveChanges();
            foreach (var categoryId in categoryIds)
            {
                var postCategory = new PostCategory
                {
                    post_id = post.id,
                    category_id = categoryId
                };
                db.PostCategories.Add(postCategory);
            }
            db.SaveChanges();

        }

        public void DeleteBlogPost(int id)
        {
            throw new NotImplementedException();
        }

       
        public post GetBlogPostById(int id)
        {
            var post = db.posts.FirstOrDefault(p => p.id == id);
            return post;
        }

        public IEnumerable<post> GetBlogPosts()
        {
            //fetch the posts join with the users and categories
            var blogPosts = db.posts.Include("user").Include("PostCategories").ToList();


            return blogPosts;
        }

        public void UpdateBlogPost(post blogPostView)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<post> GetBlogPostsApproved()
        {
            var posts = db.posts.Where(p => p.approved == "yes").ToList();
            return posts.OrderByDescending(p => p.created_at);
        }

        public void AddComment(comment comment)
        {
            db.comments.Add(comment);
            db.SaveChanges();
        }

        public IEnumerable<comment> GetCommentsByPostId(int post_id)
        {
            var comments = db.comments
                             .Include("replies")
                             .Where(c => c.post_id == post_id && c.approved == "yes")
                             .OrderByDescending(c => c.created_at)
                             .ToList();
            return comments;
        }

        public IEnumerable<comment> GetCommentsApproved(int user_id)
        {
            var comments = db.comments.Where(c => c.approved == "yes" && c.user_id == user_id).ToList();
            return comments;
        }

        public void AddReply(reply reply)
        {

            db.replies.Add(reply);
            db.SaveChanges();
            
        }

        public IEnumerable<post> GetBlogPostsByUserId(int user_id)
        {
            var posts = db.posts.Where(p => p.user_id == user_id && p.approved=="yes").ToList();
            return posts; 
        }

        public comment GetCommentById(int id)
        {
            var comment = db.comments.FirstOrDefault(c => c.id == id);
            return comment;
             
        }

        public void UpdateComment(comment comment)
        {
            var commentToUpdate = db.comments.FirstOrDefault(c => c.id == comment.id);
            if (commentToUpdate != null)
            {
                commentToUpdate.comment1 = comment.comment1;  
                commentToUpdate.approved = comment.approved;
                
                db.SaveChanges();
            }
        }

        public IEnumerable<post> GetBlogPostsByCategory(string category)
        {
            return db.posts.Where(p => p.PostCategories.Any(pc => pc.category.name == category) && p.approved == "yes").ToList();
        }

        public IEnumerable<post> GetBlogPostsByDate(DateTime? fromDate, DateTime? toDate)
        {
            return db.posts.Where(p => p.created_at >= fromDate && p.created_at <= toDate && p.approved == "yes").ToList();
        }
    }
}

      
