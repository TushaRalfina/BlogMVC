/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: CategoryRepository
* Arsyeja: Implementimi i metodave per marrjen e  kategorive dhe subkategorive.
* Pershkrimi: Kjo klase ofron funksionalitete per te marre te gjitha kategorite, per te marre nje kategori specifike sipas ID-se, per te marre postime ne nje kategori te caktuar dhe per te marre nenkategorite e nje kategorie te caktuar.
* Trashegon nga: Asnje
* Interfaces: ICategoryRepository
* Constants: Asnje
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

        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetCategories
        * Pershkrimi: Kjo metode kthen nje liste te te gjitha kategorive ne db.
        * Return: IEnumerable<category>: Nje liste e te gjitha kategorive.
        **/
        public IEnumerable<category> GetCategories()
        {

            using (var db = new BlogEntities())
            {
                return db.categories.ToList();
            }
        }

        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetCategoryById
        * Pershkrimi: Kjo metode kthen nje kategori bazuar ne ID-ne e saj.
        * Parametrat:
        * - int id: ID-ja e kategorise qe do te merret.
        * Return: category: Objekti i kategorise qe korrespondon me ID-ne.
        **/

        public category GetCategoryById(int id)
        {
            using (var db = new BlogEntities())
            {
                 return db.categories.FirstOrDefault(c => c.id == id);
             }
        }

        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetBlogPostsByCategoryId
         * Pershkrimi: Kjo metode kthen nje liste te postimeve te aprovuara te blogut qe i perkasin nje kategorie te caktuar.
         * Parametrat:
        * - int id: ID-ja e kategorise.
        * Return: IEnumerable<post>: Nje liste e postimeve te aprovuara te lidhura me kategorine.
        **/

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
        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha 
        * Metoda: GetSubcategoriesByCategoryId
        * Arsyeja: Per te marre nje liste te nenkategorive te nje kategorie te caktuar.
        * Pershkrimi: Kjo metode kthen nje liste te nenkategorive te nje kategorie te caktuar.
         * Parametrat:
        * - int id: ID-ja e kategorise.
        * Return: IEnumerable<category>: Nje liste e nenkategorive te lidhura me kategorine.
        **/

        public IEnumerable<category> GetSubcategoriesByCategoryId(int id)
        {

            using (var db = new BlogEntities())
            {
                return db.categories.Where(c => c.parent_id == id).ToList();
            }
         }
    }
}
 