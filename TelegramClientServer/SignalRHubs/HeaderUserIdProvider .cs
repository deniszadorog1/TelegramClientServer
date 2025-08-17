using Microsoft.AspNetCore.SignalR;

namespace TelegramClientServer.SignalRHubs
{
    public class HeaderUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            if (httpContext != null && 
                httpContext.Request.Headers.TryGetValue("userId", out var values))
            {
                return values.ToString();
            }
            return null;
        }
    }
}
