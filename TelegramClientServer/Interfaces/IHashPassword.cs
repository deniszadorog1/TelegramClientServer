namespace TelegramClientServer.Interfaces
{
    public interface IHashPassword
    {
        public string CreateHash(string str);
        public bool Verfy(string str, string hash); 
    }
}
