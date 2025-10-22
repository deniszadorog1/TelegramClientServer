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
        public bool IsSticker { get; set; }
        public string MediaName { get; set; }

        public MediaAction(int id, int senderUserId, 
            DateTime sentTime, string mediaName, 
            bool isSticker, bool isRead, bool isPinned,
            int? forwardedFromId) : 
            base(id, senderUserId, sentTime, isRead, isPinned, forwardedFromId)
        {
            IsSticker = isSticker;
            MediaName = mediaName;
        }

        public MediaAction()
        {
            Id = -1;
            SenderUserId = -1;
            MediaName = "testPATH";
            IsRead = false;
        }

        public bool IsImage()
        {
            if (IsSticker) return false;
             string ext = Path.GetExtension(MediaName);
            return ext == ".png" || ext == ".jpg" || ext == "jpeg";
        }

        public bool IsGif()
        {
            if (IsSticker) return false;
            string ext = Path.GetExtension(MediaName);
            return ext == ".gif";
        }

        public bool IsVideo()
        {
            if (IsSticker) return false;
            string ext = Path.GetExtension(MediaName);
            return ext == ".mp4";
        }

        public override string GetLastMessage()
        {
            return "Media";
        }
    }
}
