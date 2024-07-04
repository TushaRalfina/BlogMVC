/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: AddBlogPostRequest
* Arsyeja: Modeli i perdorur për kerkesen e shtimit te nje postimi te ri ne blog.
* Pershkrimi: Kjo klase perfshin titullin, permbajtjen, user_id-ne e perdoruesit, imazhin kryesor,filet, si dhe listen e kategorive te zgjedhura per postimin e ri.
*/



using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogMVC.Models.ViewModels
{
    public class AddBlogPostRequest
    {
        [AllowHtml]
        [Required(ErrorMessage = "Title is required")]
        public string title { get; set; }

         [AllowHtml]
         [Required(ErrorMessage = "Content is required")]
        public string content { get; set; }

        public int user_id { get; set; }

        public string main_image { get; set; }

        public HttpPostedFileBase main_imagee { get; set; }

        [Required(ErrorMessage = "Please select at least one category")]
        public List<int> SelectedCategoryIds { get; set; }
        
        public List<HttpPostedFileBase> files { get; set; }

       
        public IEnumerable<category> categoriess { get; set; }




    }

}