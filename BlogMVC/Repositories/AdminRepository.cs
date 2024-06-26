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

         public void AddCategory(category category)
        { 
             using (var db = new BlogEntities())
            {
                db.categories.Add(category);
                db.SaveChanges();
            }
        }

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
      


        public void ApproveBlogPost(int id)
        {
            using (var db = new BlogEntities())
            {
                var post = db.posts.Find(id);
                post.approved = "yes";
                db.SaveChanges();
            }
        }

        public void ApproveComment(int id)
        {
            using (var db = new BlogEntities())
            {
                var comment = db.comments.Find(id);
                comment.approved = "yes";
                db.SaveChanges();
            }

        }

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

        public void FshiReply(int id)
        {
            using (var db = new BlogEntities())
            {
                var reply = db.replies.Find(id);
                db.replies.Remove(reply);
                db.SaveChanges();
            }

        }

        public IEnumerable<post> GetBlogPostsNotApproved()
        {

            using (var db = new BlogEntities())
            {
                return db.posts.Include(p => p.user).Where(x => x.approved == "no").ToList();
            }

        }

        public int GetCommentIdByReplyId(int id)
        {
            using (var db = new BlogEntities())
            {
                var reply = db.replies.Find(id);
                return reply.comment_id;
            }
         }

        public IEnumerable<comment> GetCommentsNotApproved()
        {
            using (var db = new BlogEntities())
            {
                return db.comments.Include(c => c.user).Where(x => x.approved == "no").ToList();
            }
        }

 
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