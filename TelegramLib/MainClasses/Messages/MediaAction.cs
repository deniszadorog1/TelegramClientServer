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
        public string MediaName { get; set; }
        public MediaAction(int id, int senderId, DateTime sentTime, string mediaName) : 
            base(id, senderId, sentTime)
        {
            MediaName = mediaName;
        }

        public MediaAction()
        {
            Id = -1;
            SenderId = -1;
            MediaName = "testPATH";
        }

        public MediaType GetMediaTypeFromFilename()
        {
            if (string.IsNullOrWhiteSpace(MediaName))
                return MediaType.Unknown;

            string extension = Path.GetExtension(MediaName).ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".webp":
                    {
                        return MediaType.Image;
                    }
                case ".gif":
                    {
                        return MediaType.Gif;
                    }
                case ".mp4":
                case ".avi":
                case ".mov":
                case ".webm":
                case ".mkv":
                case ".wmv":
                    {
                        return MediaType.Video;
                    }
                default:
                    {
                        return MediaType.Unknown;
                    }
            }
        }

        public override string GetLastMessage()
        {
            return "Media";
        }

    }
}
