//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BlogMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class file
    {
        public int id { get; set; }
        public int post_id { get; set; }
        public string file_type { get; set; }
        public string file_name { get; set; }
        public string file_content { get; set; }
    
        public virtual post post { get; set; }
    }
}
