using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramVisualPart.Helper
{
    public static class MediaServerUrl
    {
        private static bool _isLoaded = false;

        public static void Load()
        {
            if (!_isLoaded)
            {
                DotNetEnv.Env.Load();
                _isLoaded = true;
            }
        }

        public static string Url = DotNetEnv.Env.GetString("MEDIA_SERVER_URL", "http://localhost:5171");
    }
}
