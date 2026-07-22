using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TelegramLib.MainClasses.Messages
{
    public class ShareContactMessage : Message
    {
        public string SharedName { get; set; }
        
        public User SharedUser { get; set; }


        public ShareContactMessage()
        {
            Id = -1;
            SenderUserId = -1;
            SentTime = DateTime.Now;
            SharedName = string.Empty;
            SharedUser = null;
            IsRead = false;
        }

        public ShareContactMessage(int id, int senderUserId,
            DateTime sentTime, string sharedContactName, 
            User shared, bool isRead, bool isPinned,
            int? forwardedFromId)
            : base(id, senderUserId, sentTime, isRead, isPinned, forwardedFromId)
        {
            SharedName = sharedContactName;
            SharedUser = shared;
        }

        public override string GetLastMessage()
        {
            const string contact = "Contact";
            return contact; 
        }

    }
}
