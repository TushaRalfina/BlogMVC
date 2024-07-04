/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: BlogPostRepository
* Arsyeja: Implementimi i metodave per menaxhimin e postimeve ne blog dhe komenteve.
* Pershkrimi: Kjo klase ofron funksionalitete per te shtuar dhe marre poste dhe komente, per te aprovuar postime dhe komente, si dhe per te marre postime dhe komente te specifikuara nga useri, kategoria, ose data.
* Interfaces: IBlogPostRepository
* Metodat: 
  - AddBlogPost(post post, List<int> categoryIds): Shton nje post se bashku me kategorite e tij.
  - GetBlogPostById(int id): Kthen nje post me id e dhene.
  - GetBlogPosts(): MKthen nje liste te postimeve te blogut.
  - GetBlogPostsApproved(): Kthen nje liste te postimeve te aprovuara.
  - AddComment(comment comment): Shton nje koment te ri.
  - GetCommentsByPostId(int post_id):Kthen liste te komenteve te aprovuara per nje post te caktuar.
  - GetCommentsApproved(int user_id): Kthen nje liste te komenteve te aprovuara nga nje perdorues i caktuar.
  - AddReply(reply reply): Shton nje reply ne nje koment.
  - GetBlogPostsByUserId(int user_id): Kthen nje liste te posteve te aprovuara nga nje user i caktuar.
  - GetCommentById(int id): Kthen koment me id e dhene.
  - UpdateComment(comment comment): update nje koment ekzistues.
  - GetBlogPostsByCategory(string category): Kthen nje liste te postimeve te aprovuara per nje kategori te caktuar.
  - GetBlogPostsByDate(DateTime? fromDate, DateTime? toDate): Kthen nje liste te postimeve te aprovuara per nje periudhe te caktuar.
