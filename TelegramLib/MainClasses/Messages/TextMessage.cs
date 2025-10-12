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

        public TextMessage(int id, int senderUserId, DateTime sentTime, string text, bool isRead) :
            base(id, senderUserId, sentTime, isRead)
        {
            Text = text;
        }

        public TextMessage()
        {
            Id = -1;
            Text = "TextMessage";
            SentTime = DateTime.Now;
            IsRead = false;
        }

        public override string GetLastMessage()
        {
            return Text;
        }
    }
}
