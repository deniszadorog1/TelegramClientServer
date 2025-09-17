using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramVisualPart.Services
{
    public static class VisConstParamsJsonService
    {
        private static Dictionary<string, string> _dict = null;
        private static string _fileName = string.Empty;
        private  static void SetStringParams()
        {

            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
            string libPath = Path.Combine(parentPath, "TelegramVisualPart");
            string langsPath = Path.Combine(libPath, "LanguageFiles");
            string jsonFilePath = Path.Combine(langsPath, _fileName);

            string json = File.ReadAllText(jsonFilePath);
            _dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static int GetNumByName(string name)
        {
            SetStringParams();
            int.TryParse(_dict[name], out int res);
            return res;
        }

        public static string GetStringByName(string name)
        {
            SetStringParams();
            return _dict[name];
        }

        public static void SetFileName(string fileName)
        {
            _fileName = fileName;
        }


    }
}
