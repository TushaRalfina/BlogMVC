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

        public IEnumerable<subcategory> GetSubCategories(int category_id)
        {
            using (var db = new BlogEntities())
            {
                return db.subcategories.Where(x => x.category_id == category_id).ToList();
            }
        }

         public IEnumerable<category> GetCategoriesWithSubCategories()
        {
            using (var db = new BlogEntities())
            {
                return db.categories.Include("subcategories").ToList();
            }
        }
    }
}