using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Models.ViewModels
{
    public class AddBlogPostRequest
    {

        [AllowHtml]
        //title of the blog post
        public string title { get; set; }

        //content of the blog post
        [AllowHtml]
        public string content { get; set; }

        //id of the user who created the blog post

        public int user_id { get; set; }

        

        //display categories
        public List<int> SelectedCategoryIds { get; set; }
        public IEnumerable<SelectListItem> categories { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }   


    }
}