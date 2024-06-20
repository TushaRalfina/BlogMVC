using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMVC.Repositories
{
    internal interface ICategoryRepository
    {

        //get all categories

        IEnumerable<category> GetCategories();

        //get all subcategories for a category

        IEnumerable<subcategory> GetSubCategories(int category_id);

        //get all categories with subcategories

        IEnumerable<category> GetCategoriesWithSubCategories();
    }
}
