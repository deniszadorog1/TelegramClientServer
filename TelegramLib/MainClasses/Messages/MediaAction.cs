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
        public int BandId { get; set; }

        public MediaAction(int id, int senderUserId, 
            DateTime sentTime, string mediaName, 
            bool isSticker, bool isRead, bool isPinned,
            int? forwardedFromId, int bandId = -1) : 

            base(id, senderUserId, sentTime, isRead, isPinned, forwardedFromId)
        {
            IsSticker = isSticker;
            MediaName = mediaName;
            BandId = bandId;
        }

        public MediaAction()
        {
            Id = -1;
            SenderUserId = -1;
            MediaName = "testPATH";
            IsRead = false;
        }

        private readonly List<string> _imgsExt = new List<string>()
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".webp"
        };

        private readonly List<string> _gifExt = new List<string>()
        {
            ".gif"
        };

        private readonly List<string> _videoExt = new List<string>()
        {
            ".mp4"
        };

        public bool IsImage()
        {
            if (IsSticker) return false;
             string ext = Path.GetExtension(MediaName).ToLowerInvariant();

            return _imgsExt.Contains(ext);
            //return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".webp";
        }

        public bool IsGif()
        {
            if (IsSticker) return false;
            string ext = Path.GetExtension(MediaName);
            return _gifExt.Contains(ext);
            //return ext == ".gif";
        }

        public bool IsVideo()
        {
            if (IsSticker) return false;
            string ext = Path.GetExtension(MediaName);
            return _videoExt.Contains(ext);
            //return ext == ".mp4";
        }

        public override string GetLastMessage()
        {
            const string returnType = "Media";
            return returnType;
        }
    }

}
