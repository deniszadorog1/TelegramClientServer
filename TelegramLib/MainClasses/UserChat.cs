using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramLib.Enums.Chat;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using static System.Net.Mime.MediaTypeNames;
using AutoDeleteType = TelegramLib.Enums.Chat.AutoDeleteType;
using ShareContactMessage = TelegramLib.MainClasses.Messages.ShareContactMessage;

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

        public List<Messages.Message> ScheduleMessages { get; set; }

        public UserChat(int id, User chatter,
            List<Messages.Message> messages,
            ChatBackground bg,
            AutoDeleteType type,
            List<Messages.Message> pinned,
            List<Messages.Message> scheduleMessages)
        {
            Id = id;
            Chatter = chatter;
            Messages = messages;
            ChatBg = bg;
            AutoDel = type;
            PinnedMessages = pinned;
            ScheduleMessages = scheduleMessages;
        }

        public UserChat()
        {
            //Set Test Params Here
        }

        public List<MediaAction> GetMediaMessages(bool isSched = false)
        {
            if (isSched)
            {
                return ScheduleMessages.OfType<MediaAction>().Where(x => !x.IsSticker).ToList();
            }

            //not Images(can be gifs or video) 
            return Messages.OfType<MediaAction>()
                .Where(x => !x.IsSticker).ToList();
        }


        public List<MediaAction> GetSchedVideos()
        {
            return ScheduleMessages.OfType<MediaAction>().Where(x => x.IsVideo()).ToList();
        }

        public List<MediaAction> GetChatVideos()
        {
            return Messages.OfType<MediaAction>().Where(x => x.IsVideo()).ToList();
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
            PinnedMessages.Clear();
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
            Messages = Messages.OrderBy(x => x.SentTime).ToList();
            return Messages;
        }

        public List<Messages.Message> GetOnlyPinnedMessages()
        {
            return Messages.Where(x => x.IsPinned).ToList();
        }

        public List<Messages.Message> GetScheduleMessages()
        {
            return ScheduleMessages;
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

        public string GetLastMessageInString()
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
            x.SenderUserId != loggedUserId &&
            x.GetType() != typeof(StaticMessage)).Count();
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
            return mes is null ? false : Messages.Any(x => x.Id == mes.Id);
        }

        public bool IsMessageContains(int mesId)
        {
            return Messages.Any(x => x.Id == mesId);
        }

        public bool IsInSchedMessages(Messages.Message mes)
        {
            return ScheduleMessages.Any(x => x.Id == mes.Id);
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

            PinnedMessages.Sort((a, b) => b.SentTime.CompareTo(a.SentTime));
            //PinnedMessages.Sort((a, b) => a.SentTime.CompareTo(b.SentTime));

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

        public Messages.Message GetLastPinnedMessage()
        {
            return PinnedMessages.FirstOrDefault();
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
            ClearRepliedInMessage(Messages, mes.Id);
            ClearRepliedInMessage(ScheduleMessages, mes.Id);
        }

        public void ClearRepliedInMessage(List<Messages.Message> messages, int mesId)
        {
            if (messages is null) return;
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i] is TextMessage text &&
                    !(text.RepliedMessageId is null) &&
                    text.RepliedMessageId == mesId)
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

        public int GetMesIdPairOfMessageByTime(TelegramLib.MainClasses.Messages.Message mes)
        {
            if (mes is null) return -1;

            const int maxDiffer = 20;
            for (int i = 0; i < Messages.Count; i++)
            {
                /*              bool sameBaseTime =
                                  mes.SentTime.Year == Messages[i].SentTime.Year &&
                                  mes.SentTime.Month == Messages[i].SentTime.Month &&
                                  mes.SentTime.Day == Messages[i].SentTime.Day &&
                                  mes.SentTime.Hour == Messages[i].SentTime.Hour &&
                                  mes.SentTime.Minute == Messages[i].SentTime.Minute &&
                                  mes.SentTime.Second == Messages[i].SentTime.Second;*/

                double diffMs = Math.Abs((mes.SentTime - Messages[i].SentTime).TotalMilliseconds);
                if (diffMs < maxDiffer) return Messages[i].Id;
            }
            return -1;
        }

        public List<MediaAction> GetGifMessages(bool isSched = false)
        {
            if (isSched) return ScheduleMessages.OfType<MediaAction>().Where(x => x.IsGif()).ToList();

            List<MediaAction> gifs = new List<MediaAction>();
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i] is MediaAction media &&
                    media.IsGif())
                {
                    gifs.Add(media);
                }
            }
            return gifs;
        }

        public void SetEmptyChatterImage()
        {
            Chatter.GetFirstImageName();
        }

        public bool IsStateDateExist(DateTime time)
        {
            return Messages.OfType<StaticMessage>().Any(x => !(x.Date is null) &&
            ((DateTime)x.Date).Year == time.Year &&
            ((DateTime)x.Date).Month == time.Month &&
            ((DateTime)x.Date).Day == time.Day);
        }

        public List<TelegramLib.MainClasses.Messages.Message> GetMessagesByDateTime(DateTime date)
        {
            return Messages.Where(x =>
            x.SentTime.Year == date.Year &&
            x.SentTime.Month == date.Month &&
            x.SentTime.Day == date.Day).ToList();
        }

        public void RemoveMessageById(int id)
        {
            TelegramLib.MainClasses.Messages.Message toRemove =
                Messages.FirstOrDefault(x => x.Id == id);

            if (toRemove is null) return;

            Messages.Remove(toRemove);
        }

        public TelegramLib.MainClasses.Messages.Message GetMessageByDateTime(DateTime date)
        {
            //Check for Same Day
            TelegramLib.MainClasses.Messages.Message sameDateMes =
                Messages.FirstOrDefault(x => x.IsMessageForDate(date));
            if (!(sameDateMes is null)) return sameDateMes;

            //Date close date message

            return null;
        }

        private const int _maxSentTimeDiffer = 10;
        public TelegramLib.MainClasses.Messages.Message GetMessageByFullDateTime(DateTime time)
        {
            TelegramLib.MainClasses.Messages.Message mes =
                 Messages.FirstOrDefault(x => Math.Abs((x.SentTime - time).TotalMilliseconds)
                     < _maxSentTimeDiffer);

            return mes;
        }

        public void RemoveMessageBySentTime(DateTime time)
        {
            TelegramLib.MainClasses.Messages.Message toRemove = GetMessageByFullDateTime(time);

            if (toRemove is null) return;
            Messages.Remove(toRemove);

            RemoveDateMessages();
        }

        public int GetLinksAmount()
        {
            int res = 0;
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i] is TextMessage text)
                {
                    var match = Regex.Match(text.Text, @"https?:\/\/[^\s]+");
                    if (match.Success) res++;
                }
            }
            return res;
        }

        public List<string> GetLinks()
        {
            List<string> res = new List<string>();

            for (int i = 0; i < Messages.Count; i++)
            {
                if (!(Messages[i] is TextMessage text)) continue;

                var match = Regex.Match(text.Text, @"https?:\/\/[^\s]+");

                if (!match.Success) continue;
                res.Add(match.Value);
            }
            return res;
        }

        public bool IsImageMessagesExist()
        {
            return Messages.Any(x => x is MediaAction media &&
            media.IsImage() && !media.IsSticker);
        }

        public int GetAmountOfImages()
        {
            return Messages.Where(x => x is MediaAction media &&
            media.IsImage() && !media.IsSticker).ToList().Count();
        }

        public bool IsVideoMessagesExist()
        {
            return Messages.Any(x => x is MediaAction media &&
            media.IsVideo());
        }

        public int GetAmountOfVideos()
        {
            return Messages.Where(x => x is MediaAction media &&
            media.IsVideo()).ToList().Count();
        }


        public bool IsGifMessagesExist()
        {
            return Messages.Any(x => x is MediaAction media &&
            media.IsGif());
        }

        public int GetAmountOfGifs()
        {
            return Messages.Where(x => x is MediaAction media &&
            media.IsGif()).ToList().Count();
        }

        public void RemoveDateMessages()
        {
            List<StaticMessage> toRemove = new List<StaticMessage>();
            bool isUsualMessageWas = false;
            for (int i = Messages.Count - 1; i > 0; i--)
            {
                if (!isUsualMessageWas &&
                    Messages[i] is StaticMessage stat &&
                    !(stat.Date is null))
                {
                    toRemove.Add(stat);
                }

                if (Messages is StaticMessage)
                {
                    isUsualMessageWas = false;
                }
                else isUsualMessageWas = true;
            }

            foreach (var mes in toRemove)
            {
                Messages.Remove(mes);
            }
        }

        public void AddScheduleMessage(
            TelegramLib.MainClasses.Messages.Message message,
            TelegramLib.MainClasses.User user)
        {
            ScheduleMessages.Add(message);
            UpdateScheduleMessages(user);
        }

        public void UpdateScheduleMessages(TelegramLib.MainClasses.User sender)
        {
            //Remove static messages
            RemoveStaticMessagesFromSchedule();

            //Sort by Date
            ScheduleMessages.Sort((a, b) => a.SentTime.CompareTo(b.SentTime));

            //Add Static messages(if need)

            List<DateTime> toAddStatTimes = new List<DateTime>();

            DateTime? lastDate = null;
            for (int i = 0; i < ScheduleMessages.Count; i++)
            {
                if (ScheduleMessages[i] is StaticMessage stat)
                {
                    lastDate = stat.Date?.Date;
                    continue;
                }

                var msgDate = ScheduleMessages[i].SentTime.Date;

                if (lastDate != msgDate)
                {
                    var staticMsg = new StaticMessage
                    {
                        Date = msgDate,
                        SenderUserId = sender.Id
                    };

                    ScheduleMessages.Insert(i, staticMsg);
                    i++; 
                    lastDate = msgDate;
                }
            }
        }

        public void RemoveScheduleMessage(
            int messId)
        {
            TelegramLib.MainClasses.Messages.Message toRemove =
                ScheduleMessages.FirstOrDefault(x => x.Id == messId);

            if (toRemove is null) return;

            ScheduleMessages.Remove(toRemove);
        }

        public void RemoveStaticMessagesFromSchedule()
        {
            List<Messages.Message> toRemove = 
                new List<Messages.Message>();

            for(int i = 0; i < ScheduleMessages.Count; i++)
            {
                if (ScheduleMessages[i] is StaticMessage mes)
                {
                    toRemove.Add(mes);
                }
            }

            foreach(var removeMes in toRemove)
            {
                ScheduleMessages.Remove(removeMes);
            }
        }

        public void UpdateChatMessages(List<TelegramLib.
            MainClasses.Messages.Message> newMessages)
        {
            Messages = newMessages;
        }

        public void RemoveOldSchedMessages()
        {
            List<Messages.Message> toRemove = 
               ScheduleMessages.Where(
                   x => !(x is StaticMessage) && 
               x.SentTime < DateTime.Now).ToList();

            foreach(var mes in toRemove)
            {
                ScheduleMessages.Remove(mes);
            }      
        }

        public int? GetMessageIdByText(string text)
        {
            TextMessage res =
                Messages.OfType<TextMessage>().FirstOrDefault(x => x.Text == text);

            if (res is null) return null;
            else return res.Id;
        }
    }
}
