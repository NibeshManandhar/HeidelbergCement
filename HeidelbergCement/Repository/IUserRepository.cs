using HeidelbergCement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Repository
{
    public interface IUserRepository
    {
        public User GetUserByEmailAndPassWord(string email, string password);

    }
}
