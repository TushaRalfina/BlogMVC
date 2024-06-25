using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMVC.Interfaces
{
    internal interface IAdminRepository
    {

         IEnumerable<post> GetBlogPostsNotApproved();

         void ApproveBlogPost(int id);

        void DeleteBlogPosts(int id);

 
        IEnumerable<comment> GetCommentsNotApproved();

         void ApproveComment(int id);

         void DeleteComment(int id);

 
        int GetPostIdByCommentId(int id);

        
        void FshiReply(int id);
          

        int GetCommentIdByReplyId(int id);

         void AddCategory(category category);

        void AddSubCategory(string name, int category_id);









    }
}
