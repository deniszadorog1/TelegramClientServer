using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Interfaces;

namespace TelegramVisualPart.Services
{
    class ProxyDataProvider : IDataProvider
    {
        public async Task<bool> AddMessageAsync(
            TelegramLib.MainClasses.Messages.Message message, 
            TelegramLib.MainClasses.UserChat chat)
        {
            return await ApiService.AddMessage(message, chat);
        }
    }
}
