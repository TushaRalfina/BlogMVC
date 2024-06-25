/**
* Versioni: V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: UserProfileViewModel
* Arsyeja: Modeli i perdorur per te paraqitur informacionin e profilit te user-it.
* Pershkrimi: Kjo klase permban informacionin e nevojshem per te paraqitur profilin e nje perdoruesi, duke perfshire id-ne, emrin e perdoruesit, password-in, email-in, rolin, foto e profilit, bio, emrin dhe mbiemrin, si dhe postimet e krijuara nga user-i.
*/





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

        public virtual ICollection<post> posts { get; set; }

 
    }
}