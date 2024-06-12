using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BlogMVC.Repositories
{
    public class UserRepository : IUserRepository
    {

        BlogEntities db = new BlogEntities();

        public void AddUser(UserViewModel userViewModel)
        {

            userViewModel.created_at = System.DateTime.Now;
            //add user to the database
            db.users.Add(new user
            {
                username = userViewModel.username,
                password = userViewModel.password,
                email = userViewModel.email,
                role = userViewModel.role,
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                bio = userViewModel.bio,
                created_at = userViewModel.created_at,
                updated_at = userViewModel.updated_at
            });
            db.SaveChanges();
        }

        public user UpdateUser(user user)
        {
            try
            {
                var existingUser = db.users.FirstOrDefault(u => u.id == user.id);

                if (existingUser != null)
                {
                    existingUser.id = user.id;
                    existingUser.username = user.username;
                    existingUser.password = user.password;
                    existingUser.role = user.role;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.email = user.email;
                    existingUser.bio = user.bio;
                    existingUser.updated_at = System.DateTime.Now;
                    db.SaveChanges();


                    return existingUser;
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public user GetUserById(int id)
        {
            return db.users.FirstOrDefault(u => u.id == id);
        }

        public user GetUserByUsername(string username, string password)
        {
            // Fetch the user by username
               return db.users.FirstOrDefault(u => u.username == username);
           

            return null;
        }
        /*public  string HashPassword(string password)
           {
               using (var sha256 = SHA256.Create())
               {
                   var bytes = Encoding.UTF8.GetBytes(password);
                   var hash = sha256.ComputeHash(bytes);
                   return Convert.ToBase64String(hash);
               }

            }*/
       } 
    }

