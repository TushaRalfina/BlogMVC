using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace BlogMVC.Models.ViewModels
{
    public class CategoryViewModel
    {
        public category category { get; set; }
        public List<post> posts { get; set; }

        //PostCategories is a join table between posts and categories
        public List<PostCategory> PostCategories { get; set; }
    }
}