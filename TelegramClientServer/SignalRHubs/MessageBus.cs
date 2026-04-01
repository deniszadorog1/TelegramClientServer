using System.Threading.Channels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TelegramLib.MainClasses.Messages; 
using TelegramLib.MainClasses; 

namespace TelegramClientServer.SignalRHubs
{

    public class MessageEnvelope
    {
        public User Sender { get; set; }
        public TextMessage Content { get; set; }
        public string ReceiverId { get; set; }
    }

    public static class MessageBus
    {
        // Створюємо канал
        private static readonly Channel<MessageEnvelope> _channel = Channel.CreateUnbounded<MessageEnvelope>();

        // Продюсер: записує в канал
        public static async ValueTask PublishAsync(MessageEnvelope envelope)
        {
            await _channel.Writer.WriteAsync(envelope);
        }

        // Консумер (Корутина): читає з каналу
        public static IAsyncEnumerable<MessageEnvelope> SubscribeAsync(CancellationToken ct = default)
        {
            return _channel.Reader.ReadAllAsync(ct);
        }
    }
}
