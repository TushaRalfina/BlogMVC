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

       

       
    }
}