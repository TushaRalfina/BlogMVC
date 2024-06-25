/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: CategoryRepository
* Arsyeja: Implementimi i metodave për marrjen e  kategorive dhe subkategorive.
* Pershkrimi: Kjo klasë ofron funksionalitete për të marrë të gjitha kategoritë, për të marrë një kategori specifike sipas ID-së, për të marrë postime në një kategori të caktuar dhe për të marrë nënkategoritë e një kategorie të caktuar.
* Trashegon nga: Asnjë
* Interfaces: ICategoryRepository
* Constants: Asnjë
* Metodat: 
  - GetCategories(): Kthen nje liste te kategorive.
  - GetCategoryById(int id): Kthen nje kategori me id e dhene.
  - GetBlogPostsByCategoryId(int id): Kthen nje liste te postimeve per nje kategori te caktuar.
  - GetSubcategoriesByCategoryId(int id): Kthen nje liste te nenkategorive per nje kategori te caktuar.
*/



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

        public IEnumerable<category> GetSubcategoriesByCategoryId(int id)
        {

            using (var db = new BlogEntities())
            {
                return db.categories.Where(c => c.parent_id == id).ToList();
            }
         }
    }
}
 