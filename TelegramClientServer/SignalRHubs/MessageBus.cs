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
        public List<Message> Content { get; set; }
        public string ReceiverId { get; set; }
    }

    public static class MessageBus
    {
        private static readonly Channel<MessageEnvelope> _channel = Channel.CreateUnbounded<MessageEnvelope>();

        public static async ValueTask PublishAsync(MessageEnvelope envelope)
        {
            await _channel.Writer.WriteAsync(envelope);
        }

        public static IAsyncEnumerable<MessageEnvelope> SubscribeAsync(CancellationToken ct = default)
        {
            return _channel.Reader.ReadAllAsync(ct);
        }
    }
}
