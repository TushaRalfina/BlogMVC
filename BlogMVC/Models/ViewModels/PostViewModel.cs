/**
* Versioni: V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: PostViewModel
* Arsyeja: Modeli i perdorur per te paraqitur informacionin e nje postimi dhe komentet e aprovuara lidhur me ate.
* Pershkrimi: Kjo klase permban nje instance te postimit (post) dhe nje liste te komenteve te aprovuara (ApprovedComments) per te cilat jane te lidhura me kete post.
*/

using System.Collections.Generic;


namespace BlogMVC.Models.ViewModels
{
    public class PostViewModel
    {
        public post Post { get; set; }
        public List<comment> ApprovedComments { get; set; }
    }

}