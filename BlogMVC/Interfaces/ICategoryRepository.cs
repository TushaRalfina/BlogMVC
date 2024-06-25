/**
* Versioni:  
* Data: 25/06/2024  
* Programuesi: Ralfina Tusha
* Pershkrimi: Interface qe permban metodat qe mund te perdoren nga CategoryRepository
* Metodat: GetCategories, GetCategoryById, GetBlogPostsByCategoryId, GetSubcategoriesByCategoryId
(c) Copyright by Soft & Solution 
**/








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


        IEnumerable<category> GetSubcategoriesByCategoryId(int id);

    }
}
