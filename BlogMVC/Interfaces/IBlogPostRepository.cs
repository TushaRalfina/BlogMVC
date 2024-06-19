using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMVC.Repositories
{
    internal interface IBlogPostRepository
    {

        void AddBlogPost(post post, List<int> categoryIds);

        IEnumerable<post> GetBlogPosts();

        post GetBlogPostById(int id);

        void UpdateBlogPost(post blogPostView);

        void DeleteBlogPost(int id);


       IEnumerable<post> GetBlogPostsApproved();

 
        void AddComment(comment comment);

        IEnumerable<comment> GetCommentsByPostId(int post_id);

        
         IEnumerable<comment> GetCommentsApproved(int user_id);


    }
}
