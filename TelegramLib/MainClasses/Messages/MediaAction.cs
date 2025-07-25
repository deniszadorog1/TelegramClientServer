using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Messages;

namespace TelegramLib.MainClasses.Messages
{
    public class MediaAction : Message
    {
        public bool IsSticker { get; }
        public string MediaName { get; set; }
        public MediaAction(int id, int senderId, DateTime sentTime, string mediaName, bool isSticker) : 
            base(id, senderId, sentTime)
        {
            IsSticker = isSticker;
            MediaName = mediaName;
        }

        public MediaAction()
        {
            Id = -1;
            SenderId = -1;
            MediaName = "testPATH";
        }


        public override string GetLastMessage()
        {
            return "Media";
        }

    }
}
