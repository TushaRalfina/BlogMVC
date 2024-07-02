/**
* Versioni:  V 1.0.0
* Data: 25/06/2024
* Programuesi: Ralfina Tusha
* Klasa: UserRepository
* Arsyeja: Implementimi i metodave për menaxhimin e userave  
* Pershkrimi: Kjo klase ofron funksionalitete per te shtuar, update dhe marre userat, si dhe per te  hashuar passwordet e tyre.
* Interfaces: IUserRepository
* Metodat: 
  - AddUser(UserViewModel userViewModel): Regjistron nje përdorues te ri në db duke perdorur informacionin nga UserViewModel.
  - UpdateUser(user user): update  te dhenat e nje user-it ne db.
  - GetUserById(int id): Kthen nje user me id e dhene.
  - GetUserByUsername(string username): Kthen nje user nga db duke kaluar si parameter username-in e userit.
  - HashPassword(string password): Hashon  passwordin duke perdorur algoritmin SHA-256.
  - GetPostsByUserId(int id): Kthen nje liste te postimeve te aprovuara per nje user te caktuar.
*/
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


        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: AddUser
        * Arsyeja: Shtimi i nje useri te ri
        * Pershkrimi: Kjo metode shton nje suer te ri ne db duke marre informacionin nga UserViewModel.
        * Para kushti: UserViewModel duhet te permbaje te gjitha fushat e nevojshme te plotesuara dhe password-i duhet te jete i papergatitur.
        * Post kushti: useri i ri do te jete i ruajtur ne db dhe password-i do te jete i hashuar.
        * Parametrat: 
        *   - userViewModel: UserViewModel objekti qe permban te dhenat e userit te ri.
        * Return: Nuk kthen ndonje vlere.
        */
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

        /**
        * Data: 26/06/2024
        * Programuesi:Ralfina Tusha
        * Metoda: UpdateUser
        * Arsyeja: Per te perditesuar te dhenat e nje useri ekzistues
        * Pershkrimi: Kjo metode perditeson te dhenat e nje useri ekzistues ne db duke marre informacionin nga objekti user.
        * Para kushti: Perdoruesi duhet te ekzistoje ne db dhe objekti user duhet te permbaje id-ne e sakte.
        * Post kushti: Perdoruesi do te jete i perditesuar me te dhenat e reja.
        * Parametrat: 
        *   - user: objekti user qe permban te dhenat e perditesuara te perdoruesit.
        * Return: Kthen objektin user te perditesuar nese perditesimi eshte i suksesshem, perndryshe shfaq nje Exception nese ndodh ndonje gabim.
        */

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


        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetUserById
        * Pershkrimi: Kjo metode kthen nje user me id e dhene.
        * Parametrat: int id: ID-ja e userit qe do te merret.
        * Return: user: Objekti i userit qe korrespondon me id-ne.
        **/
        public user GetUserById(int id)
        {
            return db.users.FirstOrDefault(u => u.id == id);
        }


        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetUserByUsername
        * Pershkrimi: Kjo metode kthen nje user nga db duke kaluar si parameter username-in e userit.
        * Parametrat: string username: Username i userit qe do te merret.
        * Return: user: Objekti i userit qe korrespondon me username-in.
        **/
        public user GetUserByUsername(string username)
        {
                return db.users.FirstOrDefault(u => u.username == username );
        }


        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: HashPassword
        * Pershkrimi: Kjo metode hashon passwordin duke perdorur algoritmin SHA-256.
        * Parametrat: string password: Passwordi qe do te hashohet.
        * Return: string: Passwordi i hashuar.
        **/
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



        /**
        * Data: 26/06/2024
        * Programuesi: Ralfina Tusha
        * Metoda: GetPostsByUserId
        * Pershkrimi:Kjo metode kthen nje liste te postimeve per nje user te caktuar.
        * Paramentrat: int id: ID-ja e userit per te cilin do te merren postimet.
        * Return: IEnumerable<post>: Nje liste e postimeve  per nje user te caktuar.
        **/
        public IEnumerable<post> GetPostsByUserId(int id)
        {
            return db.posts.Where(p => p.user_id == id).ToList();   
        }

        //ndryshim i shtuar

        public user GetUserByEmail(string email)
        {
            return db.users.FirstOrDefault(u => u.email == email);
         }
    }
}

