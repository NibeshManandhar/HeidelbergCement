using HeidelbergCement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Repository
{
    interface IUserRepository
    {
        public User FindUserByEmailAndPassWord(string email, string password);

    }
}
