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

        public UserContactcs(int id, string name, string userName, 
            DateTime? birthDate,
            string bio, string? phoneNumber, List<string> iconsPaths)
        {
            Id = id;
            Name = name;
            UserName = userName;
            BirthDate = birthDate;
            BIO = bio;
            PhoneNumber = phoneNumber;
            IconsPaths = iconsPaths;
        }

        public UserContactcs()
        {
            Id = Id;
            Name = "testNAME";
            UserName = "testUSERNAME";
            BirthDate = DateTime.Now;
            BIO = "testBIO";
            IconsPaths = new List<string>();
        }
    }
}
