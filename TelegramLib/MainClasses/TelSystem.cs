using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses
{
    public class TelSystem
    {
        public User LoggedUser { get; set; }
    
        public TelSystem(User user)
        {
            LoggedUser = user;
        }
        
        //Test system
        public TelSystem()
        {
            LoggedUser = new User();
        }
    }
}
