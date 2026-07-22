using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Windows.Input;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace TelegramVisualPart.Services
{
    public static class VisConstParamsJsonService
    {
        private static Dictionary<string, string> _dict = null;
        private static string _fileName = string.Empty;
        private  static void SetStringParams()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string langsPath = Path.Combine(baseDir, "LanguageFiles");
            string jsonFilePath = Path.Combine(langsPath, _fileName);

            if (!File.Exists(jsonFilePath))
            {
                _dict = new Dictionary<string, string>();
                return;
            }

            string json = File.ReadAllText(jsonFilePath, Encoding.UTF8);
            _dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static int GetNumByName(string name)
        {
            SetStringParams();
            if (_dict != null && _dict.TryGetValue(name, out string value))
            {
                int.TryParse(value, out int res);
                return res;
            }
            return 0; 
        }

        public static string GetStringByName(string name)
        {
            SetStringParams();
            if (_dict != null && _dict.TryGetValue(name, out string value))
            {
                return value;
            }
            return $"[{name}]";
        }

        public static void SetFileName(string fileName)
        {
            _fileName = fileName;
        }


    }
}
