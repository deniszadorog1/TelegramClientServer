using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.Messages
{
    public interface IMessage
    {
        public int Id { get; set; }
        public DateTime SentTime { get; set; }
    }
}
