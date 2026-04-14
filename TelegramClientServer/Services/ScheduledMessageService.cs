using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using TelegramClientServer.SignalRHubs;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Services;

namespace TelegramClientServer.Services
{
    public class ScheduledMessageService : IHostedService, IDisposable
    {
        private readonly IHubContext<MainHub> _hubContext;
        private Timer _timer = null;
        private bool _isRunning = false;

        public ScheduledMessageService(IHubContext<MainHub> hubContext)
        {             
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ => await Tick(), null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
            return Task.CompletedTask;
        }

        private async Task Tick()
        {
            if (_isRunning) return;

            try
            {
                _isRunning = true;

                //Get Amount of updated users to send notification
               (HashSet<int> toSendNotification, HashSet<int> chatIdsToUpdate) = 
                    DbService.GetUserIdsSentSchedMessages();

                if (toSendNotification.Count > 0)
                {
                    foreach(var num in toSendNotification)
                    {
                        await _hubContext.Clients.User(num.ToString()).
                            SendAsync("UpdateAfterSchedMessages", chatIdsToUpdate);
                    }
                }
            }
/*            catch (Exception ex)
            {
                Console.WriteLine("Cant set message box there!!(");
            }*/
            finally
            {
                _isRunning = false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0); 
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
