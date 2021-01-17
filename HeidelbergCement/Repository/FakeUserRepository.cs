using HeidelbergCement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Repository
{
    public class FakeUserRepository : IUserRepository
    { 
        private static List<User> Users= new List<User>();

        public FakeUserRepository()
        {
            Users.Add(new User() { Email = "heidelbergcement@123.com", Password = "2021@HeidelbergCement" });
            Users.Add(new User() { Email = "test", Password = "test" });
        }


        
        public User GetUserByEmailAndPassWord(string email, string password)
        {            
            return Users.FirstOrDefault(x => string.Equals(x.Email, email, StringComparison.CurrentCultureIgnoreCase) 
                                        && password.Equals(x.Password)); 
        }
    }
}
