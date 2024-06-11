using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string profile_picture { get; set; }
        public string bio { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

    }
}