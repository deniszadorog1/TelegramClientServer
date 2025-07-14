using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.Messages
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public DateTime SentTime { get; set; }
    
        public Message(int id, int senderId, DateTime sentTime)
        {
            Id = id;
            SenderId = senderId;
            SentTime = sentTime;
        }

        public Message()
        {
            Id = -1;
            SenderId = -1;
            SentTime = DateTime.Now;
        }
         
    }
}
