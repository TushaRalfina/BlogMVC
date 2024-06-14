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

        public void AddBlogPost(post post,List<int> categoryIds)
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
           var post=db.posts.FirstOrDefault(x => x.id == id);
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
            var posts= db.posts.Where(p => p.approved == "yes").ToList();
            return posts.OrderByDescending(p => p.created_at);
          }
    }
}