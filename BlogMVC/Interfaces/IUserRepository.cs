/**
* Versioni:  
* Data: 25/06/2024  
* Programuesi: Ralfina Tusha
* Pershkrimi: Interface qe permban metodat qe mund te perdoren nga UserRepository
* Metodat: AddUser, GetUserByUsername, GetUserById, UpdateUser, HashPassword, GetPostsByUserId
(c) Copyright by Soft & Solution 
**/


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

    }
}