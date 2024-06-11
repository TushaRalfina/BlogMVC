using BlogMVC.Models;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace BlogMVC.Repositories
{
    public class UserRepository : IUserRepository
    {
       
        BlogEntities db = new BlogEntities();

        public void AddUser(user user)
        {
 
            user.created_at = System.DateTime.Now;
            db.users.Add(user);
            db.SaveChanges();
        }

        public void UpdateUser(user user)
        {
            try
            {
                var existingUser = db.users.FirstOrDefault(u => u.id == user.id);
                if (existingUser != null)
                {
                    existingUser.username = user.username;
                    existingUser.password = user.password;
                    existingUser.email = user.email;
                    existingUser.role = user.role;
                    existingUser.profile_picture = user.profile_picture;
                    existingUser.bio = user.bio;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.updated_at = System.DateTime.Now;
                    db.SaveChanges();
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

        public user GetUserByUsername(string username,string password)
        {
            return db.users.FirstOrDefault(u => u.username == username && u.password == password);
        }
    }
}
