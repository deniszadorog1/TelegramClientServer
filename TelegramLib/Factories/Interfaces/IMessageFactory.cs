using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;

namespace TelegramLib.Factories.Interfaces
{
    internal interface IMessageFactory
    {
        Task<TextMessage> CreateTextMessageAsync(int senderId, string text, int? replMessId, UserChat chat);

        Task<MediaAction> CreateMediaMessageAsync(int senderId, string mediaName, bool isSticker, UserChat chat);
    }
}
