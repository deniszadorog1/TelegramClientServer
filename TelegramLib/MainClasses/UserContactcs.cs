using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses
{
    public class UserContactcs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BIO { get; set; }
        public string? PhoneNumber { get; set; }
        List<string> IconsPaths { get; set; }
        public DateTime? LastSeen { get; set; }
        public bool IsNotificationsIsOn { get; set; }
        
        public bool IsBlockedUserBlocked { get; set; }

        public UserContactcs(int id, string name, string userName, 
            DateTime? birthDate,
            string bio, string? phoneNumber, List<string> iconsPaths,
            DateTime? lastSeen, bool isNotsOn)
        {
            Id = id;
            Name = name;
            UserName = userName;
            BirthDate = birthDate;
            BIO = bio;
            PhoneNumber = phoneNumber;
            IconsPaths = iconsPaths;
            LastSeen = lastSeen;
            IsNotificationsIsOn = isNotsOn;
        }

        public UserContactcs()
        {
            Id = Id;
            Name = "testNAME";
            UserName = "testUSERNAME";
            BirthDate = DateTime.Now;
            BIO = "testBIO";
            IconsPaths = new List<string>();
            LastSeen = DateTime.Now;
            IsNotificationsIsOn = true;
        }

        public bool GetNotifsState()
        {
            return IsNotificationsIsOn;
        }

        public bool IsNamesAreEqual(string name)
        {
            return Name == name;
        }

        public string GetPhoneNumber()
        {
            return PhoneNumber is null ? "unavailable" :  PhoneNumber;
        }

        public string GetUserName()
        {
            return UserName;
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

        public string GetLastImageName()
        {
            if (IconsPaths is null || IconsPaths.Count == 0)
            {
                IconsPaths = new List<string>();
                IconsPaths.Add("Fray.jpg");
            }
            return IconsPaths.Last();
        }
    }
}
