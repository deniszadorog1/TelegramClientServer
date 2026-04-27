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
        public static string Url;

        static MediaServerUrl()
        {
            if (!_isLoaded)
            {
                DotNetEnv.Env.Load();
                _isLoaded = true;
                Url = Environment.GetEnvironmentVariable("MEDIA_SERVER_URL") /*?? "http://localhost:5171"*/;
            }
        }

        public static void Load()
        {
            string envPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");

            if (System.IO.File.Exists(envPath))
            {
                DotNetEnv.Env.Load(envPath);
            }

            Url = Environment.GetEnvironmentVariable("MEDIA_SERVER_URL")
                  ?? DotNetEnv.Env.GetString("MEDIA_SERVER_URL"/*, "http://localhost:5171"*/);

            Url = Url.TrimEnd('/');
        }
    }
}
