/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: BlogPostRepository
* Arsyeja: Implementimi i metodave per menaxhimin e postimeve në blog dhe komenteve.
* Pershkrimi: Kjo klase ofron funksionalitete per te shtuar dhe marre poste dhe komente, për te aprovuar postime dhe komente, si dhe për te marre postime dhe komente te specifikuara nga useri, kategoria, ose data.
* Interfaces: IBlogPostRepository
* Metodat: 
  - AddBlogPost(post post, List<int> categoryIds): Shton nje post se bashku me kategorite e tij.
  - GetBlogPostById(int id): Kthen nje post me id e dhene.
  - GetBlogPosts(): MKthen nje liste te postimeve te blogut.
  - GetBlogPostsApproved(): Kthen nje liste te postimeve te aprovuara.
  - AddComment(comment comment): Shton nje koment te ri.
  - GetCommentsByPostId(int post_id):Kthen liste te komenteve te aprovuara per nje post te caktuar.
  - GetCommentsApproved(int user_id): Kthen nje liste te komenteve te aprovuara nga nje perdorues i caktuar.
  - AddReply(reply reply): Shton nje reply ne nje koment.
  - GetBlogPostsByUserId(int user_id): Kthen nje liste te posteve te aprovuara nga nje user i caktuar.
  - GetCommentById(int id): Kthen koment me id e dhene.
  - UpdateComment(comment comment): update një koment ekzistues.
  - GetBlogPostsByCategory(string category): Kthen nje liste te postimeve te aprovuara per nje kategori te caktuar.
  - GetBlogPostsByDate(DateTime? fromDate, DateTime? toDate): Kthen nje liste te postimeve te aprovuara per nje periudhe te caktuar.
*/


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
 

       
        public post GetBlogPostById(int id)
        {
            var post = db.posts.FirstOrDefault(p => p.id == id);
            return post;
        }

        public IEnumerable<post> GetBlogPosts()
        {
             var blogPosts = db.posts.Include("user").Include("PostCategories").ToList();


            return blogPosts;
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

      
