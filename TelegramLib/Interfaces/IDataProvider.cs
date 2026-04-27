using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.Interfaces
{
    public interface IDataProvider
    {
        public Task<bool> AddMessageAsync(
            TelegramLib.MainClasses.Messages.Message message,
            TelegramLib.MainClasses.UserChat chat );
    }
}
