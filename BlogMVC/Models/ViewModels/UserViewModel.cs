/**
* Versioni: V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: UserViewModel
* Arsyeja: Modeli i perdorur per paraqitjen dhe validimin e te dhenave te perdoruesve ne formen te regjistrimit.
* Pershkrimi: Kjo klase permban fushat e nevojshme per te paraqitur dhe validuar te dhenat e perdoruesit, duke perfshire id-ne, emrin e perdoruesit, password-in dhe konfirmimin e tij, email-in, rolin, emrin dhe mbiemrin, foton e profilit, bio, daten e krijimit dhe te update-imit te profilit, si dhe  kodin per verifikimin e email-it.
*/




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
        public string VerificationCode { get; set; }





    }
}