using BlogMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMVC.Repositories
{
    public interface IUserRepository
    {
        // Define the methods that will be used in the UserRepositoy class

        void AddUser(user user);

        user GetUserByUsername(string username,string password);

        user GetUserById(int id);

        void UpdateUser(user user);

        

 

    }
}
