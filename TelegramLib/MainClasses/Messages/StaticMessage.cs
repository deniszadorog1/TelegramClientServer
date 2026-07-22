using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Chat;

namespace TelegramLib.MainClasses.Messages
{
    public class StaticMessage : Message
    {
        public int? MessageReferenceId { get; set; }
        public AutoDeleteType? DelType { get; set; }
        public DateTime? Date { get; set; }

        public StaticMessage()
        {

        }

        public StaticMessage(DateTime? date,int senderId)
        {
            Date = date;
            SenderUserId = senderId;
        }

        public StaticMessage(AutoDeleteType? type, int senderId)
        {
            Id = -1;
            MessageReferenceId = null;
            DelType = type;
            SenderUserId = senderId;
            SentTime = DateTime.Now;
        }

        public StaticMessage(int? refMesId, int mesId, int pinnerId)
        {
            Id = mesId;
            MessageReferenceId = refMesId;
            DelType = null;
            SenderUserId = pinnerId;
            SentTime = DateTime.Now;
        }

        public override string GetLastMessage()
        {
            const string delRes = "Set deleted type";
            const string pinned = "Pinned message";

            if (!(DelType is null)) return delRes;
            return pinned;
        }
    }
}
