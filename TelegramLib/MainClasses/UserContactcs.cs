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
            return;
            Id = -1;
            Name = "testNAME";
            Login = "testUSERNAME";
            BirthDate = DateTime.Now;
            BIO = "testBIO";
            LastSeen = DateTime.Now;
            IsNotificationsIsOn = true;

            AutoDeletion = null;

            IsOnline = false;

            UserImages.Add(new UserImage("Minato.jpg", DateTime.Now));
            UserImages.Add(new UserImage("WhiteCat.png", DateTime.Now));
            UserImages.Add(new UserImage("fray.jpg", DateTime.Now));
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

/*        public bool IsNamesAreEqual(string name)
        {
            return Name == name;
        }*/

        public string GetPhoneNumber()
        {
            return PhoneNumber is null ? "unavailable" : PhoneNumber;
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
            return BirthDate is null ? "unavailable" :
                $"{((DateTime)BirthDate).Day}.{((DateTime)BirthDate).Month}.{((DateTime)BirthDate).Year}";
        }

        public string GetLastSeen()
        {
            return LastSeen is null ? "recently" :
                $"{LastSeen.Value.Day}.{LastSeen.Value.Month}.{LastSeen.Value.Year}";
        }

        public bool IsSendersIdsAreEqual(int senderId)
        {
            return Id == senderId;
        }

        public UserImage GetFirstImageName()
        {
            if (!(MaskImage is null)) return MaskImage;
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
