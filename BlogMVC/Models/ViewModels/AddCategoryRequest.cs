﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ViewModels
{
    public class AddCategoryRequest
    {

         
        public string name { get; set; }
         public List<string> subcategories { get; set; }



 
 

    }
}