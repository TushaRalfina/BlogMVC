/**
* Versioni: V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: CategoryViewModel
* Arsyeja: Modeli i perdorur per paraqitjen e informacionit lidhur me nje kategori.
* Pershkrimi: Kjo klase perfaqeson nje model per ta perdorur ne shfaqjen e informacionit te kategorise, duke perfshire vete kategorine, postimet ne kete kategori, nenkategorite dhe te gjitha kategorite ne pergjithesi.
*/

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