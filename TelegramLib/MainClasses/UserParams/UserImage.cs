using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.UserParams
{
    public class UserImage
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public UserImage(string name, DateTime date)
        {
            Name = name;
            Date = date;
        }

        public UserImage()
        {
            Name = "fray.jpg";
            Date = DateTime.Now;
        }
    }
}
