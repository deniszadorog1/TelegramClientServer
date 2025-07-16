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
        public int Id { get; set; }
        public UserContactcs Chatter { get; set; }
        public List<Message> Messages { get; set; }

        public UserChat(int id, UserContactcs chatter, List<Message> messages)
        {
            Id = id;
            Chatter = chatter;
            Messages = messages;
        }

        public UserChat()
        {
            //Set Test Params Here
        }

        public void ClearChat()
        {
            Messages.Clear();
        }
        
    }
}
