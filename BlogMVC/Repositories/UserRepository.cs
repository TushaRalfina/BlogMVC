using BlogMVC.Models;
using BlogMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
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
            userViewModel.password = HashPassword(userViewModel.password);


            db.users.Add(new user
            {
                username = userViewModel.username,
                email = userViewModel.email,
                password = userViewModel.password,
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

        public user GetUserByUsername(string username)
        {
            // Fetch the user by  username
               return db.users.FirstOrDefault(u => u.username == username );
           
 
        }
         public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                 StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public IEnumerable<post> GetPostsByUserId(int id)
        {
            return db.posts.Where(p => p.user_id == id).ToList();
            
        }
    }
}

