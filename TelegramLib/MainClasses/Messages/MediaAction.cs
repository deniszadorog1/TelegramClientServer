using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Messages;
using System.IO;

namespace TelegramLib.MainClasses.Messages
{
    public class MediaAction : Message
    {
        public bool IsSticker { get; }
        public string MediaName { get; set; }

        public MediaAction(int id, int senderUserId, DateTime sentTime, string mediaName, bool isSticker) : 
            base(id, senderUserId, sentTime)
        {
            IsSticker = isSticker;
            MediaName = mediaName;
        }

        public MediaAction()
        {
            Id = -1;
            SenderUserId = -1;
            MediaName = "testPATH";
        }

        public bool IsImage()
        {
             string ext = Path.GetExtension(MediaName);
            return ext == ".png" || ext == ".jpg" || ext == "jpeg";
        }

        public bool IsGif()
        {
            string ext = Path.GetExtension(MediaName);
            return ext == ".gif";
        }

        public bool IsVideo()
        {
            string ext = Path.GetExtension(MediaName);
            return ext == ".mp4";
        }

        public override string GetLastMessage()
        {
            return "Media";
        }

    }
}
