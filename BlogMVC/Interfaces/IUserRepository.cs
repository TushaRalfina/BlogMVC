using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMVC.Repositories
{
    public interface IUserRepository
    {

        void AddUser(UserViewModel user);

        user GetUserByUsername(string username);

        user GetUserById(int id);

        user UpdateUser(user user);

        string HashPassword(string password);

         IEnumerable<post> GetPostsByUserId(int id);

         user GetUserByVerificationToken(string token);









    }
}