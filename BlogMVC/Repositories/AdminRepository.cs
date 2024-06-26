/**
* Versioni: V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: AdminRepository
* Arsyeja: Implementimi i metodave per administrimin e kategorive, postimeve dhe komenteve.
* Pershkrimi: Kjo klase ofron funksionalitete per te shtuar kategori dhe nenkategori, per te aprovuar dhe fshire postime dhe komente, si dhe per te marre listat e postimeve dhe komenteve qe nuk jane aprovuar.
* Interfaces: IAdminRepository
* Metodat:
  - AddCategory(category category): Shton nje kategori te re.
  - AddSubCategory(string name, int category_id): Shton nje subkategori te re nen nje kategori ekzistuese.
  - ApproveBlogPost(int id): Aprovon nje postim.
  - ApproveComment(int id): Aprovon nje koment.
  - DeleteBlogPosts(int id): Fshin nje postim blogu.
  - DeleteComment(int id): Fshin nje koment dhe replies perkatese.
  - FshiReply(int id): Fshin nje reply.
  - GetBlogPostsNotApproved():Kthen nje liste te postimeve te paaprovuara.
  - GetCommentIdByReplyId(int id): Kthen id-ne e komentit qe i korrespondon nje reply i caktuar.
  - GetCommentsNotApproved(): Kthen nje liste te komenteve te paaprovuara.
  - GetPostIdByCommentId(int id): Kthen id-ne e postimit per nje koment te caktuar.
*/


using BlogMVC.Interfaces;
using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
 
namespace BlogMVC.Repositories
{
    public class AdminRepository : IAdminRepository

    {
        /**
        * Data: 26/06/2024
        * Programuesi:Ralfina Tusha 
        * Metoda: AddCategory
         * Pershkrimi: Kjo metode shton nje kategori te re ne db.
        * Parametrat:
        * - category category: Objekti i kategorise qe do te shtohet.
        * Return: Nuk ka.
        **/

        public void AddCategory(category category)
        { 
             using (var db = new BlogEntities())
            {
                db.categories.Add(category);
                db.SaveChanges();
            }
        }
        /**
        * Data: 26/06/2024
        * Programuesi:Ralfina Tusha 
        * Metoda: AddSubCategory
         * Pershkrimi: Kjo metode shton nje nenkategori te re ne nje kategori ekzistuese ne db.
        * Para kushti: Kategoria prind duhet te ekzistoje.
        * Post kushti: Nenkategoria e re eshte shtuar ne bazen e te dhenave.
        * Parametrat:
        * - string name: Emri i nenkategorise.
        * - int category_id: ID-ja e kategorise prind.
        * Return: Nuk ka.
        **/

        public void AddSubCategory(string name,int category_id)
        {
            using (var db = new BlogEntities())
            {
                var category = db.categories.Find(category_id);
                if (category == null)
                {
                    throw new Exception("Category not found");
                }
                var subcategory = new category
                {
                    name = name,
                    parent_id = category_id

                };
                db.categories.Add(subcategory);
                db.SaveChanges();    
            }
        }

        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: ApproveBlogPost
        * Pershkrimi: Kjo metode aprovon nje postim blogu duke ndryshuar statusin e tij ne "yes".
        * Para kushti: Postimi duhet te ekzistoje.
        * Post kushti: Statusi i aprovimit te postimit eshte ndryshuar ne "yes".
        * Parametrat:
        * - int id: ID-ja e postimit qe do te aprovohet.
        * Return: Nuk ka.
        **/

