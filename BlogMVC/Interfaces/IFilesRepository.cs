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
    }
}
