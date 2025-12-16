using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.DTOsHelper
{
    public class EditDTO
    {
        public TelegramLib.MainClasses.Messages.TextMessage TextMes { get; set; }
        public TelegramLib.MainClasses.Messages.MediaAction MediaMes { get; set; }
    }
}
