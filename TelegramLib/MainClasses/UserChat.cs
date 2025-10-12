using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Chat;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;

namespace TelegramLib.MainClasses
{
    public class UserChat
    {
        public int Id { get; set; }
        public User Chatter { get; set; }
        public List<Message> Messages { get; set; }
        public ChatBackground ChatBg { get; set; }
        public AutoDeleteType AutoDel { get; set; }

        public bool IsPinned { get; set; }
        public bool IsMarked { get; set; }

        public UserChat(int id, User chatter, List<Message> messages, 
            ChatBackground bg, AutoDeleteType type)
        {
            Id = id;
            Chatter = chatter;
            Messages = messages;
            ChatBg = bg;
            AutoDel = type;
        }   

        public UserChat()
        {
            //Set Test Params Here
        }

        public List<MediaAction> GetMediaMessages()
        {
            //not Images(can be gifs or video) 
            return Messages.OfType<MediaAction>()
                .Where(x => !x.IsSticker).ToList();
        }

        public int GetMessageId(Message message)
        {
            return Messages.Where(x => x.Id == message.Id).First().Id;
        }

/*        public void AddSticker(string name, int senderId)
        {
            Messages.Add(new MediaAction(Messages.Count + 1, senderId, DateTime.Now, name, true));
        }*/

        public ChatBackground GetBackground()
        {
            return ChatBg;
        }

        public void ClearChat()
        {
            Messages.Clear();
        }

        public User GetChatter()
        {
            return Chatter;
        }

        public string GetLastSeen()
        {
            string lastSeen = $"{Chatter.LastSeenOnline.Day}.{Chatter.LastSeenOnline.Month}.{Chatter.LastSeenOnline.Year}";
            return lastSeen;
        }

        public List<Message> GetChatMessages()
        {
            return Messages;
        }

        public DateTime? GetLastMessageDateTime()
        {
            return Messages.Count == 0 ? null : Messages.Last().GetSentDate();
        }

        public DateTime? GetFirstMessageDateTime()
        {
            return Messages.Count == 0 ? null : Messages.First().GetSentDate();
        }

        public string GetLastMesDateInString()
        {
            string res = string.Empty;
            if(Messages.Count != 0)
            {
                DateTime? time = Messages.Last().GetSentDate();
                if (time is null) return res;
                res = $"{time.Value.Day}.{time.Value.Month}.{time.Value.Year}";
            }
            return res;
        }

        public void RemoveFirstMessage()
        {
            Messages.RemoveAt(0);
        }

        public string GetLastMessage()
        {
            return Messages.Count == 0 ? "*Will be there*" : Messages.Last().GetLastMessage();
        }

        public Message GetLastMessageObj()
        {
            return Messages.Last();
        }

        /*        public bool IsNamesAreEqual(string chatterName)
                {
                    return Chatter.IsNamesAreEqual(chatterName);
                }*/

        public bool IsUserLoginsAreEqual(string login)
        {
            return Chatter.Login == login;
        }

        public List<TextMessage> GetMessagesWithGivenText(string text)
        {
            return Messages.OfType<TextMessage>().Where(x => x.Text.Contains(text)).ToList();
        }

        public int? GetMessageIndexByText(string text)
        {
            for(int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i] is TextMessage textMess && 
                    textMess.Text == text)
                {
                    return i;
                }
            }
            return null;
        }

        public void RemoveElementByIndex(int elIndex, MediaType type)
        {
            for(int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i] is MediaAction media && 
                    !media.IsSticker)
                {
                    if (type != GetMediaTypeFromFilename(media.MediaName)) continue;
                    if(elIndex == 0)
                    {
                        Messages.Remove(media);
                        return;
                    }                    
                    elIndex--;
                }
            }
        }

        public MediaType GetMediaTypeFromFilename(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return MediaType.Unknown;

            string extension = Path.GetExtension(path).ToLowerInvariant();

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

        public bool IsChatterIdsAreEqual(int chatterId) 
        {
            return Chatter.Id == chatterId;
        }

        public void AddSharedMessage(int senderUserId, int id,
            User sharedUser, string sharedContactName)
        {
            ShareContactMessage toAdd = new ShareContactMessage(id,
                senderUserId, DateTime.Now, sharedContactName, sharedUser, false);

            Messages.Add(toAdd);
        }

        public List<Message> GetMessageByGivenIds(List<int> ids)
        {
            List<Message> res = new List<Message>();
            return Messages.Where(x => ids.Contains(x.Id)).ToList();
        }

        public int GetAmountOfUnreadMessages(int loggedUserId)
        {
            return Messages.Where(x => !x.IsRead && 
            x.SenderUserId != loggedUserId).Count();
        }
    }
}
