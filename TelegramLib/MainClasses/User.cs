using System;
using System.Collections.Generic;
using System.Linq;
using TelegramLib.Helpers;
using TelegramLib.MainClasses.UserParams;

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
        public List<UserImage> UserImages { get; set; }

        //Blcoked users cant sent messages to Logged user
        public List<UserContactcs> BlockedContacts { get; set; }

        public User(int id, string login, string password, string name,
                    string surname, string bio,
                    ColorHelper color, string phoneNumber,
                    string userName, DateTime? birthDay,
                    List<UserContactcs> blockedContacts,
                    List<UserImage> userImages)
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

            BlockedContacts = blockedContacts;
            UserImages = userImages;
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
            BlockedContacts = new List<UserContactcs>();

            UserImages = new List<UserImage>();
            UserImages.Add(new UserImage("WhiteCat.png", DateTime.Now));
            UserImages.Add(new UserImage("Minato.jpg", DateTime.Now));
        }

        public bool IsSameId(int id)
        {
            return Id == id;
        }

        public UserImage GetFirstImageName()
        {
            if (UserImages is null || UserImages.Count == 0)
            {
                UserImages = new List<UserImage>();
                UserImages.Add(new UserImage("fray.jpg", DateTime.Now));
            }
            return UserImages.First();
        }

        public List<string> GetImagesNames()
        {
            List<string> res = new List<string>();

            for (int i = 0; i < UserImages.Count; i++)
            {
                res.Add(UserImages[i].Name);
            }

            return res;
        }

        public void RemoveUserImageByIndex(int index)
        {
            if (UserImages.Count <= index || UserImages.Count == 0) return;
            UserImages.RemoveAt(index);
        }
    }
}
