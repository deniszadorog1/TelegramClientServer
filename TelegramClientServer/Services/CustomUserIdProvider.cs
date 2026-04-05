using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Достаем ID из того клейма, который ты реально используешь в JWT. 
        // Если ты создавал клейм через ClaimTypes.NameIdentifier — пиши его.
        // Если писал просто "id" — пиши "id".
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? connection.User?.FindFirst("id")?.Value;
    }
}