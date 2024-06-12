using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ViewModels
{
    public class CategoryPostViewModel
    {
        public IEnumerable<category> categories { get; set; }
        public IEnumerable<post> posts { get; set; }


    }
}