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
        public bool IsRead { get; set; }
        
        public bool IsPinned { get; set; }
        public int? ForwardedFromId { get; set; }

        public Message(int id, int senderUserId,
            DateTime sentTime, bool isRead, bool isPinned,
            int? forwardedFromId)
        {
            Id = id;
            SenderUserId = senderUserId;
            SentTime = sentTime;
            IsRead = isRead;
            IsPinned = isPinned;
            ForwardedFromId = forwardedFromId;
        }

        public Message()
        {
            Id = -1;
            IsLoggedUserSent = false;
            SentTime = DateTime.Now;
            IsRead = false;
            ForwardedFromId = null;
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

        public void MirrorPinStatus()
        {
            IsPinned = !IsPinned;
        }

        public bool IsMessageForDate(DateTime date)
        {
            if (SentTime.Year != date.Year ||
               SentTime.Month != date.Month ||
               SentTime.Day != date.Day) return false;

            if (this is StaticMessage stat && !(stat.Date is null)) return false;

            return true;
        }
    }
}
