/**
* Versioni:  
* Data: 25/06/2024  
* Programuesi: Ralfina Tusha
* Pershkrimi: Interface qe permban metodat qe mund te perdoren nga BlogPostRepository
* Metodat: AddBlogPost, GetBlogPosts, GetBlogPostById, UpdateBlogPost, DeleteBlogPost, GetBlogPostsApproved, AddComment,
* GetCommentsByPostId, GetCommentsApproved, AddReply, GetBlogPostsByUserId, GetCommentById, UpdateComment, GetBlogPostsByCategory,GetBlogPostsByDate
(c) Copyright by Soft & Solution 
**/


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


         void AddReply(reply reply);


         IEnumerable<post> GetBlogPostsByUserId(int user_id);


         comment GetCommentById(int id);


         void UpdateComment(comment comment);


         IEnumerable<post> GetBlogPostsByCategory(string category);


         IEnumerable<post> GetBlogPostsByDate(DateTime? fromDate, DateTime? toDate);
    }
}
