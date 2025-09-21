using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Messages;

namespace TelegramLib.MainClasses.Messages
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public bool IsLoggedUserSent { get; set; }
        public DateTime SentTime { get; set; }

        public Message(int id, int senderUserId, DateTime sentTime)
        {
            Id = id;
            SenderUserId = senderUserId;
            SentTime = sentTime;
        }

        public Message()
        {
            Id = -1;
            IsLoggedUserSent = true;
            SentTime = DateTime.Now;
        }

        public DateTime? GetSentDate()
        {
            return SentTime;
        }

        public virtual string GetLastMessage()
        {
            return "This is last message";
        }

        public string GetSentTimeInString()
        {
            return $"{SentTime.Day}.{SentTime.Month}.{SentTime.Year}";
        }
    }
}
