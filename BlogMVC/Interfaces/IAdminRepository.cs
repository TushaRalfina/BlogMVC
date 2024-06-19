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

        //get blog posts that are not approved
        IEnumerable<post> GetBlogPostsNotApproved();

        //approve blog post
        void ApproveBlogPost(int id);

        void DeleteBlogPosts(int id);

        //GetCommentsNotApproved

        IEnumerable<comment> GetCommentsNotApproved();

        //ApproveComment
        void ApproveComment(int id);

        //DeleteComment
        void DeleteComment(int id);

        //GetPostIdByCommentId

        int GetPostIdByCommentId(int id);



    }
}
