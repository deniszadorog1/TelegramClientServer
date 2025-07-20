using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Helpers;

namespace TelegramLib.MainClasses
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name;
        public string Surname { get; set; }
        public string BIO { get; set; }
        
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public DateTime? BirthDay { get; set; }

        public ColorHelper MainColor { get; set; }

        public DateTime LastSeenOnline { get; set; }

        public User(int id, string login, string password, string name,
                    string surname, string bio,
                    ColorHelper color, string phoneNumber, 
                    string userName, DateTime? birthDay)
        {
            Id = id;
            Login = login;
            Password = password;
            Name = name;
            Surname = surname;
            BIO = bio;

            MainColor = color;

            PhoneNumber = phoneNumber;
            UserName = userName;
            BirthDay = birthDay;
        }

        //Smth like test params
        public User()
        {
            Id = -1;
            Login = "asdLOGIN";
            Password = "asdPASSWORD";
            Name = "asdNAME";
            Surname = "asdSURNAME";
            BIO = "asdBIO";
           
            MainColor = new ColorHelper(255, 0, 0);

            PhoneNumber = "asdPhoneNumber";
            UserName = "asdUserName";

            BirthDay = new DateTime(2003, 7, 4);
        }

        public bool IsSameId(int id)
        {
            return Id == id;
        }
    }
}
