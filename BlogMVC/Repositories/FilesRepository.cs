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

        /**
          * Data: 26/06/2024
          * Programuesi: Ralfina Tusha
          * Metoda: GetFiles
          * Pershkrimi: Kjo metode kthen nje liste te te gjitha fileve ne db.
          * Return: IEnumerable<file>: Nje liste e te gjitha fileve
          **/
        public IEnumerable<file> GetFiles()
        {
           return db.files.ToList();
        }


        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: AddFiles
        * Pershkrimi: Kjo metode shton nje file te ri ne db.
        * Parametrat: file file: Objekti i file qe do te shtohet.
        * Return:void
        **/
        public void AddFiles(file file)
            {
                db.files.Add(file);
                db.SaveChanges();
            }

        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetFileById
        * Pershkrimi: Kjo metode kthen nje file bazuar ne ID-ne e tij.
        * Parametrat:int id: ID-ja e file qe do te merret.
        * Return: file: Objekti i file qe korrespondon me ID-ne.
        **/

        public file GetFileById(int id)
            {
            return db.files.FirstOrDefault(f => f.id == id);
            }
        }
    }
