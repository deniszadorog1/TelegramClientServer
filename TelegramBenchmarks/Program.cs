using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using TelegramLib.Services;
using TelegramLib.MainClasses.Messages;

namespace TelegramBenchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MessageLoadBenchmark>();
        }
    }

    [MemoryDiagnoser] 
    public class MessageLoadBenchmark
    {
        private const int ChatId = 4; 

        [Benchmark]
        public List<Message> LoadAsList()
        {
            return DbService.GetMessagesByChatId(ChatId, false);
        }

        [Benchmark]
        public async Task LoadAsStream()
        {
            await foreach (var message in DbService.StreamMessagesById(ChatId))
            {
            }
        }
    }
}
