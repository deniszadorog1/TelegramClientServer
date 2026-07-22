using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.Exceptions
{
    internal class ChatException : Exception
    {
        public string Message { get; }

        public ChatException(string message)
            : base(message)
        {
            Message = message;
        }

    }
}
