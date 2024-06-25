/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: FilesRepository
* Arsyeja: Implementimi i metodave për manipulimin e fileve .
* Pershkrimi: Kjo klasë ofron funksionalitete për të marrë të gjitha filet, shtuar një file të ri dhe marrë një file specifik sipas ID-së.
* Trashegon nga: 
* Interfaces: IFilesRepository
* Constants: Asnje
* Metodat: 
  - GetFiles(): Kthen nje liste te fileve.
  - AddFiles(file file): Shton nnje file te ri.
  - GetFileById(int id): Kthen nje file me id e dhene.
*/

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
