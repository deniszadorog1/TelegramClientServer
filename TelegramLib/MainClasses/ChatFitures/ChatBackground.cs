namespace TelegramLib.MainClasses.ChatFitures
{
    public class ChatBackground
    {
        public string FileName { get; set; }
        public bool IsBlurred { get; set; }
        public bool IsGeneral { get; set; }

        public ChatBackground(string path, bool isBlurred,
            bool isGeneral)
        {
            FileName = path;
            IsBlurred = isBlurred;
            IsGeneral = isGeneral;
        }

        public ChatBackground()
        {
            FileName = string.Empty;
            IsBlurred = false;
            IsGeneral = false;
        }

        public void SetPath(string path)
        {
            FileName = path;
        }

        public void SetBlurState(bool blurState)
        {
            IsBlurred = blurState;
        }

        public string GetFileName()
        {
            return FileName;
        }

        public bool GetBlurState()
        {
            return IsBlurred;
        }

        public bool GetIsGeneral()
        {
            return IsGeneral;
        }

        public void SetIsGeneral(bool isGeneral)
        {
            IsGeneral = isGeneral;
        }
    }
}
