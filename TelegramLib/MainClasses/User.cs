using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name;
        public string BIO { get; set; }

        public User(string login, string password, string name, string bio)
        {
            Login = login;
            Password = password;
            Name = name;
            BIO = bio;
        }



    }
}
