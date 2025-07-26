using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;

namespace TelegramLib.MainClasses
{
    public class UserChat
    {
        public int Id { get; set; }
        public UserContactcs Chatter { get; set; }
        public List<Message> Messages { get; set; }
        public ChatBackground ChatBg { get; set; }

        public AutoDeleteDuration AutoDelDuration { get; set; }

        public UserChat(int id, UserContactcs chatter, List<Message> messages, 
            ChatBackground bg, AutoDeleteDuration autoDelDuration)
        {
            Id = id;
            Chatter = chatter;
            Messages = messages;
            ChatBg = bg;
            AutoDelDuration = autoDelDuration;
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

        public void AddSticker(string name, int senderId)
        {
            Messages.Add(new MediaAction(Messages.Count + 1, senderId, DateTime.Now, name, true));
        }

        public ChatBackground GetBackground()
        {
            return ChatBg;
        }

        public void ClearChat()
        {
            Messages.Clear();
        }

        public UserContactcs GetChatter()
        {
            return Chatter;
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

        public void RemoveFirstMessage()
        {
            Messages.RemoveAt(0);
        }

        public string GetLastMessage()
        {
            return Messages.Count == 0 ? "*Will be there*" : Messages.Last().GetLastMessage();
        }
        
        public bool IsNamesAreEqual(string chatterName)
        {
            return Chatter.IsNamesAreEqual(chatterName);
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

    }
}
