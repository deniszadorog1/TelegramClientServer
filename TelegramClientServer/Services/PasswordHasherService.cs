using Microsoft.AspNetCore.Identity;
using TelegramClientServer.Interfaces;

namespace TelegramClientServer.Services
{
    public class PasswordHasherService : IHashPassword
    {
        public string CreateHash(string str)
        {
            return BCrypt.Net.BCrypt.HashPassword(str);
        }

        public bool Verfy(string str, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(str, hash);
        }

    }
}
