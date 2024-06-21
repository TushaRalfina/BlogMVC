using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ViewModels
{
    public class UserViewModel


    {
        public UserViewModel()
        {
            this.role = "user";
            
        }
        public int id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(16, MinimumLength = 4, ErrorMessage = "Username must be at least 4 characters")]
        public string username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string password { get; set; }


         
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password and Confirm Password must match")]
        public string confirm_password { get; set; }

       

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Invalid email format")]
        public string email { get; set; }


        [Required(ErrorMessage = "Role is required")]
        public string role { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string profile_picture { get; set; }

        public string bio { get; set; }

        public Nullable<System.DateTime> created_at { get; set; }

        public Nullable<System.DateTime> updated_at { get; set; }





    }
}