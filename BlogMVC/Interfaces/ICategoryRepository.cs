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

 
        IEnumerable<category> GetCategories();

        category GetCategoryById(int id);

        IEnumerable<post> GetBlogPostsByCategoryId(int id);

        //GetSubcategoriesByCategoryId

        IEnumerable<category> GetSubcategoriesByCategoryId(int id);



    }
}
