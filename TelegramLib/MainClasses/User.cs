using System;
using System.Collections.Generic;
using System.Linq;
using TelegramLib.Helpers;
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

        public UserImage ImageMask { get; set; }

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

        const string _baseMinato = "Minato.jpg";

        public void UpdateParamsByUser(User user)
        {
            if (user is null || Id != user.Id) return;

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
        }

        public bool IsSameId(int id)
        {
            return Id == id;
        }

        public UserImage GetFirstImageName()
        {
            if (UserImages is null || UserImages.Count == 0)
            {
                UserImages = new List<UserImage>()
                {
                    new UserImage()
                };
            }
            return UserImages.First();
        }

        public string GetImgName()
        {
            return ImageMask is not null ? ImageMask.Name : 
                UserImages.Count == 0 ? string.Empty : 
                UserImages.FirstOrDefault().Name;
        }

        

        public string GetFirstImageNameInString()
        {
            if (UserImages is null || UserImages.Count == 0 ||
                UserImages.First().Name == string.Empty)
            {
                return _baseMinato;
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

        public void RemoveMask()
        {
            if (!(ImageMask is null) && UserImages.Count >= 1) UserImages.RemoveAt(0);
            ImageMask = null;
        }

        public void AddUserImage(UserImage img)
        {
            const int minIndex = 1;
            if (!(ImageMask is null))
            {
                UserImages.Insert(minIndex, img);
                return;
            }
            UserImages.Insert(0, img);
        }

        public void RemoveImageByIndex(int index)
        {
            UserImages.RemoveAt(index);
        }

        public UserImage GetUserImageById(int id)
        {
            if (id < 0 || UserImages.Count - 1 < id) return null;
            return UserImages[id];
        }
    }
}
