/**
* Versioni:  
* Data: 25/06/2024  
* Programuesi: Ralfina Tusha
* Pershkrimi: Interface qe permban metodat qe mund te perdoren nga AdminRepository
* Metodat: GetBlogPostsNotApproved, ApproveBlogPost, DeleteBlogPosts, GetCommentsNotApproved, ApproveComment, 
* DeleteComment, GetPostIdByCommentId, FshiReply, GetCommentIdByReplyId, AddCategory, AddSubCategory
(c) Copyright by Soft & Solution 
**/
 
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


 

        void AddCategory(category category);


        void AddSubCategory(string name, int category_id);
    }
}