*/


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


        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: AddBlogPost
        * Pershkrimi: Kjo metode shton nje postim blogu te ri ne db dhe lidh postimin me kategorite perkatese.
        * Parametrat:
        * - post post: Objekti i postimit qe do te shtohet.
        * - List<int> categoryIds: Lista e ID-ve te kategorive me te cilat do te lidhet postimi.
        * Return: Nuk ka.
        **/
        public void AddBlogPost(post post, List<int> categoryIds)
        {
            db.posts.Add(post);
            db.SaveChanges();
            foreach (var categoryId in categoryIds)
            {
                var postCategory = new PostCategory
                {
                    post_id = post.id,
                    category_id = categoryId,
                    invalidate= 10      
                };
                db.PostCategories.Add(postCategory);
            }
            db.SaveChanges();

        }


        /**
         * Data: 26/06/2024
         * Programuesi: Rafina Tusha
         * Metoda: GetBlogPostById
         * Pershkrimi: Kjo metode kthen nje postim blogu bazuar ne ID-ne e tij.
         * Parametrat:
         * - int id: ID-ja e postimit qe do te merret.
         * Return: post: Objekti i postimit qe korrespondon me ID-ne.
         **/
        public post GetBlogPostById(int id)
        {
            var post = db.posts.Where(p => p.invalidate==10).FirstOrDefault(p => p.id == id);
            return post;
        }


        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetBlogPosts
        * Pershkrimi: Kjo metode kthen nje liste te te gjithe postimeve te blogut.
        * Return: IEnumerable<post>: Nje liste e te gjithe postimeve te blogut.
        **/

        public IEnumerable<post> GetBlogPosts()
        {
             var blogPosts = db.posts.Include("user").Include("PostCategories").ToList();


            return blogPosts;
        }


        /**
       * Data: 26/06/2024
       * Programuesi: Rafina Tusha
       * Metoda: GetBlogPostsApproved
       * Pershkrimi: Kjo metode kthen nje liste te postimeve te blogut qe jane aprovuar.
       * Return: IEnumerable<post>: Nje liste e postimeve te aprovuar te blogut.
       **/

        public IEnumerable<post> GetBlogPostsApproved()
        {
            var posts = db.posts.Where(p => p.approved == "yes").ToList();
            return posts.OrderByDescending(p => p.created_at);
        }


        /**
        * Data: 26/06/2024
        * Programuesi:Ralfina Tusha 
        * Metoda: AddComment
        * Pershkrimi: Kjo metode shton nje koment te ri ne db.
        * Parametrat:
        * - comment comment: Objekti i komentit qe do te shtohet.
        * Return: Nuk ka.
        **/
        public void AddComment(comment comment)
        {
            db.comments.Add(comment);
            db.SaveChanges();
        }


        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetCommentsByPostId
        * Pershkrimi: Kjo metode kthen nje liste te komenteve te aprovuara per nje postim te caktuar duke perfshire dhe replies.
        * Parametrat:
        * - int post_id: ID-ja e postimit.
        * Return: IEnumerable<comment>: Nje liste e komenteve te aprovuara te lidhura me postimin.
        **/

        public IEnumerable<comment> GetCommentsByPostId(int post_id)
        {
            var comments = db.comments
                             .Where(c => c.post_id == post_id && c.approved == "yes" && c.invalidate==10)
                             .OrderByDescending(c => c.created_at)
                             .ToList();
            return comments;
        }


        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetCommentsApproved
        * Pershkrimi: Kjo metode kthen nje liste te komenteve te aprovuara per nje perdorues te caktuar.
        * Parametrat:
        * - int user_id: ID-ja e perdoruesit.
        * Return: IEnumerable<comment>: Nje liste e komenteve te aprovuara te lidhura me perdoruesin.
        **/

        public IEnumerable<comment> GetCommentsApproved(int user_id)
        {
            var comments = db.comments.Where(c => c.approved == "yes" && c.user_id == user_id).ToList();
            return comments;
        }

        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: AddReply
        * Pershkrimi: Kjo metode shton nje reply te re ne nje koment.
        * Parametrat:
        * - reply reply: Objekti i reply qe do te shtohet.
        * Return: Nuk ka.
        **/

        public void AddReply(comment reply)
        {
            db.comments.Add(reply);
            db.SaveChanges();
        }

        /**
        * Data:26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetBlogPostsByUserId
        * Pershkrimi: Kjo metode kthen nje liste te postimeve te aprovuara te blogut te krijuara nga nje perdorues te caktuar
        * Parametrat:
        * - int user_id: ID-ja e perdoruesit.
        * Return: IEnumerable<post>: Nje liste e postimeve te aprovuara te lidhura me perdoruesin.
        **/

        public IEnumerable<post> GetBlogPostsByUserId(int user_id)
        {
            var posts = db.posts.Where(p => p.user_id == user_id && p.approved=="yes").ToList();
            return posts; 
        }

        /**
        * Data: 26/06/2024
        * Programuesi:Ralfina Tusha 
        * Metoda: GetCommentById
        * Pershkrimi: Kjo metode kthen nje koment bazuar ne ID-ne e tij.
         * Parametrat:
        * - int id: ID-ja e komentit qe do te kthehet.
        * Return: comment:  komenti qe korrespondon me ID-ne.
        **/

        public comment GetCommentById(int id)
        {
            var comment = db.comments.FirstOrDefault(c => c.id == id);
            return comment;   
        }
        /**
        * Data: 26/06/2024
        * Programuesi:Ralfina Tusha 
        * Metoda: UpdateComment
        * Pershkrimi: Kjo metode updates nje koment ekzistues ne db.
        * Parametrat:
        * - comment comment:komenti qe do te update-ohet.
        * Return: Nuk ka.**/
        public void UpdateComment(comment comment)
        {
            var commentToUpdate = db.comments.FirstOrDefault(c => c.id == comment.id);
            if (commentToUpdate != null)
            {
                commentToUpdate.comment1 = comment.comment1;  
                commentToUpdate.approved = comment.approved;  
                db.SaveChanges();
            }
        }

        /**
        * Data:26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetBlogPostsByCategory
        * Pershkrimi: Kjo metode kthen nje liste te postimeve te aprovuara te blogut per nje kategori te caktuar.
        * Parametrat:
        * - string category: emri i kategorise.
        * Return: IEnumerable<post>: Nje liste e postimeve te aprovuara te lidhura me kategorine.
        **/

        public IEnumerable<post> GetBlogPostsByCategory(string category)
        {
            return db.posts.Where(p => p.PostCategories.Any(pc => pc.category.name == category) && p.approved == "yes").ToList();
        }


        /**
        * Data: 26/06/2024
        * Programuesi: Rafina Tusha
        * Metoda: GetBlogPostsByDate
        * Pershkrimi: Kjo metode kthen nje liste te postimeve te aprovuara te blogut brenda nje periudhe te caktuar kohore.
        * Parametrat:
        * - DateTime? fromDate: Data e fillimit te periudhes.
        * - DateTime? toDate: Data e perfundimit te periudhes.
        * Return: IEnumerable<post>: Nje liste e postimeve te aprovuara te krijuara brenda periudhes se specifikuar.
        **/
        public IEnumerable<post> GetBlogPostsByDate(DateTime? fromDate, DateTime? toDate)
        {
            return db.posts.Where(p => p.created_at >= fromDate && p.created_at <= toDate && p.approved == "yes").ToList();
        }
    }
}

      
