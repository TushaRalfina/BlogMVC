/**
* Versioni: V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: EditProfileRequest
* Arsyeja: Kerkese per ndryshimin e te dhenave te profili i userit.
* Pershkrimi: Kjo klase permban informacionin qe nevojitet per ndryshimin e profilin, duke perfshire id-ne e perdoruesit, email-in, emrin, mbiemrin dhe bio.
*/


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