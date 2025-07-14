using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.Messages
{
    public class MediaAction : Message
    {
        public string MediaPath { get; set; }
        public MediaAction(int id, int senderId, DateTime sentTime, string mediaPath) : 
            base(id, senderId, sentTime)
        {
            MediaPath = mediaPath;
        }

        public MediaAction()
        {
            Id = -1;
            SenderId = -1;
            MediaPath = "testPATH";
        }
    }
}
