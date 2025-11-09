using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TelegramLib.Helpers;
using TelegramLib.MainClasses.UserParams;
using TelegramLib.Models;
using UserImage = TelegramLib.MainClasses.UserParams.UserImage;

namespace TelegramLib.MainClasses
{
    public class User
    {
        public int Id { get; set; }
        public bool IsOnline { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string BIO { get; set; }

        public string PhoneNumber { get; set; }
        public DateTime? BirthDay { get; set; }

        public ColorHelper MainColor { get; set; }
        public DateTime LastSeenOnline { get; set; }
        public List<UserImage> UserImages { get; set; }

        //Blcoked users cant sent messages to Logged user
        public List<User> BlockedUsers { get; set; }

        public User(int id, string login, string password, string name,
                    string surname, string bio,
                    ColorHelper color, string phoneNumber, DateTime? birthDay,
                    List<User> blockedContacts,
                    List<UserImage> userImages, DateTime lastSeen, 
                    bool isOnline)
        {
            Id = id;
            Login = login;
            Password = password;
            Name = name;
            Surname = surname;
            BIO = bio;

            MainColor = color;

            PhoneNumber = phoneNumber;
            BirthDay = birthDay;

            BlockedUsers = blockedContacts;
            UserImages = userImages;

            LastSeenOnline = lastSeen;
            IsOnline = isOnline;
        }

        public void UpdateParamsByUser(User user)
        {
            if (user is null || Id  != user.Id) return;

            Login = user.Login;
            Password = user.Password;
            Name = user.Name;
            Surname = user.Surname;
            BIO = user.BIO;

            PhoneNumber = user.PhoneNumber;
            BirthDay = user.BirthDay;
        }

        //Smth like test params
        public User()
        {
            return;
            Id = -1;
            Login = "emptyLOGIN";
            Password = "emptyPASSWORD";
            Name = "emptyNAME";
            Surname = "emptySURNAME";
            BIO = "emptyBIO";

            MainColor = new ColorHelper(-1, 255, 0, 0);

            PhoneNumber = "emptyPhoneNumber";

            BirthDay = new DateTime(2000, 1, 1);
            BlockedUsers = new List<User>();

            IsOnline = false;

            /*            UserImages = new List<UserImage>();
                        UserImages.Add(new UserImage("WhiteCat.png", DateTime.Now));
                        UserImages.Add(new UserImage("Minato.jpg", DateTime.Now));*/
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

        public string GetFirstImageNameInString()
        {
            if (UserImages is null || UserImages.Count == 0 ||
                UserImages.First().Name == string.Empty)
            {
                return "fray.jpg";
            }
            return UserImages.First().Name;
        }

        public List<string> GetUserImagesNames()
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

        public void AddBlockedContact(User contact)
        {
            if (!(BlockedUsers.FirstOrDefault(x => x.Id == contact.Id) is null)) return;
            BlockedUsers.Add(contact);
        }

        public void UnblockUserById(int id)
        {
            User toUnblock = BlockedUsers.FirstOrDefault(x => x.Id == id);
            if (toUnblock is null) return;
            BlockedUsers.Remove(toUnblock);
        }

        public bool IsUserIsBlockedById(int userId)
        {
            return BlockedUsers.Any(x => x.Id == userId);
        }

        public string GetLastSeenInChat()
        {
            return $"{LastSeenOnline.Day}.{LastSeenOnline.Month}.{LastSeenOnline.Year}";
        }

        public bool IsIdsAreEqual(int id)
        {
            return Id == id;
        }
    }
}
