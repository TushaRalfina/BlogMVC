using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogMVC.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        BlogEntities db = new BlogEntities();
        public IEnumerable<file> GetFiles()
        {
           return  db.files.ToList();
        }

         //add files to the database
         public void AddFiles(file file)
            {
                db.files.Add(file);
                db.SaveChanges();
            }
    }
}