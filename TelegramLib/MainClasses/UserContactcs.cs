using System;
using System.Collections.Generic;
using System.Linq;
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses.UserParams;

namespace TelegramLib.MainClasses
{
    public class UserContactcs
    {
        public int Id { get; set; }
        public int ContactUserId { get; set; }
        public bool IsOnline { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BIO { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LastSeen { get; set; }
        public bool IsNotificationsIsOn { get; set; }
        public List<UserImage> UserImages { get; set; }

        public UserImage MaskImage { get; set; }
        public AutoDeleteDuration AutoDeletion { get; set; }

        public UserContactcs(int id, string name, string surname, string userName,
            DateTime? birthDate,
            string bio, string phoneNumber,
            DateTime? lastSeen, bool isNotsOn,
            List<UserImage> userImages, AutoDeleteDuration deletion, 
            bool isOnline)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Login = userName;
            BirthDate = birthDate;
            BIO = bio;
            PhoneNumber = phoneNumber;
            LastSeen = lastSeen;
            IsNotificationsIsOn = isNotsOn;
            UserImages = userImages;
            AutoDeletion = deletion;
            IsOnline = isOnline;
        }

        public UserContactcs()
        {
        }

        public void SetAutoDeleteDuration(AutoDeleteType? type)
        {
            if (type is null) return;
            AutoDeletion = new AutoDeleteDuration((AutoDeleteType) type);
        }

        public bool GetNotifsState()
        {
            return IsNotificationsIsOn;
        }

        public void SetNotifState(bool state)
        {
            IsNotificationsIsOn = state;
        }


        const string _unavailable = "unavailable";
        public string GetPhoneNumber()
        {
            return PhoneNumber is null ? _unavailable : PhoneNumber;
        }

        public string GetUserName()
        {
            return Login;
        }

        public bool UserLoginsAreEqual(string login)
        {
            return Login == login;
        }

        public string GetBirthDate()
        {
            return BirthDate is null ? _unavailable :
                $"{((DateTime)BirthDate).Day}.{((DateTime)BirthDate).Month}.{((DateTime)BirthDate).Year}";
        }

        public bool IsSendersIdsAreEqual(int senderId)
        {
            return Id == senderId;
        }

        const string _baseMinato = "Minato.jpg";

        public UserImage GetFirstImageName()
        {
            if (!(MaskImage is null)) return MaskImage;
            if (UserImages is null || UserImages.Count == 0)
            {
                UserImages = new List<UserImage>();
                UserImages.Add(new UserImage(_baseMinato, DateTime.Now));
            }
            return UserImages.First();
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

        public List<string> GetImagesNames()
        {
            List<string> res = new List<string>();

            for (int i = 0; i < UserImages.Count; i++)
            {
                res.Add(UserImages[i].Name);
            }

            return res;
        }

        public void UpdateByUser(User user)
        {
            Name = user.Name;
            Login = user.Login;
            BirthDate = user.BirthDay;
            BIO = user.BIO;
            PhoneNumber = user.PhoneNumber;
            LastSeen = user.LastSeenOnline;
            UserImages = user.UserImages is null || user.UserImages.Count == 0 ? UserImages : user.UserImages;
        }

        public void RemoveMask()
        {
            if (!(MaskImage is null) && UserImages.Count >= 1) UserImages.RemoveAt(0);
            MaskImage = null;
        }
    }
}
