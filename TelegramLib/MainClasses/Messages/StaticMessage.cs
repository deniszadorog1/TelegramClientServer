using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.Messages
{
    public class StaticMessage : Message
    {
        public int? MessageReferenceId { get; set; }
        
        public StaticMessage()
        {

        }

        public StaticMessage(int? refMesId, int mesId, int pinnerId) 
        {
            Id = mesId;
            MessageReferenceId = refMesId;
            SenderUserId = pinnerId;
            SentTime = DateTime.Now;
        }
    }
}
