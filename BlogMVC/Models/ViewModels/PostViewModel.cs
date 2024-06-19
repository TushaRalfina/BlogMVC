using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ViewModels
{
    public class PostViewModel
    {
        public post Post { get; set; }
        public List<comment> ApprovedComments { get; set; }


    }

}