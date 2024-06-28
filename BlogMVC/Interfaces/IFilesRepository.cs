/**
* Versioni:  
* Data: 25/06/2024  
* Programuesi: Ralfina Tusha
* Pershkrimi: Interface qe permban metodat qe mund te perdoren nga FilesRepository
* Metodat: GetFiles, AddFiles, GetFileById
(c) Copyright by Soft & Solution 
**/

 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogMVC.Models;

namespace BlogMVC.Repositories
{
    internal interface IFilesRepository
    {
        IEnumerable<file> GetFiles();


        void AddFiles(file file);


        file GetFileById(int id);

 
        IEnumerable<file> GetFilesByPostId(int id);
    }
}
