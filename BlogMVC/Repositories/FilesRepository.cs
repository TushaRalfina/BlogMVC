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
           return db.files.ToList();
        }

          public void AddFiles(file file)
            {
                db.files.Add(file);
                db.SaveChanges();
            }

        public file GetFileById(int id)
            {
            return db.files.FirstOrDefault(f => f.id == id);
            }
        }
    }
