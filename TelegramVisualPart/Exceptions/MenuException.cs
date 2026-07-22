using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramVisualPart.Exceptions
{
    internal class MenuException : Exception
    {
        public string Message { get; }

        public MenuException(string message)
            : base(message)
        {
            Message = message;
        }
    }
}
