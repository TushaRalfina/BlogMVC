using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public IEnumerable<category> GetCategories()
        {

            using (var db = new BlogEntities())
            {
                return db.categories.ToList();
            }
        }

        public category GetCategoryById(int id)
        {
            using (var db = new BlogEntities())
            {
                 return db.categories.FirstOrDefault(c => c.id == id);
             }
        }

        public IEnumerable<post> GetBlogPostsByCategoryId(int id)
        {
            using (var db = new BlogEntities())
            {
                var postsInCategory = (from post in db.posts.Include("PostCategories")
                                       where post.PostCategories.Any(pc => pc.category_id == id) && post.approved=="yes"
                                       select post).ToList();

                return postsInCategory;
            }
        }


    }
}
 