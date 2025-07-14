using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.MainClasses.Messages;

namespace TelegramLib.MainClasses
{
    public class UserChat
    {
        public UserContactcs Chatter { get; set; }
        List<Message> Messages { get; set; }

        public UserChat(UserContactcs chatter, List<Message> messages)
        {
            Chatter = chatter;
            Messages = messages;
        }

        public UserChat()
        {
            //Set Test Params Here
        }
        
    }
}