        public void ApproveBlogPost(int id)
        {
            using (var db = new BlogEntities())
            {
                var post = db.posts.Find(id);
                post.approved = "yes";
                db.SaveChanges();
            }
        }
        /**
            * Data: Data e krijimit
            * Programuesi: 
            * Metoda: ApproveComment
             * Pershkrimi: Kjo metode aprovon nje koment duke ndryshuar statusin e tij ne "yes".
            * Para kushti: Komenti duhet te ekzistoje.
            * Post kushti: Statusi i aprovimit te komentit eshte ndryshuar ne "yes".
            * Parametrat:
            * - int id: ID-ja e komentit qe do te aprovohet.
            * Return: Nuk ka.
            **/
        public void ApproveComment(int id)
        {
            using (var db = new BlogEntities())
            {
                var comment = db.comments.Find(id);
                comment.approved = "yes";
                db.SaveChanges();
            }

        }
         /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: DeleteBlogPosts
         * Pershkrimi: Kjo metode fshin nje postim blogu dhe te gjitha lidhjet  tij.
        * Para kushti: Postimi duhet te ekzistoje.
        * Post kushti: Postimi dhe te gjitha lidhjet e tij jane fshire nga db.
        * Parametrat:
        * - int id: ID-ja e postimit qe do te fshihet.
        * Return: Nuk ka.
        **/

        public void DeleteBlogPosts(int id)
        {
            using (var db = new BlogEntities())
            {
                var post = db.posts.Find(id);
                db.PostCategories.RemoveRange(post.PostCategories);
                db.files.RemoveRange(post.files);
                db.posts.Remove(post);
                db.SaveChanges();
            }

        }
        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: DeleteComment
         * Pershkrimi: Kjo metode fshin nje koment dhe te gjitha reply e tij.
        * Para kushti: Komenti duhet te ekzistoje.
        * Post kushti: Komenti dhe te gjitha reply e tij jane fshire nga db.
        * Parametrat:
        * - int id: ID-ja e komentit qe do te fshihet.
        * Return: Nuk ka.
        **/

        public void DeleteComment(int id)
        {

            using (var db = new BlogEntities())
            {
                var comment = db.comments.Find(id);
                db.replies.RemoveRange(comment.replies);
                db.comments.Remove(comment);
                db.SaveChanges();
            }
        }
        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: FshiReply
         * Pershkrimi: Kjo metode fshin nje reply nga db.
         * Parametrat:
        * - int id: ID-ja e reply qe do te fshihet.
        * Return: Nuk ka.
        **/

        public void FshiReply(int id)
        {
            using (var db = new BlogEntities())
            {
                var reply = db.replies.Find(id);
                db.replies.Remove(reply);
                db.SaveChanges();
            }

        }
        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetBlogPostsNotApproved
         * Pershkrimi: Kjo metode kthen nje liste te postimeve te blogut qe nuk jane aprovuar ende.
         * Parametrat: Nuk ka.
        * Return: IEnumerable<post>: Nje liste e postimeve te blogut te paaprovuar.
        **/

        public IEnumerable<post> GetBlogPostsNotApproved()
        {

            using (var db = new BlogEntities())
            {
                return db.posts.Include(p => p.user).Where(x => x.approved == "no").ToList();
            }

        }
        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetCommentIdByReplyId
        * Pershkrimi: Kjo metode kthen ID-ne e komentit te lidhur me nje reply te dhene.
         * Parametrat:
        * - int id: ID-ja e reply.
        * Return: int: ID-ja e komentit te lidhur me reply.
        **/

        public int GetCommentIdByReplyId(int id)
        {
            using (var db = new BlogEntities())
            {
                var reply = db.replies.Find(id);
                return reply.comment_id;
            }
         }

        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetCommentsNotApproved
         * Pershkrimi: Kjo metode kthen nje liste te komenteve qe nuk jane aprovuar ende.
         * Parametrat: Nuk ka.
        * Return: IEnumerable<comment>: Nje liste e komenteve te paaprovuar.
        **/
        public IEnumerable<comment> GetCommentsNotApproved()
        {
            using (var db = new BlogEntities())
            {
                return db.comments.Include(c => c.user).Where(x => x.approved == "no").ToList();
            }
        }

        /**
        * Data:26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetPostIdByCommentId
        * Pershkrimi: Kjo metode kthen ID-ne e postimit te lidhur me nje koment te dhene.
        * Parametrat:
        * - int id: ID-ja e komentit.
        * Return: int: ID-ja e postimit te lidhur me komentin.
        **/

        public int GetPostIdByCommentId(int id)
        {
            using (var db = new BlogEntities())
            {
                var comment = db.comments.Find(id);
                return comment.post_id;
            }
        }
 
    }
}