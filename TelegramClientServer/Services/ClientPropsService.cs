using Instances;
using System.Security.Claims;
using TelegramClientServer.Interfaces;

namespace TelegramClientServer.Services
{
    public class ClientPropsService : IFController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ClientPropsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            return int.Parse(userIdClaim.Value);
        }
    }
}
