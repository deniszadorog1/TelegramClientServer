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

        public TextMessage(int id, int senderId, DateTime sentTime, string text) : 
            base(id, senderId, sentTime)
        {
            Text = text;
        }

        public TextMessage()
        {
            Id = -1;
            Text = "TextMessage";
            SentTime = DateTime.Now; 
        }

        public override string GetLastMessage()
        {
            return Text;
        }
    }
}
