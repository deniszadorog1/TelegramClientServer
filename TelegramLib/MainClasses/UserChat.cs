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

        public UserContactcs GetChatter()
        {
            return Chatter;
        }

        public List<Message> GetChatMessages()
        {
            return Messages;
        }

        public DateTime? GetLastMessageDateTime()
        {
            return Messages.Count == 0 ? null : Messages.Last().GetSentDate();
        }

        public string GetLastMessage()
        {
            return Messages.Count == 0 ? "*Will be there*" : Messages.Last().GetLastMessage();
        }
        
        public bool IsNamesAreEqual(string chatterName)
        {
            return Chatter.IsNamesAreEqual(chatterName);
        }

    }
}
