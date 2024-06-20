using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Models.ViewModels
{
    public class PostsCategoriesModel
    {
        public IPagedList<post> Posts { get; set; }
        public IEnumerable<category> Categories { get; set; }
    }
}