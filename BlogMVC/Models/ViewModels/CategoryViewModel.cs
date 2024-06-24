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

        public IEnumerable<category> subcategories { get; set; }
        public IEnumerable<category> categories { get; set; }


    }
}