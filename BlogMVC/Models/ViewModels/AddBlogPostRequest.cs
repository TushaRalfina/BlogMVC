using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Models.ViewModels
{
    public class AddBlogPostRequest
    {

        [AllowHtml]
         public string title { get; set; }

         [AllowHtml]
        public string content { get; set; }

 
        public int user_id { get; set; }

         public string main_image { get; set; }

        public HttpPostedFileBase main_imagee { get; set; }  




         public List<int> SelectedCategoryIds { get; set; }
        
        public List<HttpPostedFileBase> files { get; set; }

         public IEnumerable<SelectListItem> categories { get; set; }



    }
   
}