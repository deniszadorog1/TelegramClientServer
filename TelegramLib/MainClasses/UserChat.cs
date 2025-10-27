using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public List<TelegramLib.MainClasses.Messages.Message> Messages { get; set; }
        public ChatBackground ChatBg { get; set; }
        public AutoDeleteType AutoDel { get; set; }

        public bool NotificationStatus { get; set; }
        public bool IsPinned { get; set; }
        public bool IsMarked { get; set; }

        public List<TelegramLib.MainClasses.Messages.Message> PinnedMessages { get; set; }

        public UserChat(int id, User chatter,
            List<Messages.Message> messages,
            ChatBackground bg, AutoDeleteType type,
            List<Messages.Message> pinned)
        {
            Id = id;
            Chatter = chatter;
            Messages = messages;
            ChatBg = bg;
            AutoDel = type;
            PinnedMessages = pinned;
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

        /*        public int GetMessageId(Message message)
                {
                    return Messages.Where(x => x.Id == message.Id).First().Id;
                }*/

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

        public List<Messages.Message> GetChatMessages()
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
            if (Messages.Count != 0)
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

        public Messages.Message GetLastMessageObj()
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
            for (int i = 0; i < Messages.Count; i++)
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
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i] is MediaAction media &&
                    !media.IsSticker)
                {
                    if (type != GetMediaTypeFromFilename(media.MediaName)) continue;
                    if (elIndex == 0)
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
                senderUserId, DateTime.Now, sharedContactName,
                sharedUser, false, false, null);

            Messages.Add(toAdd);
        }

        public List<Messages.Message> GetMessageByGivenIds(List<int> ids)
        {
            List<Messages.Message> res = new List<Messages.Message>();
            return Messages.Where(x => ids.Contains(x.Id)).ToList();
        }

        public int GetAmountOfUnreadMessages(int loggedUserId)
        {
            return Messages.Where(x => !x.IsRead &&
            x.SenderUserId != loggedUserId).Count();
        }

        public void ChangeNotificationStatus(bool state)
        {
            NotificationStatus = state;
        }

        public bool GetNotificationStatus()
        {
            return NotificationStatus;
        }

        public TelegramLib.MainClasses.Messages.Message GetMessageById(int id)
        {
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i].Id == id) return Messages[i];
            }
            return null;
        }

        public bool IsMessageContains(TelegramLib.MainClasses.Messages.Message mes)
        {
            return Messages.Any(x => x.Id == mes.Id);
        }

        public void AddPinnedMessage(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            //To check is contains by id

            TelegramLib.MainClasses.Messages.Message toAdd =
                PinnedMessages.FirstOrDefault(x => x.Id == mes.Id);

            if (PinnedMessages.Contains(toAdd)) return;
            mes.IsPinned = true;
            PinnedMessages.Add(mes);
        }

        public void DeletePinnedMessage(Messages.Message mes)
        {
            mes.IsPinned = false;

            TelegramLib.MainClasses.Messages.Message toRemove =
                PinnedMessages.FirstOrDefault(x => x.Id == mes.Id);
            PinnedMessages.Remove(toRemove);
        }

        public int GetPinnedMessageIndex(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            TelegramLib.MainClasses.Messages.Message res =
                PinnedMessages.FirstOrDefault(x => x.Id == mes.Id);

            return res is null ? -1 : PinnedMessages.IndexOf(res);
        }

        public Messages.Message GetNextPinnedMessage(Messages.Message mes)
        {
            int tempMesIndex = GetPinnedMessageIndex(mes);
            if (tempMesIndex == -1) return mes;

            tempMesIndex++;

            return tempMesIndex < PinnedMessages.Count ?
                PinnedMessages[tempMesIndex] :
                PinnedMessages.First();
        }

        public bool IsAnyPinnedMessage()
        {
            return PinnedMessages.Count > 0;
        }

        public void RemovePinnedMessage(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            TelegramLib.MainClasses.Messages.Message toRemove =
                PinnedMessages.FirstOrDefault(x => x.Id == mes.Id);
            if (toRemove is null) return;
            PinnedMessages.Remove(toRemove);
        }

        public void RemoveRepliedMessages(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i] is TextMessage text &&
                    !(text.RepliedMessageId is null) &&
                    text.RepliedMessageId == mes.Id)
                {
                    text.RepliedMessageId = -1;
                }
            }
        }

        public void RemovePinnedMessageById(int id)
        {
            TelegramLib.MainClasses.Messages.Message mes = 
                PinnedMessages.FirstOrDefault(x => x.Id == id);
            if (mes is null) return;

            PinnedMessages.Remove(mes);
        }

        public int GetAmountOfPinnedMessages()
        {
            return Messages.Where(x => x.IsPinned).Count();
        }
    }
}
