using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ViewModels
{
    public class EditProfileRequest
    {

        public int id { get; set; }  
      

        public string email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string bio { get; set; }


    }
}