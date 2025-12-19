using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.Messages
{
    public class TextMessage : Message
    {
        public string Text { get; set; }
        public int? RepliedMessageId { get; set; }

        public bool IsEdited { get; set; }

        public TextMessage(int id, int senderUserId, 
            DateTime sentTime, string text, bool isRead,
            int? replMessId, bool isPinned, 
            int? forwardedFromId, bool isEdited) :
            base(id, senderUserId, sentTime, isRead, isPinned, forwardedFromId)
        {
            Text = text;
            RepliedMessageId = replMessId;
            IsEdited = isEdited;
        }

        public TextMessage()
        {
            Id = -1;
            Text = "TextMessage";
            SentTime = DateTime.Now;
            IsRead = false;
            RepliedMessageId = null;
            IsEdited = false;
        }

        public override string GetLastMessage()
        {
            //Test with cleared cpaces
            string text = Text.Replace("\r\n", " ");
            return text;
        }


    }
}
