using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Factories.Interfaces;
using TelegramLib.Interfaces;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;

namespace TelegramLib.Factories.Async
{
    internal class MessageFactory : IMessageFactory
    {
        private readonly IDataProvider _dataProvider;
        public MessageFactory(IDataProvider data)
        {
            _dataProvider = data;
        }

        public async Task<TextMessage> CreateTextMessageAsync(
            int senderId, string text, int? replMessId, UserChat chat)
        {
            TextMessage mes = new TextMessage(
                        id: -1,
                        senderUserId: senderId,
                        sentTime: DateTime.Now,
                        text: text,
                        isRead: false,
                        replMessId: replMessId,
                        isPinned: false,
                        forwardedFromId: null,
                        isEdited: false
                    );
            
            await _dataProvider.AddMessageAsync(mes, chat);

            return mes;
        }

        public async Task<MediaAction> CreateMediaMessageAsync(
            int senderId, string mediaName, bool isSticker, UserChat chat)
        {
            MediaAction media = new MediaAction(
                        id: -1,
                        senderUserId: senderId,
                        sentTime: DateTime.Now,
                        mediaName: mediaName,
                        isSticker: isSticker,
                        isRead: false,
                        isPinned: false,
                        forwardedFromId: null,
                        bandId: -1
            );

            await _dataProvider.AddMessageAsync(media, chat);
            return media;
        }
    }
}
